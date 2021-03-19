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

            var tileWidthWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(level);
            var tileHeightWorldUnits = resolution.UnitsPerPixel * schema.GetTileHeight(level);
            var firstCol = (int)Math.Floor((extent.MinX - schema.GetOriginX(level)) / tileWidthWorldUnits + Tolerance);
            var firstRow = (int)Math.Floor((extent.MinY - schema.GetOriginY(level)) / tileHeightWorldUnits + Tolerance);
            var lastCol = (int)Math.Ceiling((extent.MaxX - schema.GetOriginX(level)) / tileWidthWorldUnits - Tolerance);
            var lastRow = (int)Math.Ceiling((extent.MaxY - schema.GetOriginY(level)) / tileHeightWorldUnits - Tolerance);
            return new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
        }

        private static Extent TileToWorldNormal(TileRange range, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWidthWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(level);
            var tileHeightWorldUnits = resolution.UnitsPerPixel * schema.GetTileHeight(level);
            var minX = range.FirstCol * tileWidthWorldUnits + schema.GetOriginX(level);
            var minY = range.FirstRow * tileHeightWorldUnits + schema.GetOriginY(level);
            var maxX = (range.FirstCol + range.ColCount) * tileWidthWorldUnits + schema.GetOriginX(level);
            var maxY = (range.FirstRow + range.RowCount) * tileHeightWorldUnits + schema.GetOriginY(level);
            return new Extent(minX, minY, maxX, maxY);
        }

        private static TileRange WorldToTileInvertedY(Extent extent, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWidthWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(level);
            var tilHeightWorldUnits = resolution.UnitsPerPixel * schema.GetTileHeight(level);
            var firstCol = (int)Math.Floor((extent.MinX - schema.GetOriginX(level)) / tileWidthWorldUnits + Tolerance);
            var firstRow = (int)Math.Floor((-extent.MaxY + schema.GetOriginY(level)) / tilHeightWorldUnits + Tolerance);
            var lastCol = (int)Math.Ceiling((extent.MaxX - schema.GetOriginX(level)) / tileWidthWorldUnits - Tolerance);
            var lastRow = (int)Math.Ceiling((-extent.MinY + schema.GetOriginY(level)) / tilHeightWorldUnits - Tolerance);
            return new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
        }

        private static Extent TileToWorldInvertedY(TileRange range, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWidthWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(level);
            var tileHeightWorldUnits = resolution.UnitsPerPixel * schema.GetTileHeight(level);
            var minX = range.FirstCol * tileWidthWorldUnits + schema.GetOriginX(level);
            var minY = -(range.FirstRow + range.RowCount) * tileHeightWorldUnits + schema.GetOriginY(level);
            var maxX = (range.FirstCol + range.ColCount) * tileWidthWorldUnits + schema.GetOriginX(level);
            var maxY = -(range.FirstRow) * tileHeightWorldUnits + schema.GetOriginY(level);
            return new Extent(minX, minY, maxX, maxY);
        }
    }
}
