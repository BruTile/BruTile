// Copyright 2007 - Paul den Dulk (Geodan)
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

[assembly: CLSCompliant(true)]

namespace Tiling
{
  public enum AxisDirection
  {
    //Direction is relative to the coordinate system in which the map is presented.
    Normal,
    InvertedY
    //InvertedX and InvertedXY do not exist yet, and may never.
  }

  static public class Tile
  {
    static IAxis axisNormal = new AxisNormal();
    static IAxis axisInvertedY = new AxisInvertedY();
        
    /// <summary>
    /// Returns a List of TileInfo that cover the provided extent. 
    /// </summary>
    public static IList<TileInfo> GetTiles(ITileSchema schema, 
      Extent extent, double resolution)
    {
      int level = GetNearestLevel(schema.Resolutions, resolution);
      return GetTiles(schema, extent, level);
    }

    public static IList<TileInfo> GetTiles(ITileSchema schema, Extent extent, int level)
    {
      IList<TileInfo> tiles = new List<TileInfo>();
      IAxis tileAxis = GetAxisTransform(schema.Axis);
      TileRange range = tileAxis.WorldToTile(extent, level, schema);
      tiles.Clear();

      for (int x = range.FirstCol; x < range.LastCol; x++)
      {
        for (int y = range.FirstRow; y < range.LastRow; y++)
        {
          TileInfo tile = new TileInfo();
          tile.Extent = tileAxis.TileToWorld(new TileRange(x, y), level, schema);
          tile.Key = new TileKey(x, y, level);

          if (WithinSchemaExtent(schema.Extent, tile.Extent))
          {
            tiles.Add(tile);
          }
        }
      }
      return tiles;
    }

    private static bool WithinSchemaExtent(Extent schemaExtent, Extent tileExtent)
    {
      if (!tileExtent.Intersects(schemaExtent)) return false;
      //We do not accept all tiles that intersect. We reject tiles that have five
      //percent or less overlap with the schema Extent. It turns out that in practice
      //that many tiles with a small overlap with the schema extent are not on the server.
      return ((tileExtent.Intersect(schemaExtent).Area / tileExtent.Area) > 0.05);
    }

    public static int GetNearestLevel(IList<double> resolutions, double resolution) //todo: should be in util?
    {
      if (resolutions.Count == 0)
      {
        throw new ArgumentException("No tile resolutions");
      }

      //smaller than smallest
      if (resolutions[resolutions.Count - 1] > resolution) return resolutions.Count - 1; 
      
      //bigger than biggest
      if (resolutions[0] < resolution) return 0;

      int result = 0;
      double resultDistance = double.MaxValue;
      for (int i = 0; i < resolutions.Count; i++)
      {
        double distance = Math.Abs(resolutions[i] - resolution);
        if (distance < resultDistance)
        {
          result = i;
          resultDistance = distance;
        }
      }
      return result;
    }
      
    private static IAxis GetAxisTransform(AxisDirection axis)
    {
      switch (axis)
      {
        case AxisDirection.Normal:
          return axisNormal;
        case AxisDirection.InvertedY:
          return axisInvertedY;
        default:
          throw new ArgumentException("could not find axis transformer");
      }
    }
  }
}