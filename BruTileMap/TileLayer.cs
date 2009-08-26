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
using System.ComponentModel;
using Tiling;

#if SILVERLIGHT
using System.Windows;
#endif

namespace BruTileMap
{
  public class TileLayer<T> : IDisposable
  {
    #region Fields

    ITileSchema schema;
    TileFetcher<T> tileFetcher;
    public event AsyncCompletedEventHandler DataUpdated;
    MemoryCache<T> memoryCache = new MemoryCache<T>(100, 200);
    const int maxRetries = 3;
    ITileFactory<T> tileFactory;

#if SILVERLIGHT
    //TODO: see if we can move all the dispatcher stuff to the TileFactory.
    //NOTE: First attempts showed that it is hard to do if you want GetTile to return the bytes synchronously.
    System.Windows.Threading.Dispatcher dispatcherUIThread;
#endif

    #endregion

    #region Properties

    public ITileSchema Schema
    {
      //TODO: check if we need realy need this property
      get { return schema; }
    }

    public MemoryCache<T> MemoryCache
    {
      get { return memoryCache; }
    }

    #endregion

    #region Constructors

    public TileLayer(IFetchTile tileProvider, ITileSchema schema, ITileFactory<T> tileFactory)
      : this(tileProvider, schema, tileFactory, new NullCache())
    {
    }

    public TileLayer(IFetchTile tileProvider, ITileSchema schema, ITileFactory<T> tileFactory, ITileCache<byte[]> fileCache)
    {
#if SILVERLIGHT
      dispatcherUIThread = GetUIThreadDispatcher();
#endif
      this.schema = schema;
      tileFetcher = new TileFetcher<T>(tileProvider, memoryCache, schema, fileCache);
      this.tileFactory = tileFactory;
      RegisterEventHandlers();
    }

    #endregion

    #region Public Methods

    public void UpdateData(Extent extent, double resolution)
    {
      tileFetcher.UpdateData(extent, resolution);
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

    private void RegisterEventHandlers()
    {
      tileFetcher.FetchCompleted += new FetchCompletedEventHandler(tileFetcher_FetchCompleted);
    }

    private void UnRegisterEventHandlers()
    {
      tileFetcher.FetchCompleted -= new FetchCompletedEventHandler(tileFetcher_FetchCompleted);
    }

    private void tileFetcher_FetchCompleted(object sender, FetchCompletedEventArgs e)
    {
#if SILVERLIGHT
      dispatcherUIThread.BeginInvoke(delegate() { FetchCompletedOnUIThread(e); });
#else
      FetchCompletedOnUIThread(e);
#endif
    }

    private void FetchCompletedOnUIThread(FetchCompletedEventArgs e)
    {
      if (!e.Cancelled && e.Error == null)
      {
        TileInfo tile = e.TileInfo;
        byte[] image = e.Image;
        System.Exception error = e.Error;
        if (memoryCache.Find(tile.Key) == null)
        {
          try
          {
            T bitmap = tileFactory.GetTile(image);
            memoryCache.Add(tile.Key, bitmap);
          }
          catch (Exception ex)
          {
            error = ex;
          }
        }
        else
        {
        }
        OnDataUpdated(new AsyncCompletedEventArgs(error, e.Cancelled, null));
      }
      else if ((e.Error is System.Net.WebException) && (e.TileInfo.Retries < maxRetries))
      {
        e.TileInfo.Retries++;
        //todo: implement retries
      }
      else
      {
        OnDataUpdated(new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
      }
    }

    private void OnDataUpdated(AsyncCompletedEventArgs e)
    {
      if (DataUpdated != null)
        DataUpdated(this, e);
    }
    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        UnRegisterEventHandlers();
      }
    }

    #endregion

    #region Private classes

    private class NullCache : ITileCache<byte[]>
    {
      public NullCache()
      {
      }

      public void Add(TileKey key, byte[] image)
      {
        //do nothing
      }

      public void Remove(TileKey key)
      {
        throw new NotImplementedException(); //and should not
      }

      public byte[] Find(TileKey key)
      {
        return null;
      }
    }

    #endregion
  }
}
