// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile
{
    public static class TileTransform
    {
        private const double Tolerance = 0.000000001;

        public static TileRange WorldToTile(Extent extent, int level, ITileSchema schema)
        {
            switch (schema.YAxis)
            {
                case YAxis.TMS:
                    return WorldToTileNormal(extent, level, schema);
                case YAxis.OSM:
                    return WorldToTileInvertedY(extent, level, schema);
                default:
                    throw new Exception("YAxis type was not found");
            }
        }

        public static Extent TileToWorld(TileRange range, int level, ITileSchema schema)
        {
            switch (schema.YAxis)
            {
                case YAxis.TMS:
                    return TileToWorldNormal(range, level, schema);
                case YAxis.OSM:
                    return TileToWorldInvertedY(range, level, schema);
                default:
                    throw new Exception("YAxis type was not found");
            }
        }
        
        private static TileRange WorldToTileNormal(Extent extent, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];

            var tileWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(level);
            var firstCol = (int)Math.Floor((extent.MinX - schema.GetOriginX(level)) / tileWorldUnits + Tolerance);
            var firstRow = (int)Math.Floor((extent.MinY - schema.GetOriginY(level)) / tileWorldUnits + Tolerance);
            var lastCol = (int)Math.Ceiling((extent.MaxX - schema.GetOriginX(level)) / tileWorldUnits - Tolerance);
            var lastRow = (int)Math.Ceiling((extent.MaxY - schema.GetOriginY(level)) / tileWorldUnits - Tolerance);
            return new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
        }

        private static Extent TileToWorldNormal(TileRange range, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(level);
            var minX = range.FirstCol * tileWorldUnits + schema.GetOriginX(level);
            var minY = range.FirstRow * tileWorldUnits + schema.GetOriginY(level);
            var maxX = (range.FirstCol + range.ColCount) * tileWorldUnits + schema.GetOriginX(level);
            var maxY = (range.FirstRow + range.RowCount) * tileWorldUnits + schema.GetOriginY(level);
            return new Extent(minX, minY, maxX, maxY);
        }

        private static TileRange WorldToTileInvertedY(Extent extent, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(level);
            var firstCol = (int)Math.Floor((extent.MinX - schema.GetOriginX(level)) / tileWorldUnits + Tolerance);
            var firstRow = (int)Math.Floor((-extent.MaxY + schema.GetOriginY(level)) / tileWorldUnits + Tolerance);
            var lastCol = (int)Math.Ceiling((extent.MaxX - schema.GetOriginX(level)) / tileWorldUnits - Tolerance);
            var lastRow = (int)Math.Ceiling((-extent.MinY + schema.GetOriginY(level)) / tileWorldUnits - Tolerance);
            return new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
        }

        private static Extent TileToWorldInvertedY(TileRange range, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(level);
            var minX = range.FirstCol * tileWorldUnits + schema.GetOriginX(level);
            var minY = -(range.FirstRow + range.RowCount) * tileWorldUnits + schema.GetOriginY(level);
            var maxX = (range.FirstCol + range.ColCount) * tileWorldUnits + schema.GetOriginX(level);
            var maxY = -(range.FirstRow) * tileWorldUnits + schema.GetOriginY(level);
            return new Extent(minX, minY, maxX, maxY);
        }
    }
}
