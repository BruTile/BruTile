// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.PreDefined
{
    public class SphericalMercatorInvertedWorldSchema : SphericalMercatorWorldSchema
    {
        public SphericalMercatorInvertedWorldSchema() 
        {
            Axis = AxisDirection.InvertedY;
            OriginY = -OriginY; 
            Name = "WorldShericalMercatorInverted";
        }
    }
}
