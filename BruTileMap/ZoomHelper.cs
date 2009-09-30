using System;
using System.Collections.Generic;

namespace BruTileMap
{
    public static class ZoomHelper
    {
        public static double ZoomIn(IList<double> resolutions, double resolution)
        {
          if (resolutions.Count == 0)
            {
                throw new ArgumentException("No tile resolutions");
            }

            //smaller than smallest
            if (resolutions[resolutions.Count - 1] > resolution) return resolutions.Count - 1;

            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i] < resolution)
                    return resolutions[i];
            }
            return resolutions[resolutions.Count - 1];
        }


        public static double ZoomOut(IList<double> resolutions, double resolution)
        {
            if (resolutions.Count == 0)
            {
                throw new ArgumentException("No tile resolutions");
            }

            //bigger than biggest
            if (resolutions[0] < resolution) return 0;

            for (int i = resolutions.Count - 1; i >= 0; i--)
            {
                if (resolutions[i] > resolution)
                    return resolutions[i];
            }
            return resolutions[0];
        }


    }
}
