// Copyright 2009 - Paul den Dulk (Geodan)
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
using BruTile;

namespace BruTileMap
{
  class FetchStrategy : IFetchStrategy
  {
    private Sorter sorter = new Sorter();

    public IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, int level)
    {
      IList<TileInfo> tiles = new List<TileInfo>();
      int step = 1;
      // Iterating through all levels from current to zero. If lower levels are
      // not availeble the renderer can fall back on higher level tiles. 
      while (level >= 0)
      {
        IList<TileInfo> tilesOfLevel = Tile.GetTiles(schema, extent, level);
        tilesOfLevel = PrioritizeTiles(tilesOfLevel, extent.CenterX, extent.CenterY, sorter);

        foreach (TileInfo tile in tilesOfLevel)
        {
          if ((tile.Key.Row >= 0) && (tile.Key.Col >= 0)) tiles.Add(tile);
        }
        level = level - step;
        step++;
      }

      return tiles;
    }

    /// <summary>
    /// Puts the tiles in the order in which they should be retrieved. Tiles close to the center
    /// come first.
    /// </summary>
    /// <param name="extent"></param>
    /// <param name="resolution"></param>
    /// <returns></returns>
    private static List<TileInfo> PrioritizeTiles(IList<TileInfo> inTiles, double centerX, double centerY, Sorter sorter)
    {
      List<TileInfo> tiles = new List<TileInfo>(inTiles);

      for (int i = 0; i < tiles.Count; i++)
      {
        double priority = -Distance(centerX, centerY, tiles[i].Extent.CenterX, tiles[i].Extent.CenterY);
        tiles[i].Priority = priority;
      }

      tiles.Sort(sorter);
      return tiles;
    }

    private static double Distance(double x1, double y1, double x2, double y2)
    {
      return Math.Sqrt(Math.Pow(x1 - x2, 2f) + Math.Pow(y1 - y2, 2f));
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
  }
}
