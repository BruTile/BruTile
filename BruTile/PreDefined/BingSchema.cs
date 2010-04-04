using System;
using System.Collections.Generic;
using System.Text;

namespace BruTile.PreDefined
{
    public class BingSchema : SphericalMercatorInvertedWorldSchema
    {
        public BingSchema() : base()
        {
            this.Format = "jpg";
            this.Name = "BingMaps";
            this.Resolutions.RemoveAt(0); //Bing does not have the single tile top level.
        }
    }
}
