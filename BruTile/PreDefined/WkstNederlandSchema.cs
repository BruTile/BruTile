// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;

namespace BruTile.Predefined
{
    public class WkstNederlandSchema : TileSchema
    {
        // Well known scale set: urn:ogc:def:wkss:OGC:1.0:NLDEPSG28992Scale
        // see: http://www.geonovum.nl/sites/default/files/Nederlandse_richtlijn_tiling_-_versie_1.0.pdf

        public WkstNederlandSchema()
        {
            var resolutions = new[] { 
                3440.64,
                1720.32,
                860.16,
                430.08,
                215.04,
                107.52,
                53.76,
                26.88,
                13.44,
                6.72,
                3.36,
                1.68,
                0.84,
                0.42,
                0.21
            };

            var count = 0;
            foreach (var resolution in resolutions)
            {
                Resolutions[count] = new Resolution {Id = count.ToString(CultureInfo.InvariantCulture), UnitsPerPixel = resolution};
                count++;
            }
            Height = 256;
            Width = 256;
            Extent = new Extent(-285401.920, 22598.080, 595401.92, 903401.920);
            OriginX = -285401.920;
            OriginY = 22598.080;
            Name = "urn:ogc:def:wkss:OGC:1.0:NLDEPSG28992Scale";
            Format = "png"; 
            Axis = AxisDirection.Normal;
            Srs = "EPSG:28992";
        }
    }
}
