using System;
using System.Collections.Generic;
using System.Text;

namespace BruTile.PreDefined
{
    public class SchemaWorldSphericalMercator : TileSchema
    {
        public SchemaWorldSphericalMercator()
        {
            double[] resolutions = new double[] { 
                156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 
                9783.939618750, 4891.969809375, 2445.984904688, 1222.992452344, 
                611.496226172, 305.748113086, 152.874056543, 76.437028271, 
                38.218514136, 19.109257068, 9.554628534, 4.777314267, 
                2.388657133, 1.194328567, 0.597164283};

            foreach (double resolution in resolutions) this.Resolutions.Add(resolution);
            this.Height = 256;
            this.Width = 256;
            this.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
            this.OriginX = -20037508.342789;
            this.OriginY = -20037508.342789;
            this.Name = "WorldShericalMercator";
            this.Format = "png";
            this.Axis = AxisDirection.Normal;
            this.Srs = "EPSG:3785";
        }
    }
}