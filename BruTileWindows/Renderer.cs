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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using BruTile;
using BruTileMap;

namespace BruTileWindows
{
  public class Renderer
  {
    ITileCache<Image> images = new MemoryCache<Image>(100, 200);

    public void Render(Canvas canvas, ITileSchema schema, ITransform transform, MemoryCache<MemoryStream> cache)
    {
      CollapseAll(canvas);
      int level = Tile.GetNearestLevel(schema.Resolutions, transform.Resolution);
      DrawRecursive(canvas, schema, transform, cache, transform.Extent, level);
      RemoveCollapsed(canvas);
    }

    private static void CollapseAll(Canvas canvas)
    {
      foreach (UIElement element in canvas.Children)
      {
        element.Visibility = Visibility.Collapsed;
      }
    }

    private void DrawRecursive(Canvas canvas, ITileSchema schema, ITransform transform, MemoryCache<MemoryStream> memoryCache, Extent extent, int level)
    {
      IList<TileInfo> tiles = Tile.GetTiles(schema, extent, level);

      foreach (TileInfo tile in tiles)
      {
        MemoryStream bitmapImage = memoryCache.Find(tile.Key);
        if (bitmapImage == null)
        {
          if (level > 0) DrawRecursive(canvas, schema, transform, memoryCache, tile.Extent.Intersect(extent), level - 1);
        }
        else
        {
          Rect dest = WorldToMap(tile.Extent, transform);
          double opacity = DrawImage(canvas, bitmapImage, dest, tile);
          if ((opacity < 1) && (level > 0)) DrawRecursive(canvas, schema, transform, memoryCache, tile.Extent.Intersect(extent), level - 1);
        }
      }
    }

    private double DrawImage(Canvas canvas, MemoryStream memoryStream, Rect dest, TileInfo tile)
    {
      Image image = images.Find(tile.Key);
      if (image == null)
      {
        image = new Image();
#if SILVERLIGHT
        BitmapImage source = new BitmapImage();
        source.SetSource(memoryStream);
        image.Source = source;
#else
        memoryStream.Seek(0, SeekOrigin.Begin);
        BitmapSource source = BitmapFrame.Create(memoryStream);
        image.BeginInit();
        image.Source = source;
        image.EndInit();
#endif

        images.Add(tile.Key, image);
        Canvas.SetZIndex(image, tile.Key.Level);
        image.Opacity = 0;
        AnimateOpacity(image, 0, 1, 600);
      }

      if (!canvas.Children.Contains(image))
        canvas.Children.Add(image);

      Rect destRounded = new Rect(
        new Point(Math.Round(dest.X) - 0.5, 
        Math.Round(dest.Y) - 0.5),
        new Point(Math.Round(dest.X + dest.Width) + 0.5, 
        Math.Round(dest.Y + dest.Height) + 0.5));

      Canvas.SetLeft(image, destRounded.X);
      Canvas.SetTop(image, destRounded.Y);
      image.Width = destRounded.Width;
      image.Height = destRounded.Height;

      image.Visibility = Visibility.Visible;
      return image.Opacity;
    }

    private static void AnimateOpacity(Image target, double from, double to, int duration)
    {
      target.Opacity = 0;
      DoubleAnimation animation = new DoubleAnimation();
      animation.From = from;
      animation.To = to;
      animation.Duration = new TimeSpan(0, 0, 0, 0, duration);

      Storyboard.SetTarget(animation, target);
      Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

      Storyboard storyBoard = new Storyboard();
      storyBoard.Children.Add(animation);
      storyBoard.Begin();
    }

    private static void RemoveCollapsed(Canvas canvas)
    {
      for (int i = canvas.Children.Count - 1; i >= 0; i--)
      {
        UIElement element = canvas.Children[i];
        if ((element is Image) && (element.Visibility == Visibility.Collapsed))
        {
          Image image = element as Image;
          canvas.Children.Remove(image);
        }
      }
    }

    //TODO: remove this method:
    private static Rect WorldToMap(Extent extent, ITransform transform)
    {
      BTPoint min = transform.WorldToMap(extent.MinX, extent.MinY);
      BTPoint max = transform.WorldToMap(extent.MaxX, extent.MaxY);
      return new Rect(min.X, max.Y, max.X - min.X, min.Y - max.Y);
    }

  }
}
