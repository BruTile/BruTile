﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile
{
    public static class TileTransform
    {
        public static TileRange WorldToTile(Extent extent, string levelId, ITileSchema schema)
        {
            switch (schema.Axis)
            {
                case AxisDirection.Normal:
                    return WorldToTileNormal(extent, levelId, schema);
                case AxisDirection.InvertedY:
                    return WorldToTileInvertedY(extent, levelId, schema);
                default:
                    throw new Exception("Axis type was not found");
            }
        }

        public static Extent TileToWorld(TileRange range, string levelId, ITileSchema schema)
        {
            switch (schema.Axis)
            {
                case AxisDirection.Normal:
                    return TileToWorldNormal(range, levelId, schema);
                case AxisDirection.InvertedY:
                    return TileToWorldInvertedY(range, levelId, schema);
                default:
                    throw new Exception("Axis type was not found");
            }
        }

        private static TileRange WorldToTileNormal(Extent extent, string levelId, ITileSchema schema)
        {
            var resolution = schema.Resolutions[levelId];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(levelId);
            var firstCol = (int)Math.Floor((extent.MinX - schema.GetOriginX(levelId)) / tileWorldUnits);
            var firstRow = (int)Math.Floor((extent.MinY - schema.GetOriginY(levelId)) / tileWorldUnits);
            var lastCol = (int)Math.Ceiling((extent.MaxX - schema.GetOriginX(levelId)) / tileWorldUnits);
            var lastRow = (int)Math.Ceiling((extent.MaxY - schema.GetOriginY(levelId)) / tileWorldUnits);
            return new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
        }

        private static Extent TileToWorldNormal(TileRange range, string levelId, ITileSchema schema)
        {
            var resolution = schema.Resolutions[levelId];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(levelId);
            var minX = range.FirstCol * tileWorldUnits + schema.GetOriginX(levelId);
            var minY = range.FirstRow * tileWorldUnits + schema.GetOriginY(levelId);
            var maxX = (range.FirstCol + range.ColCount) * tileWorldUnits + schema.GetOriginX(levelId);
            var maxY = (range.FirstRow + range.RowCount) * tileWorldUnits + schema.GetOriginY(levelId);
            return new Extent(minX, minY, maxX, maxY);
        }

        private static TileRange WorldToTileInvertedY(Extent extent, string levelId, ITileSchema schema)
        {
            var resolution = schema.Resolutions[levelId];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(levelId);
            var firstCol = (int)Math.Floor((extent.MinX - schema.GetOriginX(levelId)) / tileWorldUnits);
            var firstRow = (int)Math.Floor((-extent.MaxY + schema.GetOriginY(levelId)) / tileWorldUnits);
            var lastCol = (int)Math.Ceiling((extent.MaxX - schema.GetOriginX(levelId)) / tileWorldUnits);
            var lastRow = (int)Math.Ceiling((-extent.MinY + schema.GetOriginY(levelId)) / tileWorldUnits);
            return new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
        }

        private static Extent TileToWorldInvertedY(TileRange range, string levelId, ITileSchema schema)
        {
            var resolution = schema.Resolutions[levelId];
            var tileWorldUnits = resolution.UnitsPerPixel * schema.GetTileWidth(levelId);
            var minX = range.FirstCol * tileWorldUnits + schema.GetOriginX(levelId);
            var minY = -(range.FirstRow + range.RowCount) * tileWorldUnits + schema.GetOriginY(levelId);
            var maxX = (range.FirstCol + range.ColCount) * tileWorldUnits + schema.GetOriginX(levelId);
            var maxY = -(range.FirstRow) * tileWorldUnits + schema.GetOriginY(levelId);
            return new Extent(minX, minY, maxX, maxY);
        }
    }
}
