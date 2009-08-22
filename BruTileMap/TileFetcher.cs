// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.

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
using Tiling;

namespace BruTileMap
{
  public delegate void FetchCompletedEventHander(object sender, FetchCompletedEventArgs e);

  class TileFetcher<T>
  {
    #region Fields

    public event FetchCompletedEventHander FetchCompleted;
    ITileCache<byte[]> fileCache;
    MemoryCache<T> memoryCache;
    List<TileKey> inProgress = new List<TileKey>();
    IList<TileInfo> tilesNeeded = new List<TileInfo>();
    ITileProvider tileProvider;
    int maxThreads = 1;
    int threadCount = 0;
    Thread workerThread;
    Extent extent;
    double resolution;
    static System.Collections.Generic.IComparer<TileInfo> sorter = new Sorter();
    ITileSchema schema;
    bool busy;
    bool closing = false;
    AutoResetEvent waitHandle = new AutoResetEvent(false);

    #endregion

    #region Constructors Destructors

    public TileFetcher(ITileProvider tileProvider, MemoryCache<T> memCache, ITileSchema schema)
      : this(tileProvider, memCache, schema, (ITileCache<byte[]>)new NullCache())
    {
    }

    public TileFetcher(ITileProvider tileProvider, MemoryCache<T> memoryCache, ITileSchema schema, ITileCache<byte[]> fileCache)
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
      System.Threading.Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

      while (!closing)
      {
        waitHandle.WaitOne();
        Thread.Sleep(100);
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
        const int preCacheLevels = 0;
        List<TileInfo> tiles;

        int level = Tile.GetNearestLevel(schema.Resolutions, resolution);
        int lastLevel = Math.Max(0, level - preCacheLevels);

        IList<TileInfo> newTilesNeeded = new List<TileInfo>();
        // Iterating through the current and a number of higher resolution so we can 
        // fall back on higher levels when lower levels are not available. 
        while (level >= lastLevel)
        {
          //todo: first get tiles and then sort
          tiles = GetPrioritizedTiles(extent, level);
          foreach (TileInfo tile in tiles)
          {
            if (memoryCache.Find(tile.Key) != null) continue;
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
        StartFetchOnThread(tile);
        counter++;
      }
    }

    private void StartFetchOnThread(TileInfo tile)
    {
      threadCount++;

      FetchOnThread fetchOnThread = new FetchOnThread(tileProvider, tile, fileCache, new FetchCompletedEventHander(LocalFetchCompleted));
      Thread thread = new Thread(fetchOnThread.FetchTile);
      thread.Name = "Tile Fetcher";
      thread.Start();
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
        throw new NotImplementedException(); //and should not
      }

      public byte[] Find(TileKey key)
      {
        return null;
      }
    }

    #endregion
  }

  public class FetchCompletedEventArgs
  {
    public FetchCompletedEventArgs(Exception error, bool cancelled, TileInfo tileInfo, byte[] image)
    {
      this.Error = error;
      this.Cancelled = cancelled;
      this.TileInfo = tileInfo;
      this.Image = image;
    }

    public Exception Error;
    public bool Cancelled;
    public TileInfo TileInfo;
    public byte[] Image;
  }
}
