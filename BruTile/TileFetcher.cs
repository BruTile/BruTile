using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;

namespace BruTile
{
    public class TileFetcherResultEventArgs<TTile> : EventArgs where TTile : class
    {
        public TileFetcherResultEventArgs(TileFetcherResult<TTile> result)
        {
            Result = result;
        }

        public TileFetcherResult<TTile> Result { get; private set; }
    }

    public class TileFetcherResult<TTile> where TTile : class
    {
        private readonly TileInfo _info;
        private readonly TTile _result;

        public TileFetcherResult(TileInfo info, TTile result)
        {
            _info = info;
            _result = result;
        }

        public TileInfo TileInfo { get { return _info; } }
        public TTile Tile { get { return _result; } }
    }

    public class TileFetcher<TTile> where TTile : class
    {
        /// <summary>
        /// The task to get a tile
        /// </summary>
        private class GetTileTask
        {
            private readonly TileFetcher<TTile> _parent;
            private readonly TileInfo _tileInfo;
            private readonly Task  _task;
            private bool _retried = false;

            public GetTileTask(TileFetcher<TTile> parent, TileInfo tileInfo)
            {
                _parent = parent;
                _tileInfo = tileInfo;
                _task = new Task(Do);
            }

            void Do()
            {
                TileFetcherResult<TTile> result = null;
                try
                {
                    var token = _parent._cancellationTokenSource.Token;
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    var tile = _parent._cache.Find(_tileInfo.Index);
                    if (tile != null)
                    {
                        OnTileFetched(new TileFetcherResult<TTile>(_tileInfo, tile));
                        return;
                    }

                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    var rawTile = _parent._source.Provider.GetTile(_tileInfo);

                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    if (rawTile != null)
                    {
                        _parent._persistentCache.Add(_tileInfo.Index, rawTile);
                        tile = _parent._toTile(rawTile);
                        _parent._cache.Add(_tileInfo.Index, tile);
                        OnTileFetched(new TileFetcherResult<TTile>(_tileInfo, tile));
                    }
                }

                catch (WebException wex)
                {
                    if (!_retried)
                    {
                        _retried = true;
                        Do();
                    }
                }

                catch (TaskCanceledException tcex)
                {
                }

                catch (OperationCanceledException ocex)
                {
                }

                catch(Exception ex)
                {}
            }

            private void OnTileFetched(TileFetcherResult<TTile> tileFetcherResult)
            {
                var token = _parent._cancellationTokenSource.Token;
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                var h = _parent.TileFetched;
                if (h != null)
                    h.Invoke(_parent, new TileFetcherResultEventArgs<TTile>(tileFetcherResult));
            }

            public void Start()
            {
                _task.Start();
            }

            public void Dispose()
            {
                ((IDisposable) _task).Dispose();
            }
        }

        public event EventHandler<TileFetcherResultEventArgs<TTile>> TileFetched;

        private readonly List<GetTileTask> _getTileTasks = new List<GetTileTask>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource(); 

        private readonly Func<byte[], TTile> _toTile;

        private readonly object _sync = new object();
        private volatile int _numPending;

        /// <summary>
        /// Gets a value indicating that the tile fetcher is busy
        /// </summary>
        public bool IsBusy { get { return _numPending > 0; } }

        /// <summary>
        /// Method to cancel the get tiles method
        /// </summary>
        public void GetTilesCancel()
        {
            lock (_sync)
            {
                _cancellationTokenSource.Cancel();
                foreach (var getTileTask in _getTileTasks)
                    getTileTask.Dispose();
                _getTileTasks.Clear();
            }
        }

        private readonly ITileSource _source;
        private readonly MemoryCache<TTile> _cache;
        private readonly IPersistentCache<byte[]> _persistentCache;

        public IEnumerable<TileFetcherResult<TTile>> GetTiles(Extent extent, double resolution)
        {
            var level = BruTile.Utilities.GetNearestLevel(_source.Schema.Resolutions, resolution);
            var tilesToFetch = _source.Schema.GetTilesInView(extent, level);

            // Cancel the last request
            GetTilesCancel();

            foreach (var tileInfo in tilesToFetch)
            {
                // did we get this recently
                var tile = _cache.Find(tileInfo.Index);
                if (tile != null)
                {
                    // return the result
                    yield return new TileFetcherResult<TTile>(tileInfo, tile);
                    continue;
                }

                // Do we have a persistent cache?
                if (_persistentCache != null)
                {
                    // try to get the raw tile
                    var rawTile = _persistentCache.Find(tileInfo.Index);
                    if (rawTile != null)
                    {
                        // convert it to TTile
                        tile = _toTile(rawTile);
                        // add it to memory cache
                        _cache.Add(tileInfo.Index, _toTile(rawTile));
                        // return the result
                        yield return new TileFetcherResult<TTile>(tileInfo, tile);
                        continue;
                    }
                }

                // We need to start a thread
                var t = new GetTileTask(this, tileInfo);
                _getTileTasks.Add(t);
                _numPending++;
                t.Start();
            }

        }
        
    }
}