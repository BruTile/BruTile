// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace BruTile.Samples.Common
{
    internal interface IFetchStrategy
    {
        IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, int level);
    }

    internal class FetchStrategy : IFetchStrategy
    {
        public IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, int level)
        {
            IList<TileInfo> infos = new List<TileInfo>();
            // Iterating through all levels from current to zero. If lower levels are
            // not available the renderer can fall back on higher level tiles. 
            var unitsPerPixel = schema.Resolutions[level].UnitsPerPixel;
            var resolutions = schema.Resolutions.Where(k => unitsPerPixel <= k.Value.UnitsPerPixel).OrderByDescending(x => x.Value.UnitsPerPixel);

            foreach (var resolution in resolutions)
            {
                var tileInfos = schema.GetTileInfos(extent, resolution.Key);
                tileInfos = SortByPriority(tileInfos, extent.CenterX, extent.CenterY);

                foreach (var info in tileInfos)
                {
                    if ((info.Index.Row >= 0) && (info.Index.Col >= 0)) infos.Add(info);
                }
            }

            return infos;
        }

        private static IEnumerable<TileInfo> SortByPriority(IEnumerable<TileInfo> tiles, double centerX, double centerY)
        {
            return tiles.OrderBy(t => Distance(centerX, centerY, t.Extent.CenterX, t.Extent.CenterY));
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2.0) + Math.Pow(y1 - y2, 2.0));
        }
    }
}
