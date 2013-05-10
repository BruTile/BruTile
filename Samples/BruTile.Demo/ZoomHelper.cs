using System.Collections.Generic;
using System.Linq;

namespace BruTile.Demo
{
    public static class ZoomHelper
    {
        public static double ZoomIn(IDictionary<int, Resolution> resolutions, double resolution)
        {
            if (resolutions.Count == 0) return resolution / 2.0;

            //smaller than smallest
            if (resolutions.Last().Value.UnitsPerPixel > resolution) return resolutions[resolutions.Count - 1].UnitsPerPixel;

            foreach (var key in resolutions.Keys)
            {
                if (resolutions[key].UnitsPerPixel < resolution)
                    return resolutions[key].UnitsPerPixel;
            }
            return resolutions.Last().Value.UnitsPerPixel;
        }

        public static double ZoomOut(IDictionary<int, Resolution> resolutions, double resolution)
        {
            if (resolutions.Count == 0) return resolution * 2.0;

            //bigger than biggest
            if (resolutions.First().Value.UnitsPerPixel < resolution) return resolutions.First().Value.UnitsPerPixel;

            foreach (var key in resolutions.Keys.Reverse())
            {
                if (resolutions[key].UnitsPerPixel > resolution)
                    return resolutions[key].UnitsPerPixel;
            }
            return resolutions.First().Value.UnitsPerPixel;
        }
    }
}
