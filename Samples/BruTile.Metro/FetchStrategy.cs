using BruTile;
using System;
using System.Collections.Generic;

namespace SharpMap.Fetcher
{
    interface IFetchStrategy
    {
        IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, int level);
    }

    class FetchStrategy : IFetchStrategy
    {
        private readonly Sorter sorter = new Sorter();
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
            //line below only works properly of this instance is always called with the the resolutions. Think of something better
            if (preFetchLayers == null) preFetchLayers = GetPreFetchLevels(0, schema.Resolutions.Count - 1);

            IList<TileInfo> infos = new List<TileInfo>();
            // Iterating through all levels from current to zero. If lower levels are
            // not availeble the renderer can fall back on higher level tiles. 
            while (level >= 0)
            {
                ////////if (!preFetchLayers.Contains(level)) continue;
                var infosOfLevel = schema.GetTilesInView(extent, level);
                infosOfLevel = PrioritizeTiles(infosOfLevel, extent.CenterX, extent.CenterY, sorter);

                foreach (TileInfo info in infosOfLevel)
                {
                    if ((info.Index.Row >= 0) && (info.Index.Col >= 0)) infos.Add(info);
                }
                level--;
            }

            return infos;
        }

        private static IEnumerable<TileInfo> PrioritizeTiles(IEnumerable<TileInfo> tiles, double centerX, double centerY, Sorter sorter)
        {
            var infos = new List<TileInfo>(tiles);

            foreach (TileInfo t in infos)
            {
                double priority = -Distance(centerX, centerY, t.Extent.CenterX, t.Extent.CenterY);
                t.Priority = priority;
            }

            infos.Sort(sorter);
            return infos;
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2.0) + Math.Pow(y1 - y2, 2.0));
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