// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BruTile.Cache
{

    public class MemoryCache<T>: IMemoryCache<T>, INotifyPropertyChanged, IDisposable
    {
        private readonly Dictionary<TileIndex, T> _bitmaps = new Dictionary<TileIndex, T>();
        private readonly Dictionary<TileIndex, DateTime> _touched = new Dictionary<TileIndex, DateTime>();
        private readonly object _syncRoot = new object();
        private bool _disposed;
        private readonly Func<TileIndex, bool> _keepTileInMemory;
        
        public int TileCount
        {
            get
            {
                return _bitmaps.Count;
            }
        }

        public int MinTiles { get; set; }
        public int MaxTiles { get; set; }

        public MemoryCache(int minTiles = 50, int maxTiles = 100, Func<TileIndex, bool> keepTileInMemory = null)
        {
            if (minTiles >= maxTiles) throw new ArgumentException("minTiles should be smaller than maxTiles");
            if (minTiles < 0) throw new ArgumentException("minTiles should be larger than zero");
            if (maxTiles < 0) throw new ArgumentException("maxTiles should be larger than zero");
            
            MinTiles = minTiles;
            MaxTiles = maxTiles;
            _keepTileInMemory = keepTileInMemory;
        }

        public void Add(TileIndex index, T item)
        {
            lock (_syncRoot)
            {
                if (_bitmaps.ContainsKey(index))
                {
                    _bitmaps[index] = item;
                    _touched[index] = DateTime.Now;
                }
                else
                {
                    _touched.Add(index, DateTime.Now);
                    _bitmaps.Add(index, item);
                    CleanUp();
                    OnNotifyPropertyChange("TileCount");
                }
            }
        }

        public void Remove(TileIndex index)
        {
            lock (_syncRoot)
            {
                LocalRemove(index);
            }
        }

        private void LocalRemove(TileIndex index)
        {
            if (!_bitmaps.ContainsKey(index)) return;
            var disposable = (_bitmaps[index] as IDisposable);
            if (disposable != null) disposable.Dispose();
            _touched.Remove(index);
            _bitmaps.Remove(index);
            OnNotifyPropertyChange("TileCount");
        }

        public T Find(TileIndex index)
        {
            lock (_syncRoot)
            {
                if (!_bitmaps.ContainsKey(index)) return default(T);

                _touched[index] = DateTime.Now;
                return _bitmaps[index];
            }
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                DisposeTilesIfDisposable();
                _touched.Clear();
                _bitmaps.Clear();
                OnNotifyPropertyChange("TileCount");
            }
        }

        virtual protected void CleanUp()
        {
            if (_bitmaps.Count <= MaxTiles) return;

            var numTilesToAlwaysKeep = 0;
            if (_keepTileInMemory != null)
            {
                var tilesToKeep = _touched.Keys.Where(_keepTileInMemory).ToList();
                foreach (var index in tilesToKeep) _touched[index] = DateTime.Now; // touch tiles to keep
                numTilesToAlwaysKeep = tilesToKeep.Count;
            }
            var tilesToRemove = Math.Min(_bitmaps.Count - MinTiles, _bitmaps.Count - numTilesToAlwaysKeep);

            var oldItems = _touched.OrderBy(p => p.Value).Take(tilesToRemove); 

            foreach (var oldItem in oldItems)
            {
                Remove(oldItem.Key);
            }
        }

        protected virtual void OnNotifyPropertyChange(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            if (_disposed) return;
            DisposeTilesIfDisposable();
            _touched.Clear();
            _bitmaps.Clear();
            _disposed = true;
        }

        private void DisposeTilesIfDisposable()
        {
            foreach (var index in _bitmaps.Keys)
            {
                var bitmap = (_bitmaps[index] as IDisposable);
                if (bitmap != null) bitmap.Dispose();
            }
        }

#if DEBUG
        public bool EqualSetup(MemoryCache<T> other)
        {
            if (MinTiles != other.MinTiles)
                return false;

            if (MaxTiles != other.MaxTiles)
                return false;

            System.Diagnostics.Debug.Assert(_syncRoot != null && other._syncRoot != null && _syncRoot != other._syncRoot);
            System.Diagnostics.Debug.Assert(_bitmaps != null && other._bitmaps != null && _bitmaps != other._bitmaps);
            System.Diagnostics.Debug.Assert(_touched != null && other._touched != null && _touched != other._touched);

            return true;
        }
#endif

    }
}
