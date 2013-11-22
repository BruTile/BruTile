// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;

namespace BruTile.Predefined
{
    public class SphericalMercatorWorldSchema : TileSchema
    {
        public SphericalMercatorWorldSchema()
        {
            var resolutions = new[] { 
                156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 
                9783.939618750, 4891.969809375, 2445.984904688, 1222.992452344, 
                611.496226172, 305.748113086, 152.874056543, 76.437028271, 
                38.218514136, 19.109257068, 9.554628534, 4.777314267, 
                2.388657133, 1.194328567, 0.597164283};

            var count = 0;
            foreach (var resolution in resolutions)
            {
                var levelId = count.ToString(CultureInfo.InvariantCulture);
                Resolutions[levelId] = new Resolution { Id = levelId, UnitsPerPixel = resolution };
                count++;
            }
            Height = 256;
            Width = 256;
            Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
            OriginX = -20037508.342789;
            OriginY = -20037508.342789;
            Name = "WorldSphericalMercator";
            Format = "png";
            Axis = AxisDirection.Normal;
            Srs = "EPSG:3857";
        }
    }
}