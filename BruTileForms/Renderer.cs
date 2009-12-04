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
using System.Drawing;
using BruTile;
using BruTileMap;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace BruTileForms
{
  public static class Renderer
  {
    public static void Render(Graphics graphics, ITileSchema schema,
      MapTransform transform, MemoryCache<Bitmap> cache)
    {
      int level = Tile.GetNearestLevel(schema.Resolutions, transform.Resolution);
      DrawRecursive(graphics, schema, transform, cache, Tile.GetTileExtent(schema, transform.Extent, level), level);
    }

    private static void DrawRecursive(Graphics graphics, ITileSchema schema, MapTransform transform, MemoryCache<Bitmap> cache, Extent extent, int level)
    {
      IList<TileInfo> tiles = Tile.GetTilesInView(schema, extent, level);

      foreach (TileInfo tile in tiles)
      {
        Bitmap image = cache.Find(tile.Key);
        if (image == null)
        {
          if (level > 0) DrawRecursive(graphics, schema, transform, cache, tile.Extent.Intersect(extent), level - 1);
        }
        else
        {
          RectangleF dest = WorldToMap(tile.Extent, transform);
          dest = RoundToPixel(dest);
          RectangleF clip = WorldToMap(extent, transform);
          clip = RoundToPixel(clip);

          if (!Contains(clip, dest))
          {
            clip = Intersect(clip, dest);
            if (clip.IsEmpty) continue;
            DrawImage(graphics, image, dest, tile, clip);
          }
          else
          {
            //Not using a clip at all sometimes performs better than using screenwide clip.
            DrawImage(graphics, image, dest, tile);
          }
        }
      }
    }

    private static RectangleF RoundToPixel(RectangleF dest)
    {
      // To get seamless aligning you need to round the locations
      // not the width and height
      dest = new RectangleF(
          (float)Math.Round(dest.Left),
          (float)Math.Round(dest.Top),
          (float)(Math.Round(dest.Right) - Math.Round(dest.Left)),
          (float)(Math.Round(dest.Bottom) - Math.Round(dest.Top)));
      return dest;
    }

    private static bool Contains(RectangleF clip, RectangleF dest)
    {
      return ((clip.Left <= dest.Left) && (clip.Top <= dest.Top) &&
        (clip.Right >= dest.Right) && (clip.Bottom >= dest.Bottom));
    }

    private static RectangleF Intersect(RectangleF clip, RectangleF dest)
    {
      float left = Math.Max(clip.Left, dest.Left);
      float top = Math.Max(clip.Top, dest.Top);
      float right = Math.Min(clip.Right, dest.Right);
      float bottom = Math.Min(clip.Bottom, dest.Bottom);
      return new RectangleF(left, top, right - left, bottom - top);
    }

    private static void DrawImage(Graphics graphics, Bitmap bitmap, RectangleF dest, TileInfo tile)
    {
      Rectangle rectDest = new Rectangle((int)dest.X, (int)dest.Y, (int)dest.Width, (int)dest.Height);
      Rectangle rectSrc = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
#if PocketPC
      graphics.DrawImage(bitmap, rectDest, rectSrc, GraphicsUnit.Pixel);
#else
      ImageAttributes imageAttributes = new ImageAttributes();
      imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
      graphics.DrawImage(bitmap, rectDest, rectSrc.X, rectSrc.Y, rectSrc.Width, rectSrc.Height, GraphicsUnit.Pixel, imageAttributes);
#endif
    }

    private static void DrawImage(Graphics graphics, Bitmap bitmap, RectangleF dest, TileInfo tile, RectangleF clip)
    {
      //todo: clipping like this is very inefficient. Find a faster way (use a smaller srcRect instead of a clip).
      graphics.Clip = new Region(new Rectangle((int)clip.X, (int)clip.Y, (int)clip.Width, (int)clip.Height));
      DrawImage(graphics, bitmap, dest, tile);
      graphics.ResetClip();
    }

    private static Rectangle Inflate(RectangleF clip, int increase)
    {
      return new Rectangle((int)clip.Left - increase, (int)clip.Top - increase,
        (int)clip.Width + increase * 2, (int)clip.Bottom + increase * 2);
    }

    private static RectangleF WorldToMap(Extent extent, MapTransform transform)
    {
      BTPoint min = transform.WorldToMap(extent.MinX, extent.MinY);
      BTPoint max = transform.WorldToMap(extent.MaxX, extent.MaxY);
      return new RectangleF(min.X, max.Y, max.X - min.X, min.Y - max.Y);
    }

  }
}
