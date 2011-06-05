// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BruTile.Cache
{

    public class MemoryCache<T> : ITileCache<T>, INotifyPropertyChanged
    {
        //for future implemenations or replacements of this class look 
        //into .net 4.0 System.Collections.Concurrent namespace.
        #region Fields

        private readonly Dictionary<TileIndex, T> _bitmaps
          = new Dictionary<TileIndex, T>();

        private readonly Dictionary<TileIndex, DateTime> _touched
          = new Dictionary<TileIndex, DateTime>();

        private readonly object _syncRoot = new object();
        private readonly int _maxTiles;
        private readonly int _minTiles;

        #endregion

        #region Properties

        public int TileCount
        {
            get
            {
                return _bitmaps.Count;
            }
        }

        #endregion 

        #region Public Methods

        public MemoryCache(int minTiles, int maxTiles)
        {
            if (minTiles >= maxTiles) throw new ArgumentException("minTiles should be smaller than maxTiles");
            if (minTiles < 0) throw new ArgumentException("minTiles should be larger than zero");
            if (maxTiles < 0) throw new ArgumentException("maxTiles should be larger than zero");

            _minTiles = minTiles;
            _maxTiles = maxTiles;
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

        #endregion

        #region Private Methods

        private void CleanUp()
        {
            lock (_syncRoot)
            {
                //Purpose: Remove the older tiles so that the newest x tiles are left.
                TouchPermaCache(_touched);
                DateTime cutoff = GetCutOff(_touched, _minTiles);
                IEnumerable<TileIndex> oldItems = GetOldItems(_touched, ref cutoff);
                foreach (TileIndex index in oldItems)
                {
                    Remove(index);
                }
            }
        }

        private static void TouchPermaCache(Dictionary<TileIndex, DateTime> touched)
        {
            var keys = new List<TileIndex>();
            //This is a temporary solution to preserve level zero tiles in memory.
            foreach (TileIndex index in touched.Keys) if (index.LevelId == 0) keys.Add(index);
            foreach (TileIndex index in keys) touched[index] = DateTime.Now;
        }

        private static DateTime GetCutOff(Dictionary<TileIndex, DateTime> touched,
          int lowerLimit)
        {
            var times = new List<DateTime>();
            foreach (DateTime time in touched.Values)
            {
                times.Add(time);
            }
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

        #endregion

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
    }
}
