// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace BruTile.Samples.Common
{
    public static class ZoomHelper
    {
        public static double ZoomIn(IList<double> unitsPerPixelList, double unitsPerPixel)
        {
            if (unitsPerPixelList.Count == 0) return unitsPerPixel / 2.0;

            //smaller than smallest
            if (unitsPerPixelList[unitsPerPixelList.Count - 1] > unitsPerPixel) return unitsPerPixelList[unitsPerPixelList.Count - 1];

            for (int i = 0; i < unitsPerPixelList.Count; i++)
            {
                if (unitsPerPixelList[i] < unitsPerPixel)
                    return unitsPerPixelList[i];
            }
            return unitsPerPixelList[unitsPerPixelList.Count - 1];
        }

        public static double ZoomOut(IList<double> unitsPerPixelList, double unitsPerPixel)
        {
            if (unitsPerPixelList.Count == 0) return unitsPerPixel * 2.0;

            //bigger than biggest
            if (unitsPerPixelList[0] < unitsPerPixel) return unitsPerPixelList[0];

            for (int i = unitsPerPixelList.Count - 1; i >= 0; i--)
            {
                if (unitsPerPixelList[i] > unitsPerPixel)
                    return unitsPerPixelList[i];
            }
            return unitsPerPixelList[0];
        }
    }
}
