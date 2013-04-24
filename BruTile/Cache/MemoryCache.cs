// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BruTile.Cache
{

    public class MemoryCache<T> : ITileCache<T>, INotifyPropertyChanged, IDisposable
    {
        //for future implemenations or replacements of this class look 
        //into .net 4.0 System.Collections.Concurrent namespace.

        private readonly Dictionary<TileIndex, T> _bitmaps
          = new Dictionary<TileIndex, T>();

        private readonly Dictionary<TileIndex, DateTime> _touched
          = new Dictionary<TileIndex, DateTime>();

        private readonly object _syncRoot = new object();
        private readonly int _maxTiles;
        private readonly int _minTiles;
        private bool _haveDisposed;
        private readonly Func<TileIndex, bool> _keepTileInMemory = null; 
        
        public int TileCount
        {
            get
            {
                return _bitmaps.Count;
            }
        }

        public MemoryCache(int minTiles = 50, int maxTiles = 100, Func<TileIndex, bool> keepTileInMemory = null)
        {
            if (minTiles >= maxTiles) throw new ArgumentException("minTiles should be smaller than maxTiles");
            if (minTiles < 0) throw new ArgumentException("minTiles should be larger than zero");
            if (maxTiles < 0) throw new ArgumentException("maxTiles should be larger than zero");
            
            _minTiles = minTiles;
            _maxTiles = maxTiles;
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
                    if (_bitmaps.Count > _maxTiles) CleanUp();
                    OnNotifyPropertyChange("TileCount");
                }
            }
        }

        public void Remove(TileIndex index)
        {
            lock (_syncRoot)
            {
                if (!_bitmaps.ContainsKey(index)) return; //ignore if not exists
                _touched.Remove(index);
                _bitmaps.Remove(index);
                OnNotifyPropertyChange("TileCount");
            }
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
                _bitmaps.Clear();
                _touched.Clear();
            }
        }

        private void CleanUp()
        {
            lock (_syncRoot)
            {
                //Purpose: Remove the older tiles so that the newest x tiles are left.
                if (_keepTileInMemory != null) TouchPermaCache(_touched, _keepTileInMemory);
                DateTime cutoff = GetCutOff(_touched, _minTiles);
                IEnumerable<TileIndex> oldItems = GetOldItems(_touched, ref cutoff);
                foreach (TileIndex index in oldItems)
                {
                    Remove(index);
                }
            }
        }

        private static void TouchPermaCache(Dictionary<TileIndex, DateTime> touched, Func<TileIndex, bool> keepTileInMemory)
        {
            //This is a temporary solution to preserve level zero tiles in memory.
            var keys = touched.Keys.Where(keepTileInMemory).ToList();
            
            foreach (TileIndex index in keys) touched[index] = DateTime.Now;
        }

        private static DateTime GetCutOff(Dictionary<TileIndex, DateTime> touched,
          int lowerLimit)
        {
            var times = touched.Values.ToList();
            times.Sort();
            return times[times.Count - lowerLimit];
        }

        private static IEnumerable<TileIndex> GetOldItems(Dictionary<TileIndex, DateTime> touched,
          ref DateTime cutoff)
        {
            var oldItems = new List<TileIndex>();
            foreach (TileIndex index in touched.Keys)
            {
                if (touched[index] < cutoff)
                {
                    oldItems.Add(index);
                }
            }
            return oldItems;
        }

        #region INotifyPropertyChanged Members

        protected virtual void OnNotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

		~MemoryCache()
        {
            if (!_haveDisposed)
                Dispose();
        }

        public void Dispose()
        {
            if (_haveDisposed)
                return;

            if (_bitmaps != null)
            {
                foreach (var kvp in _bitmaps)
                {
                    if (kvp.Value != null)
                    {
                        if (kvp.Value is IDisposable)
                        {
                            (kvp.Value as IDisposable).Dispose();
                        }
                        
                    }
                }
                _bitmaps.Clear();
            }
            _haveDisposed = true;
        }

#if DEBUG
        public bool EqualSetup(MemoryCache<T> other)
        {
            if (_minTiles != other._minTiles)
                return false;

            if (_maxTiles != other._maxTiles)
                return false;

            System.Diagnostics.Debug.Assert(_syncRoot != null && other._syncRoot != null && _syncRoot != other._syncRoot);
            System.Diagnostics.Debug.Assert(_bitmaps != null && other._bitmaps != null && _bitmaps != other._bitmaps);
            System.Diagnostics.Debug.Assert(_touched != null && other._touched != null && _touched != other._touched);

            return true;
        }
#endif

    }
}
