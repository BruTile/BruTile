// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;
using BruTile.MbTiles;

namespace BruTile.Samples.Common;

public class Fetcher<T>
{
    private readonly ITileCache<Tile<T>> _memoryCache;
    private readonly ITileSource _tileSource;
    private Extent _extent;
    private double _unitsPerPixel;
    private readonly IList<TileIndex> _tilesInProgress = [];
    private const int MaxThreads = 4;
    private readonly AutoResetEvent _waitHandle = new(false);
    private readonly IFetchStrategy _strategy = new FetchStrategy();
    private volatile bool _isAborted;
    private volatile bool _isViewChanged;
    private Retries _retries;
    private readonly HttpClient _httpClient = new();

    public event DataChangedEventHandler<T> DataChanged;

    public Fetcher(ITileSource tileSource, ITileCache<Tile<T>> memoryCache)
    {
        _tileSource = tileSource ?? throw new ArgumentException("Parameter tileSource can not be null");
        _memoryCache = memoryCache ?? throw new ArgumentException("Parameter memoryCache can not be null");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "User-Agent-For-BruTile-Samples-Project");

        StartFetchLoop();
    }

    public void ViewChanged(Extent extent, double unitsPerPixel)
    {
        _extent = extent;
        _unitsPerPixel = unitsPerPixel;
        _isViewChanged = true;
        _waitHandle.Set();
    }

    private void StartFetchLoop()
    {
        Task.Run(FetchLoop);
    }

    public void AbortFetch()
    {
        _isAborted = true;
        _waitHandle.Set(); // Activate fetch loop so it can run out of the loop
    }

    private void FetchLoop()
    {
        IEnumerable<TileInfo> tilesWanted = null;
        _retries ??= new Retries();

        while (!_isAborted)
        {
            _waitHandle.WaitOne();

            if (_tileSource.Schema == null)
            {
                _waitHandle.Reset(); // Set in wait mode
                continue;            // Then go to begin of loop to wait
            }

            if (_isViewChanged || tilesWanted == null)
            {
                var levelId = Utilities.GetNearestLevel(_tileSource.Schema.Resolutions, _unitsPerPixel);
                tilesWanted = _strategy.GetTilesWanted(_tileSource.Schema, _extent, levelId);
                _retries.Clear();
                _isViewChanged = false;
            }

            var tilesMissing = GetTilesMissing(tilesWanted, _memoryCache, _retries);

            FetchTiles(tilesMissing);

            if (tilesMissing.Count == 0) { _waitHandle.Reset(); }

            lock (_tilesInProgress)
            {
                if (_tilesInProgress.Count >= MaxThreads)
                {
                    _waitHandle.Reset();
                }
            }
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
        foreach (var info in tilesMissing)
        {
            lock (_tilesInProgress)
            {
                if (_tilesInProgress.Count >= MaxThreads) return;
            }
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

        // Now we can go for the request.
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
                    if (_tileSource is IHttpTileSource httpTileSource)
                    {
                        var data = await httpTileSource.GetTileAsync(_httpClient, tileInfo, CancellationToken.None);
                        tile = new Tile<T> { Data = data, Info = tileInfo };
                    }
                    else if (_tileSource is MbTilesTileSource mbTilesTileSource)
                    {
                        var data = await mbTilesTileSource.GetTileAsync(tileInfo, CancellationToken.None);
                        tile = new Tile<T> { Data = data, Info = tileInfo };
                    }
                    else
                    {
                        throw new Exception("TileSource not supported");
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
    private class Retries
    {
        private readonly IDictionary<TileIndex, int> _retries = new Dictionary<TileIndex, int>();
        private const int MaxRetries = 0;
        private readonly int _threadId;
        private const string CrossThreadExceptionMessage = "Cross thread access not allowed on class Retries";

        public Retries()
        {
            _threadId = Environment.CurrentManagedThreadId;
        }

        public bool ReachedMax(TileIndex index)
        {
            if (_threadId != Environment.CurrentManagedThreadId) throw new Exception(CrossThreadExceptionMessage);

            var retryCount = (!_retries.ContainsKey(index)) ? 0 : _retries[index];
            return retryCount > MaxRetries;
        }

        public void PlusOne(TileIndex index)
        {
            if (_threadId != Environment.CurrentManagedThreadId) throw new Exception(CrossThreadExceptionMessage);

            if (!_retries.ContainsKey(index)) _retries.Add(index, 0);
            else _retries[index]++;
        }

        public void Clear()
        {
            if (_threadId != Environment.CurrentManagedThreadId) throw new Exception(CrossThreadExceptionMessage);

            _retries.Clear();
        }
    }
}

public delegate void DataChangedEventHandler<T>(object sender, DataChangedEventArgs<T> e);

public class DataChangedEventArgs<T>(Exception error, bool cancelled, Tile<T> tile)
{
    public Exception Error { get; } = error;
    public bool Cancelled { get; } = cancelled;
    public Tile<T> Tile { get; } = tile;
}
