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
using System.Windows;
using Tiling;

namespace BruTileDemo
{
  class MemoryCache<T> : DependencyObject, ITileCache<T>
  {
    #region Fields
      
    private SortedDictionary<TileKey, T> bitmaps 
      = new SortedDictionary<TileKey, T>();

    private SortedDictionary<TileKey, DateTime> touched
      = new SortedDictionary<TileKey, DateTime>();

    private object syncRoot = new object();
    private const int upperLimit = 300;
    private const int lowerLimit = 150;
    private const int counter = 0;
    private delegate void SetTileCountDelegate(int count);
    
    #endregion

    #region Dependency Properties

    public static readonly DependencyProperty TileCountProperty = 
      System.Windows.DependencyProperty.Register(
      "TileCount", typeof(int), typeof(MemoryCache<T>));

    #endregion

    #region Properties
    
    public int TileCount
    {
      get { return (int)GetValue(TileCountProperty); }
      private set { SetValue(TileCountProperty, value); }
    }

    #endregion

    #region Public Methods

    public void Add(TileKey key, T item)
    {
      lock (syncRoot)
      {
        if (bitmaps.ContainsKey(key))
        {
          bitmaps[key] = item;
          touched[key] = DateTime.Now;
        }
        else
        {
          touched.Add(key, DateTime.Now);
          bitmaps.Add(key, item);
          if (bitmaps.Count > upperLimit) CleanUp();
          SetTileCount(bitmaps.Count);
        }
      }
    }
 
    public void Remove(TileKey key)
    {
      lock (syncRoot)
      {
        if (!bitmaps.ContainsKey(key)) return; //ignore if not exists
        touched.Remove(key);
        bitmaps.Remove(key);
        SetTileCount(bitmaps.Count);
      }
    }

    public T Find(TileKey key)
    {
      lock (syncRoot)
      {
        if (!bitmaps.ContainsKey(key))
        {
          return default(T);
        }
        else
        {
          touched[key] = DateTime.Now;
          return bitmaps[key];
        }
      }
    }

    #endregion

    #region Private Methods

    private void SetTileCount(int count)
    {
      //TileCount is a DependencyProperty which can only be set on the main thread.
      if (!this.Dispatcher.CheckAccess())
      {
        this.Dispatcher.BeginInvoke(new SetTileCountDelegate(
        SetTileCount), new object[] { count });
      }
      else
      {
        TileCount = bitmaps.Count;
      }
    }

    private void CleanUp()
    {
      lock (syncRoot)
      {
        //Purpose: Remove the older tiles so that the newest x tiles are left.
        TouchPermaCache(touched);
        DateTime cutoff = GetCutOff(touched, lowerLimit);
        List<TileKey> oldItems = GetOldItems(touched, ref cutoff);
        foreach (TileKey key in oldItems)
        {
          Remove(key);
        }
      }
    }

    private void TouchPermaCache(SortedDictionary<TileKey, DateTime> touched)
    {
      List<TileKey> keys = new List<TileKey>();
      //This is a temporary solution to preserve level zero tiles in memory.
      foreach (TileKey key in touched.Keys) if (key.Level == 0) keys.Add(key); 
      foreach (TileKey key in keys) touched[key] = DateTime.Now;
    }
    
    private static DateTime GetCutOff(SortedDictionary<TileKey, DateTime> touched,
      int lowerLimit)
    {
      List<DateTime> times = new List<DateTime>();
      foreach (DateTime time in touched.Values)
      {
        times.Add(time);
      }
      times.Sort();
      return times[times.Count - lowerLimit];
    }

    private static List<TileKey> GetOldItems(SortedDictionary<TileKey, DateTime> touched, 
      ref DateTime cutoff)
    {
      List<TileKey> oldItems = new List<TileKey>();
      foreach (TileKey key in touched.Keys)
      {
        if (touched[key] < cutoff)
        {
          oldItems.Add(key);
        }
      }
      return oldItems;
    }

    #endregion
  }
}
