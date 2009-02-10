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
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Tiling;

namespace BruTileDemo
{
  class Graphics
  {
    Canvas canvas;
 
    public Graphics(Canvas canvas)
    {
      this.canvas = canvas;
      canvas.SnapsToDevicePixels = true;
    }


    internal void Render(ITileSchema schema, Transform transform, MemoryCache<Image> cache)
    {
      Begin();
      int level = Tile.GetNearestLevel(schema.Resolutions, transform.Resolution);
      DrawRecursive(schema, transform, cache, Util.ToExtent(transform.Extent), level);
      End();
    }

    private void Begin()
    {
      canvas.BeginInit();
      Clear();
    }

    private void Clear()
    {
      foreach (UIElement element in this.canvas.Children)
      {
        element.Visibility = Visibility.Collapsed;
      }
    }

    private void End()
    {
      CleanUp();
      canvas.EndInit();
    }

    private void CleanUp()
    {
      for (int i = canvas.Children.Count - 1; i >= 0; i--)
      {
        UIElement element = this.canvas.Children[i];
        if ((element is Image) && (element.Visibility == Visibility.Collapsed))
          canvas.Children.Remove(element);
      }
    }

    private void DrawRecursive(ITileSchema schema, Transform transform, MemoryCache<Image> cache, Extent extent, int level)
    {
      IList<TileInfo> tiles = Tile.GetTiles(schema, extent, level);

      foreach (TileInfo tile in tiles)
      {
        Image image = cache.Find(tile.Key);
        if (image == null)
        {
          if (level > 0) DrawRecursive(schema, transform, cache, tile.Extent.Intersect(extent), level - 1);
        }
        else
        {
          Rect dest = WorldToMap(tile.Extent, transform);
          this.DrawImage(image, ref dest, tile);

          if (image.Opacity < 1)
          {
            if (level > 0) DrawRecursive(schema, transform, cache, tile.Extent.Intersect(extent), level - 1);
          }
        }
      }
    }

    private void DrawImage(Image image, ref Rect dest, TileInfo tile)
    {
      if (!canvas.Children.Contains(image))
      {
        canvas.Children.Add(image);
        Canvas.SetZIndex(image, tile.Key.Level);
      }

      if (image.Opacity == 0) AnimateOpacity(image);

      Canvas.SetLeft(image, dest.X);
      Canvas.SetTop(image, dest.Y);
      image.Width = dest.Width;
      image.Height = dest.Height;
      image.Visibility = Visibility.Visible;
    }

    private void AnimateOpacity(Image image)
    {
      DoubleAnimation animation = new DoubleAnimation();
      animation.From = 0;
      animation.To = 1;
      animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 600));
      image.BeginAnimation(Image.OpacityProperty, animation);
    }

    private static Rect WorldToMap(Extent extent, Transform transform)
    {
      Point min = transform.WorldToMap(extent.MinX, extent.MinY);
      Point max = transform.WorldToMap(extent.MaxX, extent.MaxY);
      return new Rect(min, max);
    }
  }
}
