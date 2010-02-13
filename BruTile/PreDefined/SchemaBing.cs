using System;
using System.Collections.Generic;
using System.Text;

namespace BruTile.PreDefined
{
    public class SchemaBing : SchemaWorldSphericalMercatorInverted
    {
        public SchemaBing() : base()
        {
            this.Format = "jpg";
            this.Name = "BingMaps";
            this.Resolutions.RemoveAt(0); //Bing does not have the single tile top level.
        }
    }
}
