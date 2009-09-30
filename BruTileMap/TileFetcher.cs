// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Threading;
using BruTile;

#if SILVERLIGHT
using System.Windows;
#endif

namespace BruTileMap
{
  class TileFetcher<T>
  {
    #region Fields

    private MemoryCache<T> memoryCache;
    private IFetchTile tileProvider;
    private ITileSchema schema;
    private ITileFactory<T> tileFactory;
    private Extent extent;
    private double resolution;
    private IList<TileKey> tilesInProgress = new List<TileKey>();
    private IDictionary<TileKey, bool> tilesOutProgress = new Dictionary<TileKey, bool>();
    private IList<TileInfo> tilesNeeded = new List<TileInfo>();
    private IDictionary<TileKey, int> retries = new Dictionary<TileKey, int>();
    private int threadMax = 4;
    private int threadCount = 0;
    private bool closing = false;
    private AutoResetEvent waitHandle = new AutoResetEvent(false);
    private IFetchStrategy strategy = new FetchStrategy();
    private int maxRetries = 2;
    private volatile bool needUpdate = false;

#if SILVERLIGHT
    //TODO: see if we can move all the dispatcher stuff to the TileFactory.
    //NOTE: A first attempt showed that it is hard to do if you want GetTile to return the bytes synchronously.
    System.Windows.Threading.Dispatcher dispatcherUIThread;
#endif

    #endregion

    #region EventHandlers

    public event FetchCompletedEventHandler FetchCompleted;

    #endregion

    #region Constructors Destructors

    public TileFetcher(IFetchTile tileProvider, MemoryCache<T> memoryCache, ITileSchema schema, ITileFactory<T> tileFactory)
    {
#if SILVERLIGHT
      dispatcherUIThread = GetUIThreadDispatcher();
#endif

      if (tileProvider == null) throw new ArgumentException("TileProvider can not be null");
      this.tileProvider = tileProvider;

      if (memoryCache == null) throw new ArgumentException("MemoryCache can not be null");
      this.memoryCache = memoryCache;

      if (schema == null) throw new ArgumentException("ITileSchema can not be null");
      this.schema = schema;

      if (tileFactory == null) throw new ArgumentException("ITileFactory can not be null");
      this.tileFactory = tileFactory;

      Thread loopThread = new Thread(TileFetchLoop);
      loopThread.Start();
    }

    ~TileFetcher()
    {
      closing = true;
      waitHandle.Set();
    }

    #endregion

    #region Public Methods

    public void UpdateData(Extent extent, double resolution)
    {
      //ignore if there is no change
      if ((this.extent == extent) && (this.resolution == resolution)) return;

      this.extent = extent;
      this.resolution = resolution;
      needUpdate = true;

      if (threadCount < threadMax) //don't wake up the fetch loop if we are waiting for tile fetch threads to return
        waitHandle.Set();
    }

    #endregion

    #region Private Methods

#if SILVERLIGHT
    private static System.Windows.Threading.Dispatcher GetUIThreadDispatcher()
    {
      //based dispatcher solution on: http://marlongrech.wordpress.com/category/threading/
      if (Application.Current != null &&
        Application.Current.RootVisual != null &&
        Application.Current.RootVisual.Dispatcher != null)
      {
        if (!Application.Current.RootVisual.Dispatcher.CheckAccess())
        {
          throw new ValidationException("The TileLayer class can only be created on the UIThread");
        }
        return Application.Current.RootVisual.Dispatcher;
      }
      else // if we did not get the Dispatcher throw an exception
      {
        throw new InvalidOperationException("This object must be initialized after that the RootVisual has been loaded");
      }
    }
#endif

    private void TileFetchLoop()
    {
#if !SILVERLIGHT //In Silverlight there is no such thing as thread priority
      System.Threading.Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
#endif

      while (!closing)
      {
        waitHandle.WaitOne();

        int level = Tile.GetNearestLevel(schema.Resolutions, resolution);
        if (needUpdate)
        {
          try
          {
            tilesNeeded = strategy.GetTilesNeeded(schema, extent, level);
            retries.Clear();
          }
          finally
          {
            needUpdate = false;
          }
        }

        UpdateProgress();
        FetchTiles();

        if (this.tilesNeeded.Count == 0)
          waitHandle.Reset();
        else
          waitHandle.Set();
      }
    }

    private void UpdateProgress()
    {
      lock (tilesOutProgress)
      {
        foreach (KeyValuePair<TileKey, bool> item in tilesOutProgress)
        {
          tilesInProgress.Remove(item.Key);

          if (!item.Value) //failed
          {
            if (!retries.Keys.Contains(item.Key)) retries.Add(item.Key, 0);
            else retries[item.Key]++;
          }
          else //success
          {
            retries.Remove(item.Key);
          }
        }
        tilesOutProgress.Clear();
      }
    }

    private void FetchTiles()
    {
      TileInfo tile;

      //first a number of checks
      if (threadCount >= threadMax) return;
      if (tilesNeeded.Count == 0) return;
        
      tile = tilesNeeded[0];

      if (tilesInProgress.Contains(tile.Key))
      {
        return;
      }

      if (retries.Keys.Contains(tile.Key) && retries[tile.Key] >= maxRetries)
      {
        tilesNeeded.Remove(tile);
        return;
      }

      if (memoryCache.Find(tile.Key) != null)
      {
        tilesNeeded.Remove(tile);
        return;
      }

      //now we can go for the request.
      tilesInProgress.Add(tile.Key);
      threadCount++;
      tileProvider.GetTile(tile, tileProvider_FetchCompleted);
    }

    private void tileProvider_FetchCompleted(object sender, FetchCompletedEventArgs e)
    {
#if SILVERLIGHT
      dispatcherUIThread.BeginInvoke(delegate() { LocalFetchCompleted(sender, e); });
#else
      LocalFetchCompleted(sender, e);
#endif
    }

    private void LocalFetchCompleted(object sender, FetchCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        threadCount--;
        lock (tilesOutProgress)
        {
          tilesOutProgress[e.TileInfo.Key] = false;
        }
      }
      else
      {
        try
        {
          memoryCache.Add(e.TileInfo.Key, tileFactory.GetTile(e.Image));
        }
        catch (Exception ex)
        {
          e.Error = ex;
        }
        finally
        {
          threadCount--;
          lock (tilesOutProgress)
          {
            tilesOutProgress[e.TileInfo.Key] = true;
          }
        }
      }

      if (this.FetchCompleted != null)
        this.FetchCompleted(this, e);
    }

    #endregion
  }


}
