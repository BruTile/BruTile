﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile
{
    public static class TileTransform
    {
        public static TileRange WorldToTile(Extent extent, int level, ITileSchema schema)
        {
            switch (schema.Axis)
            {
                case AxisDirection.Normal:
                    return WorldToTileNormal(extent, level, schema);
                case AxisDirection.InvertedY:
                    return WorldToTileInvertedY(extent, level, schema);
                default:
                    throw new Exception("Axis type was not found");
            }
        }

        public static Extent TileToWorld(TileRange range, int level, ITileSchema schema)
        {
            switch (schema.Axis)
            {
                case AxisDirection.Normal:
                    return TileToWorldNormal(range, level, schema);
                case AxisDirection.InvertedY:
                    return TileToWorldInvertedY(range, level, schema);
                default:
                    throw new Exception("Axis type was not found");
            }
        }

        public static TileRange WorldToTileNormal(Extent extent, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.Width;
            var firstCol = (int)Math.Floor((extent.MinX - schema.OriginX) / tileWorldUnits);
            var firstRow = (int)Math.Floor((extent.MinY - schema.OriginY) / tileWorldUnits);
            var lastCol = (int)Math.Ceiling((extent.MaxX - schema.OriginX) / tileWorldUnits);
            var lastRow = (int)Math.Ceiling((extent.MaxY - schema.OriginY) / tileWorldUnits);
            return new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
        }

        private static Extent TileToWorldNormal(TileRange range, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.Width;
            var minX = range.FirstCol * tileWorldUnits + schema.OriginX;
            var minY = range.FirstRow * tileWorldUnits + schema.OriginY;
            var maxX = (range.FirstCol + range.ColCount) * tileWorldUnits + schema.OriginX;
            var maxY = (range.FirstRow + range.RowCount) * tileWorldUnits + schema.OriginY;
            return new Extent(minX, minY, maxX, maxY);
        }

        private static TileRange WorldToTileInvertedY(Extent extent, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.Width;
            var firstCol = (int)Math.Floor((extent.MinX - schema.OriginX) / tileWorldUnits);
            var firstRow = (int)Math.Floor((-extent.MaxY + schema.OriginY) / tileWorldUnits);
            var lastCol = (int)Math.Ceiling((extent.MaxX - schema.OriginX) / tileWorldUnits);
            var lastRow = (int)Math.Ceiling((-extent.MinY + schema.OriginY) / tileWorldUnits);
            return new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
        }

        private static Extent TileToWorldInvertedY(TileRange range, int level, ITileSchema schema)
        {
            var resolution = schema.Resolutions[level];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.Width;
            var minX = range.FirstCol * tileWorldUnits + schema.OriginX;
            var minY = -(range.FirstRow + range.RowCount) * tileWorldUnits + schema.OriginY;
            var maxX = (range.FirstCol + range.ColCount) * tileWorldUnits + schema.OriginX;
            var maxY = -(range.FirstRow) * tileWorldUnits + schema.OriginY;
            return new Extent(minX, minY, maxX, maxY);
        }
    }
}
