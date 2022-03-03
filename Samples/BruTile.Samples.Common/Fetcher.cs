// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using BruTile.Cache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BruTile.Samples.Common
{
    public class Fetcher<T>
    {
        private readonly ITileCache<Tile<T>> _memoryCache;
        private readonly ITileSource _tileSource;
        private Extent _extent;
        private double _unitsPerPixel;
        private readonly IList<TileIndex> _tilesInProgress = new List<TileIndex>();
        private const int MaxThreads = 4;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(MaxThreads);
        private readonly IFetchStrategy _strategy = new FetchStrategy();
        private volatile CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private volatile bool _isViewChanged;
        private Retries _retries;
        private ConcurrentQueue<TileInfo> _loadingTiles;

        public event DataChangedEventHandler<T> DataChanged;

        public Fetcher(ITileSource tileSource, ITileCache<Tile<T>> memoryCache)
        {
            if (tileSource == null) throw new ArgumentException("TileProvider can not be null");
            _tileSource = tileSource;

            if (memoryCache == null) throw new ArgumentException("MemoryCache can not be null");
            _memoryCache = memoryCache;

            StartFetchLoop();
        }

        public void ViewChanged(Extent extent, double unitsPerPixel)
        {
            _extent = extent;
            _unitsPerPixel = unitsPerPixel;
            _isViewChanged = true;
        }

        private void StartFetchLoop()
        {
            Task.Run(FetchLoop, _cancellationTokenSource.Token);
        }

        public void AbortFetch()
        {
            _cancellationTokenSource.Cancel();
            while (_loadingTiles.TryDequeue(out _))
            {
            }
        }

        private async Task FetchLoop()
        {
            IEnumerable<TileInfo> tilesWanted = null;
            if (_retries == null) _retries = new Retries();

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(100).ConfigureAwait(false);

                if (_tileSource.Schema == null)
                {
                    continue;              // and go to begin of loop to wait
                }

                if (_isViewChanged || tilesWanted == null)
                {
                    var levelId = Utilities.GetNearestLevel(_tileSource.Schema.Resolutions, _unitsPerPixel);
                    tilesWanted = _strategy.GetTilesWanted(_tileSource.Schema, _extent, levelId);
                    _retries.Clear();
                    _isViewChanged = false;
                }

                var tilesMissing = GetTilesMissing(tilesWanted, _memoryCache, _retries);
                _loadingTiles = new ConcurrentQueue<TileInfo>(tilesMissing);

                await FetchTiles(_loadingTiles).ConfigureAwait(false);
            }
        }

        private static IList<TileInfo> GetTilesMissing(IEnumerable<TileInfo> tilesWanted,
            ITileCache<Tile<T>> memoryCache, Retries retries)
        {
            return tilesWanted.Where(
                info => memoryCache.Find(info.Index) == null && 
                !retries.ReachedMax(info.Index)).ToList();
        }

        private async Task FetchTiles(ConcurrentQueue<TileInfo> tilesMissing)
        {
            while(tilesMissing.TryDequeue(out var info))
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                FetchTile(info);
            }
        }

        private void FetchTile(TileInfo info)
        {
            if (_retries.ReachedMax(info.Index)) return;

            lock (_tilesInProgress)
            {
                if (_tilesInProgress.Contains(info.Index)) return;
                _tilesInProgress.Add(info.Index);
            }
            
            _retries.PlusOne(info.Index);

            // now we can go for the request.
            FetchAsync(info);
        }

        private void FetchAsync(TileInfo tileInfo)
        {
            Task.Run(
                async () =>
                {
                    Exception error = null;
                    Tile<T> tile = null;

                    try
                    {
                        if (_tileSource != null)
                        {
                            byte[] data = await _tileSource.GetTileAsync(tileInfo).ConfigureAwait(false);
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

                    _semaphore.Release();

                    if (DataChanged != null && !_cancellationTokenSource.IsCancellationRequested)
                        DataChanged(this, new DataChangedEventArgs<T>(error, false, tile));

                }, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Keeps track of retries per tile. This class doesn't do much interesting work
        /// but makes the rest of the code a bit easier to read.
        /// </summary>
        class Retries
        {
            private readonly IDictionary<TileIndex, int> _retries = new ConcurrentDictionary<TileIndex, int>();
            private const int MaxRetries = 0;
            private const string CrossThreadExceptionMessage = "Cross thread access not allowed on class Retries";

            public Retries()
            {
            }

            public bool ReachedMax(TileIndex index)
            {
                var retryCount = (!_retries.Keys.Contains(index)) ? 0 : _retries[index];
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