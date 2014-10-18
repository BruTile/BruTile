// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Predefined
{
    [Obsolete("Use GlobalSphericalMercator instead")]
    public class SphericalMercatorInvertedWorldSchema : SphericalMercatorWorldSchema
    {
        public SphericalMercatorInvertedWorldSchema() 
        {
            Axis = AxisDirection.OSM;
            OriginY = -OriginY; 
            Name = "WorldSphericalMercatorInverted";
        }
    }
}
