using System;
using System.Collections.Generic;
using System.Linq;

namespace BruTile.Samples.Common
{
    interface IFetchStrategy
    {
        IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, string levelId);
    }

    class FetchStrategy : IFetchStrategy
    {
        public static HashSet<int> GetPreFetchLevels(int min, int max)
        {
            var preFetchLayers = new HashSet<int>();
            int level = min;
            var step = 1;
            while (level <= max)
            {
                preFetchLayers.Add(level);
                level += step;
                step++;
            }
            return preFetchLayers;
        }

        public IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, string levelId)
        {
            IList<TileInfo> infos = new List<TileInfo>();
            // Iterating through all levels from current to zero. If lower levels are
            // not availeble the renderer can fall back on higher level tiles. 
            var unitsPerPixel = schema.Resolutions[levelId].UnitsPerPixel;
            var levels = schema.Resolutions.Where(k => unitsPerPixel <= k.Value.UnitsPerPixel).OrderByDescending(x => x.Value.UnitsPerPixel);
            
            //var levelCount = levels.Count();
            foreach (var level in levels)
            {
                var tileInfos = schema.GetTileInfos(extent, level.Key);
                tileInfos = SortByPriority(tileInfos, extent.CenterX, extent.CenterY);

                //var count = infosOfLevel.Count();
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