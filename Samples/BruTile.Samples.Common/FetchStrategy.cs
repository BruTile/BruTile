using BruTile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BruTile.Samples.Common
{
    interface IFetchStrategy
    {
        IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, int level);
    }

    class FetchStrategy : IFetchStrategy
    {
        HashSet<int> preFetchLayers;
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

        public IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, int level)
        {
            // prefetching is not working property at the moment.
            //!!!if (preFetchLayers == null) preFetchLayers = GetPreFetchLevels(0, schema.Resolutions.Count - 1);

            IList<TileInfo> infos = new List<TileInfo>();
            // Iterating through all levels from current to zero. If lower levels are
            // not availeble the renderer can fall back on higher level tiles. 
            while (level >= 0)
            {
                //!!!if (!preFetchLayers.Contains(level)) continue;
                var infosOfLevel = schema.GetTilesInView(extent, level);
                infosOfLevel = SortByPriority(infosOfLevel, extent.CenterX, extent.CenterY);

                foreach (TileInfo info in infosOfLevel)
                {
                    if ((info.Index.Row >= 0) && (info.Index.Col >= 0)) infos.Add(info);
                }
                level--;
            }

            return infos;
        }

        private static IEnumerable<TileInfo> SortByPriority(IEnumerable<TileInfo> tiles, double centerX, double centerY)
        {
            return tiles.OrderBy((t) => Distance(centerX, centerY, t.Extent.CenterX, t.Extent.CenterY));
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2.0) + Math.Pow(y1 - y2, 2.0));
        }
    }
}