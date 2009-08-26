using System;

namespace BruTileDemo
{
    internal static class ZoomHelper
    {
        internal static double ZoomIn(double[] resolutions, double resolution)
        {
            if (resolutions.Length == 0)
            {
                throw new ArgumentException("No tile resolutions");
            }

            //smaller than smallest
            if (resolutions[resolutions.Length - 1] > resolution) return resolutions.Length - 1;

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i] < resolution)
                    return resolutions[i];
            }
            return resolutions[resolutions.Length - 1];
        }


        internal static double ZoomOut(double[] resolutions, double resolution)
        {
            if (resolutions.Length == 0)
            {
                throw new ArgumentException("No tile resolutions");
            }

            //bigger than biggest
            if (resolutions[0] < resolution) return 0;

            for (int i = resolutions.Length - 1; i >= 0; i--)
            {
                if (resolutions[i] > resolution)
                    return resolutions[i];
            }
            return resolutions[0];
        }


    }
}
