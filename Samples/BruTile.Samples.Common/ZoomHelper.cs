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
