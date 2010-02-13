using System;
using System.Collections.Generic;
using System.Text;

namespace BruTile.PreDefined
{
    public class SchemaWorldSphericalMercatorInverted : SchemaWorldSphericalMercator
    {
        public SchemaWorldSphericalMercatorInverted() : base()
        {
            this.Axis = AxisDirection.InvertedY;
            this.OriginY = -this.OriginY; 
            this.Name = "WorldShericalMercatorInverted";
        }
    }
}
