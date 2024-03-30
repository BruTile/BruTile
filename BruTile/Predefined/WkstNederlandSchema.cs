// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Predefined;

public class WkstNederlandSchema : TileSchema
{
    // Well known scale set: urn:ogc:def:wkss:OGC:1.0:NLDEPSG28992Scale,
    // See: http://www.geonovum.nl/sites/default/files/Nederlandse_richtlijn_tiling_-_versie_1.0.pdf
    private const int TileSize = 256;
    private readonly double _originX = -285401.920;
    private readonly double _originY = 22598.080;

    public WkstNederlandSchema()
    {
        var unitsPerPixelArray = new[] {
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
        foreach (var unitsPerPixel in unitsPerPixelArray)
        {
            var level = count;
            Resolutions[level] = new Resolution
            (
                level,
                unitsPerPixel,
                TileSize,
                TileSize,
                _originX,
                _originY
            );
            count++;
        }

        Extent = new Extent(-285401.920, 22598.080, 595401.92, 903401.920);
        OriginX = _originX;
        OriginY = _originY;
        Name = "urn:ogc:def:wkss:OGC:1.0:NLDEPSG28992Scale";
        Format = "png";
        YAxis = YAxis.TMS;
        Srs = "EPSG:28992";
    }
}
