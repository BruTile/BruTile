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
    class TileFetcher<T> : IDisposable
    {
        #region Fields

        private MemoryCache<T> memoryCache;
        private ITileSource tileSource;
        private ITileFactory<T> tileFactory;
        private Extent extent;
        private double resolution;
        private IList<TileKey> tilesInProgress = new List<TileKey>();
        private Queue<TileKey> tilesOutProgress = new Queue<TileKey>();
        private IList<TileInfo> tilesNeeded = new List<TileInfo>();
        private IDictionary<TileKey, int> retries = new Dictionary<TileKey, int>();
        private int threadMax = 4;
        private int threadCount = 0;
        private bool closing = false;
        private AutoResetEvent waitHandle = new AutoResetEvent(false);
        private IFetchStrategy strategy = new FetchStrategy();
        private int maxRetries = 2;
        private bool needUpdate = false;

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

            Thread loopThread = new Thread(TileFetchLoop);
            loopThread.Start();
        }

        ~TileFetcher()
        {
        }

        #endregion

        #region Public Methods

        public void ViewChanged(Extent extent, double resolution)
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

        private void TileFetchLoop()
        {
#if !SILVERLIGHT //In Silverlight there is no such thing as thread priority
            System.Threading.Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
#endif
            while (!closing)
            {
                waitHandle.WaitOne();

                UpdateInProgress();

                if (needUpdate)
                {
                    int level = BruTile.Utilities.GetNearestLevel(tileSource.Schema.Resolutions, resolution);
                    tilesNeeded = strategy.GetTilesWanted(tileSource.Schema, extent, level);
                    retries.Clear();
                    needUpdate = false;
                }

                tilesNeeded = GetTilesLacking(tilesNeeded, memoryCache);

                FetchTiles();

                if ((this.tilesNeeded.Count == 0) || (threadCount >= threadMax))
                    waitHandle.Reset();
                else
                    waitHandle.Set();
            }
        }

        private IList<TileInfo> GetTilesLacking(IList<TileInfo> tilesIn, MemoryCache<T> memoryCache)
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

        private void UpdateInProgress()
        {
            lock (tilesOutProgress)
            {
                foreach (TileKey key in tilesOutProgress)
                {
                    tilesInProgress.Remove(key);
                }
                tilesOutProgress.Clear();
            }
        }

        private void FetchTiles()
        {
            foreach (TileInfo tile in tilesNeeded)
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
            tilesInProgress.Add(tile.Key);
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
                lock (tilesOutProgress)
                {
                    tilesOutProgress.Enqueue(e.TileInfo.Key);
                }
                waitHandle.Set();
            }

            if (this.DataChanged != null)
                this.DataChanged(this, e);
        }

        #endregion
        
        #region IDisposable Members

        public void Dispose()
        {
            closing = true;
            waitHandle.Set();
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
