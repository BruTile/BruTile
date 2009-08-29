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

namespace BruTileMap
{
  class TileFetcher<T>
  {
    #region Fields

    private ITileCache<byte[]> fileCache;
    private MemoryCache<T> memoryCache;
    private List<TileKey> inProgress = new List<TileKey>();
    private IList<TileInfo> tilesNeeded = new List<TileInfo>();
    private IFetchTile tileProvider;
    private int maxThreads = 4;
    private int threadCount = 0;
    private Thread workerThread;
    private Extent extent;
    private double resolution;
    private System.Collections.Generic.IComparer<TileInfo> sorter = new Sorter();
    private ITileSchema schema;
    private bool busy;
    private bool closing = false;
    private AutoResetEvent waitHandle = new AutoResetEvent(false);
    
    #endregion

    #region EventHandlers

    public event FetchCompletedEventHandler FetchCompleted;
    
    #endregion
    
    #region Constructors Destructors

    public TileFetcher(IFetchTile tileProvider, MemoryCache<T> memoryCache, ITileSchema schema)
      : this(tileProvider, memoryCache, schema, (ITileCache<byte[]>)new NullCache())
    {
    }

    public TileFetcher(IFetchTile tileProvider, MemoryCache<T> memoryCache, ITileSchema schema, ITileCache<byte[]> fileCache)
    {
      if (tileProvider == null) throw new ArgumentException("TileProvider can not be null");
      if (memoryCache == null) throw new ArgumentException("MemoryCache can not be null");
      if (schema == null) throw new ArgumentException("ITileSchema can not be null");
      if (fileCache == null) throw new ArgumentException("FileCache can not be null");

      this.tileProvider = tileProvider;
      this.memoryCache = memoryCache;
      this.schema = schema;
      this.fileCache = fileCache;

      workerThread = new Thread(TileFetchMethod);
      workerThread.Start();
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
      if ((this.extent == extent) && (this.resolution == resolution)) return;
      this.extent = extent;
      this.resolution = resolution;
      if (threadCount < maxThreads)
        waitHandle.Set();
    }

    #endregion

    #region Private Methods

    private void TileFetchMethod()
    {
#if !SILVERLIGHT
      System.Threading.Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
#endif
      while (!closing)
      {
        waitHandle.WaitOne();

#if PocketPC
        Thread.Sleep(100); //TODO: check if we really need this
#endif
        RenewQueue();
        FetchTiles();
        if (this.tilesNeeded.Count == 0)
          waitHandle.Reset();
        else
          waitHandle.Set();
      }
    }

    private void RenewQueue()
    {
      if (busy == true) return;
      busy = true;

      try
      {
        List<TileInfo> tiles;

        int level = Tile.GetNearestLevel(schema.Resolutions, resolution);
        IList<TileInfo> newTilesNeeded = new List<TileInfo>();
        //dirty way to fetch highest level tiles.
        UpdatePermaCache(newTilesNeeded);

        // Iterating through the current and a number of higher resolution so we can 
        // fall back on higher levels when lower levels are not available. 
        while (level >= 0)
        {
          //todo: first get tiles and then sort
          tiles = GetPrioritizedTiles(extent, level);
          foreach (TileInfo tile in tiles)
          {
            if (memoryCache.Find(tile.Key) != null) continue;
            if ((tile.Key.Row >= 0) && (tile.Key.Col >= 0))
              newTilesNeeded.Add(tile);
          }
          level--;
        }
        tilesNeeded = newTilesNeeded;
      }
      finally
      {
        busy = false;
      }
    }

    private void UpdatePermaCache(IList<TileInfo> newTilesNeeded)
    {
      //Note: The permaCache implementation is a temporary solution
      IList<TileInfo> tiles = Tile.GetTiles(schema, schema.Extent, 0);

      foreach (TileInfo tile in tiles)
      {
        if (memoryCache.Find(tile.Key) != null) continue;
        newTilesNeeded.Add(tile);
      }
    }
    /// <summary>
    /// Gets a list of tiles in the order in which they should be retrieved
    /// </summary>
    /// <param name="extent"></param>
    /// <param name="resolution"></param>
    /// <returns></returns>
    private List<TileInfo> GetPrioritizedTiles(Extent extent, int resolution)
    {
      List<TileInfo> tiles = new List<TileInfo>(Tile.GetTiles(schema, extent, resolution));
      for (int i = 0; i < tiles.Count; i++)
      {
        double priority = -Distance(extent.CenterX, extent.CenterY,
          tiles[i].Extent.CenterX, tiles[i].Extent.CenterY);
        tiles[i].Priority = priority;
      }
      tiles.Sort(sorter);
      return tiles;
    }

    private static double Distance(double x1, double y1, double x2, double y2)
    {
      return Math.Sqrt(Math.Pow(x1 - x2, 2f) + Math.Pow(y1 - y2, 2f));
    }

    private void FetchTiles()
    {
      int count = maxThreads - threadCount;
      int counter = 0;
      foreach (TileInfo tile in tilesNeeded)
      {
        if (counter >= count) return;
        if (memoryCache.Find(tile.Key) != null) continue;
        lock (inProgress)
        {
          if (inProgress.Contains(tile.Key)) continue;
          inProgress.Add(tile.Key);
        }
        threadCount++;
        tileProvider.GetTile(tile, LocalFetchCompleted);
        counter++;
      }
    }

    private void LocalFetchCompleted(object sender, FetchCompletedEventArgs e)
    {
      lock (inProgress)
      {
        if (inProgress.Contains(e.TileInfo.Key))
          inProgress.Remove(e.TileInfo.Key);
      }
      threadCount--;

      if (this.FetchCompleted != null)
        this.FetchCompleted(this, e);
    }

    private class Sorter : IComparer<TileInfo>
    {
      public int Compare(TileInfo x, TileInfo y)
      {
        if (x.Priority > y.Priority) return -1;
        if (x.Priority < y.Priority) return 1;
        return 0;
      }
    }

    private class NullCache : ITileCache<byte[]>
    {
      public void Add(TileKey key, byte[] image)
      {
        //do nothing
      }

      public void Remove(TileKey key)
      {
        throw new NotSupportedException(); //and should not
      }

      public byte[] Find(TileKey key)
      {
        return null;
      }
    }

    #endregion
  }


}
