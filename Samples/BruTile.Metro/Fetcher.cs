﻿﻿// Copyright 2008 - Paul den Dulk (Geodan)

using BruTile;
using BruTile.Cache;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Xaml.Controls;

namespace SharpMap.Fetcher
{
    class Fetcher
    {
        private readonly ITileCache<Image> memoryCache;
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

        public event DataChangedEventHandler DataChanged;

        public Fetcher(ITileSource tileSource, ITileCache<Image> memoryCache)
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

        private async void StartFetchLoop()
        {
            await ThreadPool.RunAsync(FetchLoop, WorkItemPriority.Low, WorkItemOptions.TimeSliced);
        }

        public void AbortFetch()
        {
            isAborted = true;
            waitHandle.Set(); // active fetch loop so it can run out of the loop
        }

        private void FetchLoop(IAsyncAction operation)
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
            ITileCache<Image> memoryCache, Retries retries)
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
            ThreadPool.RunAsync(
                (source) =>
                {
                    Exception error = null;
                    byte[] image = null;

                    try
                    {
                        if (tileSource != null) image = tileSource.Provider.GetTile(tileInfo);
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
                        DataChanged(this, new DataChangedEventArgs(error, false, tileInfo, image));

                }, WorkItemPriority.Low, WorkItemOptions.TimeSliced);
        }

        /// <summary>
        /// Keeps track of retries per tile. This class doesn't do much interesting work
        /// but makes the rest of the code a bit easier to read.
        /// </summary>
        class Retries
        {
            private readonly IDictionary<TileIndex, int> retries = new Dictionary<TileIndex, int>();
            private int maxRetries = 2;

            public bool ReachedMax(TileIndex index)
            {
                int retryCount = (!retries.Keys.Contains(index)) ? 0 : retries[index];
                return retryCount >= maxRetries;
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

    public delegate void DataChangedEventHandler(object sender, DataChangedEventArgs e);

    public class DataChangedEventArgs
    {
        public DataChangedEventArgs(Exception error, bool cancelled, TileInfo tileInfo, byte[] image)
        {
            Error = error;
            Cancelled = cancelled;
            TileInfo = tileInfo;
            Image = image;
        }

        public Exception Error { get; private set; }
        public bool Cancelled { get; private set; }
        public TileInfo TileInfo { get; private set; }
        public byte[] Image { get; private set; }
    }
}