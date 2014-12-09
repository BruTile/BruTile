﻿// Copyright 2008 - Paul den Dulk (Geodan)

using System.Linq;
using BruTile.Cache;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BruTile.Samples.Common
{
    public class Fetcher<T>
    {
        private readonly ITileCache<Tile<T>> _memoryCache;
        private readonly ITileSource _tileSource;
        private Extent _extent;
        private double _resolution;
        private readonly IList<TileIndex> _tilesInProgress = new List<TileIndex>();
        private const int MaxThreads = 4;
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private readonly IFetchStrategy _strategy = new FetchStrategy();
        private volatile bool _isAborted;
        private volatile bool _isViewChanged;
        private readonly Retries _retries = new Retries();

        public event DataChangedEventHandler<T> DataChanged;

        public Fetcher(ITileSource tileSource, ITileCache<Tile<T>> memoryCache)
        {
            if (tileSource == null) throw new ArgumentException("TileProvider can not be null");
            _tileSource = tileSource;

            if (memoryCache == null) throw new ArgumentException("MemoryCache can not be null");
            _memoryCache = memoryCache;

            StartFetchLoop();
        }

        public void ViewChanged(Extent newExtent, double newResolution)
        {
            _extent = newExtent;
            _resolution = newResolution;
            _isViewChanged = true;
            _waitHandle.Set();
        }

        private void StartFetchLoop()
        {
            ThreadPool.QueueUserWorkItem(FetchLoop);
        }

        public void AbortFetch()
        {
            _isAborted = true;
            _waitHandle.Set(); // active fetch loop so it can run out of the loop
        }

        private void FetchLoop(object state)
        {
            IEnumerable<TileInfo> tilesWanted = null;

            while (!_isAborted)
            {
                _waitHandle.WaitOne();

                if (_tileSource.Schema == null)
                {
                    _waitHandle.Reset();    // set in wait mode 
                    continue;              // and go to begin of loop to wait
                }

                if (_isViewChanged || tilesWanted == null)
                {
                    var levelId = Utilities.GetNearestLevel(_tileSource.Schema.Resolutions, _resolution);
                    tilesWanted = _strategy.GetTilesWanted(_tileSource.Schema, _extent, levelId);
                    _retries.Clear();
                    _isViewChanged = false;
                }

                var tilesMissing = GetTilesMissing(tilesWanted, _memoryCache, _retries);

                FetchTiles(tilesMissing);

                if (tilesMissing.Count == 0) { _waitHandle.Reset(); }

                if (_tilesInProgress.Count >= MaxThreads) { _waitHandle.Reset(); }
            }
        }

        private static IList<TileInfo> GetTilesMissing(IEnumerable<TileInfo> tilesWanted,
            ITileCache<Tile<T>> memoryCache, Retries retries)
        {
            return tilesWanted.Where(
                info => memoryCache.Find(info.Index) == null && 
                !retries.ReachedMax(info.Index)).ToList();
        }

        private void FetchTiles(IEnumerable<TileInfo> tilesMissing)
        {
            foreach (TileInfo info in tilesMissing)
            {
                if (_tilesInProgress.Count >= MaxThreads) return;
                FetchTile(info);
            }
        }

        private void FetchTile(TileInfo info)
        {
            // first some checks
            if (_tilesInProgress.Contains(info.Index)) return;
            if (_retries.ReachedMax(info.Index)) return;

            // prepare for request
            lock (_tilesInProgress) { _tilesInProgress.Add(info.Index); }
            _retries.PlusOne(info.Index);

            // now we can go for the request.
            FetchAsync(info);
        }

        private void FetchAsync(TileInfo tileInfo)
        {
            ThreadPool.QueueUserWorkItem(
                source =>
                {
                    Exception error = null;
                    Tile<T> tile = null;

                    try
                    {
                        if (_tileSource != null)
                        {
                            byte[] data = _tileSource.GetTile(tileInfo);
                            tile = new Tile<T> { Data = data, Info = tileInfo };
                        }
                    }
                    catch (Exception ex) //This may seem a bit weird. We catch the exception to pass it as an argument. This is because we are on a worker thread here, we cannot just let it fall through. 
                    {
                        error = ex;
                    }

                    lock (_tilesInProgress)
                    {
                        if (_tilesInProgress.Contains(tileInfo.Index))
                            _tilesInProgress.Remove(tileInfo.Index);
                    }

                    _waitHandle.Set();

                    if (DataChanged != null && !_isAborted)
                        DataChanged(this, new DataChangedEventArgs<T>(error, false, tile));

                });
        }

        /// <summary>
        /// Keeps track of retries per tile. This class doesn't do much interesting work
        /// but makes the rest of the code a bit easier to read.
        /// </summary>
        class Retries
        {
            private readonly IDictionary<TileIndex, int> _retries = new Dictionary<TileIndex, int>();
            private const int MaxRetries = 0;

            public bool ReachedMax(TileIndex index)
            {
                int retryCount = (!_retries.Keys.Contains(index)) ? 0 : _retries[index];
                return retryCount > MaxRetries;
            }

            public void PlusOne(TileIndex index)
            {
                if (!_retries.Keys.Contains(index)) _retries.Add(index, 0);
                else _retries[index]++;
            }

            public void Clear()
            {
                _retries.Clear();
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