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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Tiling;

namespace BruTileDemo
{
  class TileLayer : DispatcherObject
  {
    ITileSchema schema;
    TileFetcher tileFetcher;
    public event AsyncCompletedEventHandler DataUpdated;
    MemoryCache<Image> cache = new MemoryCache<Image>();
    static System.Collections.Generic.IComparer<TileInfo> sorter = new Sorter();
    const int maxRetries = 3;
    object syncRoot = new object();

    public TileLayer(ITileProvider tileProvider, ITileSchema schema) : this(tileProvider, schema, null)
    {
    }

    public TileLayer(ITileProvider tileProvider, ITileSchema schema, ITileCache<byte[]> cache)
    {
      this.schema = schema;
      tileFetcher = new TileFetcher(tileProvider, cache);
      tileFetcher.FetchCompleted += new FetchCompletedEventHander(tileFetcher_FetchCompleted);
    }

    public MemoryCache<Image> Bitmaps
    {
      get { return cache; }
    }

    public void Draw(Graphics graphics, Transform transform)
    {
      graphics.Render(schema, transform, cache);

    }

    public void UpdateData(Rect rect, double zoom)
    {
      const int preCacheLevels = 2;
      const int priorityStep = 1000000;
      List<TileInfo> tiles;

      rect.Inflate(rect.Width / 3, rect.Height / 3);
      Extent extent = Util.ToExtent(rect);

      tileFetcher.ClearQueue();
      UpdatePermaCache();
      int resolution = Tile.GetNearestLevel(schema.Resolutions, zoom);
      int lastResolution = Math.Max(0, resolution - preCacheLevels);

      // Iterating through a number of extra resolution so we can fall back on 
      // higher levels when lower levels are not available. 
      while (resolution >= lastResolution)
      {
        tiles = GetPrioritizedTiles(extent, resolution, priorityStep * resolution);
        foreach (TileInfo tile in tiles)
        {
          if (cache.Find(tile.Key) != null) continue;
          tileFetcher.Fetch(tile, (int)tile.Priority);
        }
        resolution--;
      }
    }
   
    private List<TileInfo> GetPrioritizedTiles(Extent extent, int resolution, int basePriority)
    {
      List<TileInfo> tiles = new List<TileInfo>(Tile.GetTiles(schema, extent, resolution));

      for (int i = 0; i < tiles.Count; i++)
      {
        double priority = basePriority - Util.Distance(extent.CenterX, extent.CenterY,
          tiles[i].Extent.CenterX, tiles[i].Extent.CenterY);
        tiles[i].Priority = priority;
      }
      tiles.Sort(sorter);

      return tiles;
    }

    void tileFetcher_FetchCompleted(object sender, FetchCompletedEventArgs e)
    {
      if (!this.Dispatcher.CheckAccess())
      {
        this.Dispatcher.BeginInvoke(new FetchCompletedEventHander(tileFetcher_FetchCompleted), new object[] { sender, e });
      }
      else
      {
        if (!e.Cancelled && e.Error == null)
        {
          TileInfo tile = e.TileInfo;
          byte[] image = e.Image;
          System.Exception error = e.Error;
          if (cache.Find(tile.Key) == null)
          {
            try
            {
              BitmapSource source = BitmapFrame.Create(new MemoryStream(image));
              Image imageControl = new Image();
              imageControl.Opacity = 0;
              imageControl.Source = source;
              cache.Add(tile.Key, imageControl);
            }
            catch (FileFormatException ex)
            {
              tileFetcher.ReportBadTile(tile);
              error = ex;
            }
            catch (Exception ex)
            {
              error = ex;
            }
          }
          OnDataUpdated(new AsyncCompletedEventArgs(error, e.Cancelled, null));
        }
        else if ((e.Error is System.Net.WebException) && (e.TileInfo.Retries < maxRetries))
        {
          e.TileInfo.Retries++;
          if (cache.Find(e.TileInfo.Key) == null)
            tileFetcher.Fetch(e.TileInfo, (int)e.TileInfo.Priority);
        }
        else
        {
          OnDataUpdated(new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
        }
      }
    }

    protected void OnDataUpdated(AsyncCompletedEventArgs e)
    {
      if (DataUpdated != null)
        DataUpdated(this, e);
    }

    private static Rect WorldToMap(Extent extent, Transform transform)
    {
      Point min = transform.WorldToMap(extent.MinX, extent.MinY);
      Point max = transform.WorldToMap(extent.MaxX, extent.MaxY);
      return new Rect(min, max);
    }

    private class Sorter : IComparer<TileInfo>
    {
      public int Compare(TileInfo x, TileInfo y)
      {
        if (x.Priority > y.Priority) return -1;
        if (x.Priority < y.Priority) return 1;
        return 0;
      }
    }
    
    private void UpdatePermaCache()
    {
      //Note: The permaCache implementation is a temporary solution
      IList<TileInfo> tiles = Tile.GetTiles(schema, schema.Extent, 0);
      
      foreach (TileInfo tile in tiles)
      {
        if (cache.Find(tile.Key) != null) continue;
        tileFetcher.Fetch(tile, Int32.MaxValue);
      }
    }
  }
}
