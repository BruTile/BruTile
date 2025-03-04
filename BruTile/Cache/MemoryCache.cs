﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BruTile.Extensions;

namespace BruTile.Cache;

public class MemoryCache<T> : IMemoryCache<T>, INotifyPropertyChanged, IDisposable
{
    private readonly Dictionary<TileIndex, T> _bitmaps = [];
    private readonly Dictionary<TileIndex, long> _touched = [];
    private readonly object _syncRoot = new();
    private bool _disposed;
    private readonly Func<TileIndex, bool>? _keepTileInMemory;

    private long _cacheVersion;

    public int TileCount
    {
        get
        {
            lock (_syncRoot)
            {
                return _bitmaps.Count;
            }
        }
    }

    public int MinTiles { get; set; }
    public int MaxTiles { get; set; }

    public MemoryCache(int minTiles = 50, int maxTiles = 100, Func<TileIndex, bool>? keepTileInMemory = null)
    {
        if (minTiles >= maxTiles) throw new ArgumentException("minTiles should be smaller than maxTiles");
        if (minTiles < 0) throw new ArgumentException("minTiles should be larger than zero");
        if (maxTiles < 0) throw new ArgumentException("maxTiles should be larger than zero");

        MinTiles = minTiles;
        MaxTiles = maxTiles;
        _keepTileInMemory = keepTileInMemory;
    }

    public long GetNextCacheVersion()
    {
        _cacheVersion++;
        return _cacheVersion;
    }

    public void Add(TileIndex index, T item)
    {
        lock (_syncRoot)
        {
            if (!_bitmaps.TryAdd(index, item))
            {
                _bitmaps[index] = item;
                _touched[index] = GetNextCacheVersion();
            }
            else
            {
                _touched.Add(index, GetNextCacheVersion());
                CleanUp();
                OnNotifyPropertyChange(nameof(TileCount));
            }
        }
    }

    public void Remove(TileIndex index)
    {
        lock (_syncRoot)
        {
            if (_bitmaps.TryGetValue(index, out var bitmap))
            {

                bitmap?.DisposeIfDisposable();
                _touched.Remove(index);
                _bitmaps.Remove(index);
                OnNotifyPropertyChange(nameof(TileCount));
            }
        }
    }

    public T? Find(TileIndex index)
    {
        lock (_syncRoot)
        {
            if (!_bitmaps.TryGetValue(index, out var value))
                return default;

            _touched[index] = GetNextCacheVersion();
            return value;
        }
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            foreach (var bitmap in _bitmaps)
                bitmap.DisposeIfDisposable();
            _touched.Clear();
            _bitmaps.Clear();
            OnNotifyPropertyChange(nameof(TileCount));
        }
    }

    private void CleanUp()
    {
        if (_bitmaps.Count <= MaxTiles) return;

        var numberOfTilesToKeepInMemory = 0;
        if (_keepTileInMemory != null)
        {
            var tilesToKeep = _touched.Keys.Where(_keepTileInMemory).ToList();
            foreach (var index in tilesToKeep) _touched[index] = GetNextCacheVersion(); // Touch tiles to keep
            numberOfTilesToKeepInMemory = tilesToKeep.Count;
        }
        var numberOfTilesToRemove = _bitmaps.Count - Math.Max(MinTiles, numberOfTilesToKeepInMemory);

        var oldItems = _touched.OrderBy(p => p.Value).Take(numberOfTilesToRemove);

        foreach (var oldItem in oldItems)
        {
            Remove(oldItem.Key);
        }
    }

    protected virtual void OnNotifyPropertyChange(string propertyName)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose()
    {
        if (_disposed) return;
        foreach (var bitmap in _bitmaps)
            bitmap.DisposeIfDisposable();
        _touched.Clear();
        _bitmaps.Clear();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
