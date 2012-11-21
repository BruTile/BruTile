﻿// Copyright 2008 - Paul den Dulk (Geodan)

using BruTile;
using BruTile.Cache;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BruTile.Samples.Common
{
    public class Fetcher<T>
    {
        private readonly ITileCache<Tile<T>> memoryCache;
        private readonly ITileSource tileSource;
        private Extent extent;
        private double resolution;
        private readonly IList<TileIndex> tilesInProgress = new List<TileIndex>();
        private const int MaxThreads = 2;
        private readonly AutoResetEvent waitHandle = new AutoResetEvent(true);
        private readonly IFetchStrategy strategy = new FetchStrategy();
        private volatile bool isAborted = false;
        private volatile bool isViewChanged;
        private Retries retries = new Retries();

        public event DataChangedEventHandler<T> DataChanged;

        public Fetcher(ITileSource tileSource, ITileCache<Tile<T>> memoryCache)
        {
            if (tileSource == null) throw new ArgumentException("TileProvider can not be null");
            this.tileSource = tileSource;

            if (memoryCache == null) throw new ArgumentException("MemoryCache can not be null");
            this.memoryCache = memoryCache;

            StartFetchLoop();
        }

        public void ViewChanged(Extent newExtent, double newResolution)
        {
            extent = newExtent;
            resolution = newResolution;
            isViewChanged = true;
            waitHandle.Set();
        }

        private void StartFetchLoop()
        {
            ThreadPool.QueueUserWorkItem(FetchLoop);
        }

        public void AbortFetch()
        {
            isAborted = true;
            waitHandle.Set(); // active fetch loop so it can run out of the loop
        }

        private void FetchLoop(object state)
        {
            IEnumerable<TileInfo> tilesWanted = null;

            while (!isAborted)
            {
                waitHandle.WaitOne();

                if (tileSource.Schema == null)
                {
                    waitHandle.Reset();    // set in wait mode 
                    continue;              // and go to begin of loop to wait
                }

                if (isViewChanged || tilesWanted == null)
                {
                    int level = BruTile.Utilities.GetNearestLevel(tileSource.Schema.Resolutions, resolution);
                    tilesWanted = strategy.GetTilesWanted(tileSource.Schema, extent, level);
                    retries.Clear();
                    isViewChanged = false;
                }

                var tilesMissing = GetTilesMissing(tilesWanted, memoryCache, retries);

                FetchTiles(tilesMissing);

                if (tilesMissing.Count == 0) { waitHandle.Reset(); }

                if (tilesInProgress.Count >= MaxThreads) { waitHandle.Reset(); }
            }
        }

        private static IList<TileInfo> GetTilesMissing(IEnumerable<TileInfo> tilesWanted,
            ITileCache<Tile<T>> memoryCache, Retries retries)
        {
            var tilesNeeded = new List<TileInfo>();
            foreach (TileInfo info in tilesWanted)
            {
                if (memoryCache.Find(info.Index) == null && !retries.ReachedMax(info.Index))
                    tilesNeeded.Add(info);
            }
            return tilesNeeded;
        }

        private void FetchTiles(IEnumerable<TileInfo> tilesMissing)
        {
            foreach (TileInfo info in tilesMissing)
            {
                if (tilesInProgress.Count >= MaxThreads) return;
                FetchTile(info);
            }
        }

        private void FetchTile(TileInfo info)
        {
            // first some checks
            if (tilesInProgress.Contains(info.Index)) return;
            if (retries.ReachedMax(info.Index)) return;

            // prepare for request
            lock (tilesInProgress) { tilesInProgress.Add(info.Index); }
            retries.PlusOne(info.Index);

            // now we can go for the request.
            FetchAsync(info);
        }

        private void FetchAsync(TileInfo tileInfo)
        {
            ThreadPool.QueueUserWorkItem(
                (source) =>
                {
                    Exception error = null;
                    Tile<T> tile = null;

                    try
                    {
                        if (tileSource != null)
                        {
                            byte[] data = tileSource.Provider.GetTile(tileInfo);
                            tile = new Tile<T> { Data = data, Info = tileInfo };
                        }
                    }
                    catch (Exception ex) //This may seem a bit weird. We catch the exception to pass it as an argument. This is because we are on a worker thread here, we cannot just let it fall through. 
                    {
                        error = ex;
                    }

                    lock (tilesInProgress)
                    {
                        if (tilesInProgress.Contains(tileInfo.Index))
                            tilesInProgress.Remove(tileInfo.Index);
                    }

                    waitHandle.Set();

                    if (DataChanged != null && !isAborted)
                        DataChanged(this, new DataChangedEventArgs<T>(error, false, tile));

                });
        }

        /// <summary>
        /// Keeps track of retries per tile. This class doesn't do much interesting work
        /// but makes the rest of the code a bit easier to read.
        /// </summary>
        class Retries
        {
            private readonly IDictionary<TileIndex, int> retries = new Dictionary<TileIndex, int>();
            private int maxRetries = 0;

            public bool ReachedMax(TileIndex index)
            {
                int retryCount = (!retries.Keys.Contains(index)) ? 0 : retries[index];
                return retryCount > maxRetries;
            }

            public void PlusOne(TileIndex index)
            {
                if (!retries.Keys.Contains(index)) retries.Add(index, 0);
                else retries[index]++;
            }

            public void Clear()
            {
                retries.Clear();
            }
        }
    }

    public delegate void DataChangedEventHandler<T>(object sender, DataChangedEventArgs<T> e);

    public class DataChangedEventArgs <T>
    {
        public DataChangedEventArgs(Exception error, bool cancelled, Tile<T> tile)
        {
            Error = error;
            Cancelled = cancelled;
            Tile = tile;
        }

        public Exception Error { get; private set; }
        public bool Cancelled { get; private set; }
        public Tile<T> Tile { get; private set; }
    }
}