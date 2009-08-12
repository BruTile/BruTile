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
using Tiling;
using System.Threading;

namespace BruTileMap
{
    public delegate void FetchCompletedEventHander(object sender, FetchCompletedEventArgs e);

    class TileFetcher<T> : IDisposable
    {
        #region Fields

        public event FetchCompletedEventHander FetchCompleted;
        ITileCache<byte[]> fileCache;
        List<TileKey> inProgress = new List<TileKey>();
        IList<TileInfo> tilesNeeded = new List<TileInfo>();
        ITileProvider tileProvider;
        object syncRoot = new object();
        int maxThreads = 4;
        int threadCount = 0;
        Timer timer;
        Extent extent;
        Extent previousExtent;
        double resolution;
        static System.Collections.Generic.IComparer<TileInfo> sorter = new Sorter();
        MemoryCache<T> memoryCache;
        ITileSchema schema;
        bool busy;
        
        #endregion

        #region Constructors

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

            try
            {
                timer = new Timer(new TimerCallback(TimerTick), this, 300, 300);
            }
            catch
            {
            }
        }

        #endregion

        public void UpdateData(Extent extent, double resolution)
        {
            this.extent = extent;
            this.resolution = resolution;
        }

        private static void TimerTick(object state)
        {
            lock (state)
            {
                TileFetcher<T> tileFetcher = (TileFetcher<T>)state;

                tileFetcher.RenewQueue();

                tileFetcher.FetchTiles();
            }
        }

        private void RenewQueue()
        {
            if (extent == previousExtent) return;

            if (busy == true) return;
            busy = true;

            try
            {
                const int preCacheLevels = 0;
                const int priorityStep = 1000000;
                List<TileInfo> tiles;

                int level = Tile.GetNearestLevel(schema.Resolutions, resolution);
                int lastLevel = Math.Max(0, level - preCacheLevels);

                IList<TileInfo> newTilesNeeded = new List<TileInfo>();
                // Iterating through a number of extra resolution so we can fall back on 
                // higher levels when lower levels are not available. 
                while (level >= lastLevel)
                {
                    tiles = GetPrioritizedTiles(extent, level, priorityStep * level);
                    foreach (TileInfo tile in tiles)
                    {
                        newTilesNeeded.Add(tile);
                    }
                    level--;
                }
                tilesNeeded = newTilesNeeded;
                previousExtent = extent;
            }
            finally
            {
                busy = false;
            }
        }

        private List<TileInfo> GetPrioritizedTiles(Extent extent, int resolution, int basePriority)
        {
            List<TileInfo> tiles = new List<TileInfo>(Tile.GetTiles(schema, extent, resolution));

            for (int i = 0; i < tiles.Count; i++)
            {
                double priority = basePriority - Distance(extent.CenterX, extent.CenterY,
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
                if (inProgress.Contains(tile.Key)) continue;
                inProgress.Add(tile.Key);
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

        private class Sorter : IComparer<TileInfo>
        {
            public int Compare(TileInfo x, TileInfo y)
            {
                if (x.Priority > y.Priority) return -1;
                if (x.Priority < y.Priority) return 1;
                return 0;
            }
        }

        private void LocalFetchCompleted(object sender, FetchCompletedEventArgs e)
        {
            if (inProgress.Contains(e.TileInfo.Key)) inProgress.Remove(e.TileInfo.Key);
            threadCount--;
            if (this.FetchCompleted != null)
                this.FetchCompleted(this, e);
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
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
