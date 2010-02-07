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
using BruTile.Cache;

namespace BruTile.UI.Fetcher
{
    class TileFetcher<T> 
    {
        #region Fields

        private MemoryCache<T> memoryCache;
        private ITileSource tileSource;
        private ITileFactory<T> tileFactory;
        private Extent extent;
        private double resolution;
        private IList<TileKey> tilesInProgress = new List<TileKey>();
        private IList<TileInfo> tiles = new List<TileInfo>();
        private IDictionary<TileKey, int> retries = new Dictionary<TileKey, int>();
        private int threadMax = 4;
        private int threadCount = 0;
        private AutoResetEvent waitHandle = new AutoResetEvent(true);
        private IFetchStrategy strategy = new FetchStrategy();
        private int maxRetries = 2;
        private Thread loopThread;
        private volatile bool isDone = true;
        private volatile bool isViewChanged = false;
        
        #endregion

        #region EventHandlers

        public event DataChangedEventHandler DataChanged;

        #endregion

        #region Constructors Destructors

        public TileFetcher(ITileSource source, MemoryCache<T> memoryCache, ITileFactory<T> tileFactory)
        {
            if (source == null) throw new ArgumentException("TileProvider can not be null");
            this.tileSource = source;

            if (memoryCache == null) throw new ArgumentException("MemoryCache can not be null");
            this.memoryCache = memoryCache;

            if (tileFactory == null) throw new ArgumentException("ITileFactory can not be null");
            this.tileFactory = tileFactory;
        }

        ~TileFetcher()
        {
        }

        #endregion

        #region Public Methods

        public void ViewChanged(Extent extent, double resolution)
        {
            this.extent = extent;
            this.resolution = resolution;
            isViewChanged = true;

            if (isDone)
            {
                isDone = false;
                loopThread = new Thread(TileFetchLoop);
                loopThread.IsBackground = true;
                loopThread.Name = "LoopThread";
                Console.WriteLine("Start Thread");
                loopThread.Start();
            }
        }

        public void AbortFetch()
        {
            isDone = true;
            waitHandle.Set();
        }

        #endregion

        #region Private Methods

        private void TileFetchLoop()
        {
            try
            {
#if !SILVERLIGHT //In Silverlight there is no such thing as thread priority
                System.Threading.Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
#endif
                waitHandle.Set();

                while (!isDone)
                {
                    waitHandle.WaitOne();

                    if (isViewChanged)
                    {
                        int level = BruTile.Utilities.GetNearestLevel(tileSource.Schema.Resolutions, resolution);
                        tiles = strategy.GetTilesWanted(tileSource.Schema, extent, level);
                        retries.Clear();
                        isViewChanged = false;
                    }

                    tiles = GetTilesMissing(tiles, memoryCache);

                    FetchTiles();

                    Console.WriteLine(threadCount);

                    if (this.tiles.Count == 0)
                    {
                        isDone = true;
                        waitHandle.Set();
                    }

                    if (threadCount >= threadMax)
                        waitHandle.Reset();
                }
            }
            finally
            {
                isDone = true;
            }
        }

        private IList<TileInfo> GetTilesMissing(IList<TileInfo> tilesIn, MemoryCache<T> memoryCache)
        {
            IList<TileInfo> tilesOut = new List<TileInfo>();
            foreach (TileInfo tile in tilesIn)
            {
                if ((memoryCache.Find(tile.Key) == null) &&
                    (!retries.Keys.Contains(tile.Key) || retries[tile.Key] < maxRetries))

                    tilesOut.Add(tile);
            }
            return tilesOut;
        }

        private void FetchTiles()
        {
            foreach (TileInfo tile in tiles)
            {
                if (threadCount >= threadMax) return;
                FetchTile(tile);
            }
        }

        private void FetchTile(TileInfo tile)
        {
            //first a number of checks
            if (tilesInProgress.Contains(tile.Key)) return;
            if (retries.Keys.Contains(tile.Key) && retries[tile.Key] >= maxRetries) return;
            if (memoryCache.Find(tile.Key) != null) return;

            //now we can go for the request.
            lock (tilesInProgress) { tilesInProgress.Add(tile.Key); }
            if (!retries.Keys.Contains(tile.Key)) retries.Add(tile.Key, 0);
            else retries[tile.Key]++;
            threadCount++;
            StartFetchOnThread(tile);
        }

        private void StartFetchOnThread(TileInfo tile)
        {
            FetchOnThread fetchOnThread = new FetchOnThread(tileSource.Provider, tile, new DataChangedEventHandler(LocalFetchCompleted));
            Thread thread = new Thread(fetchOnThread.FetchTile);
            thread.Name = "Tile Fetcher";
            Console.WriteLine("Start tile fetcher");
            thread.Start();
        }

        private void LocalFetchCompleted(object sender, DataChangedEventArgs e)
        {
            //todo remove object sender
            try
            {
                if (e.Error == null && e.Cancelled == false)
                    memoryCache.Add(e.TileInfo.Key, tileFactory.GetTile(e.Image));
            }
            catch (Exception ex)
            {
                e.Error = ex;
            }
            finally
            {
                threadCount--;
                lock (tilesInProgress)
                {
                    if (tilesInProgress.Contains(e.TileInfo.Key))
                        tilesInProgress.Remove(e.TileInfo.Key);
                }
                waitHandle.Set();
            }

            if (this.DataChanged != null)
                this.DataChanged(this, e);
        }

        #endregion
    }

    public delegate void DataChangedEventHandler(object sender, DataChangedEventArgs e);

    public class DataChangedEventArgs
    {
        public DataChangedEventArgs(Exception error, bool cancelled, TileInfo tileInfo, byte[] image)
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
