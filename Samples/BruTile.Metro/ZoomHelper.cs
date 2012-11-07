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
using BruTile;

namespace BruTile.Metro
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
