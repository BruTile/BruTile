using System.Collections.Generic;

namespace BruTile.Demo
{
    public static class ZoomHelper
    {
        public static double ZoomIn(IList<Resolution> resolutions, double resolution)
        {
            if (resolutions.Count == 0) return resolution / 2.0;

            //smaller than smallest
            if (resolutions[resolutions.Count - 1].UnitsPerPixel > resolution) return resolutions[resolutions.Count - 1].UnitsPerPixel;

            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i].UnitsPerPixel < resolution)
                    return resolutions[i].UnitsPerPixel;
            }
            return resolutions[resolutions.Count - 1].UnitsPerPixel;
        }

        public static double ZoomOut(IList<Resolution> resolutions, double resolution)
        {
            if (resolutions.Count == 0) return resolution * 2.0;

            //bigger than biggest
            if (resolutions[0].UnitsPerPixel < resolution) return resolutions[0].UnitsPerPixel;

            for (int i = resolutions.Count - 1; i >= 0; i--)
            {
                if (resolutions[i].UnitsPerPixel > resolution)
                    return resolutions[i].UnitsPerPixel;
            }
            return resolutions[0].UnitsPerPixel;
        }
    }
}
