﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace BruTile
{
    public interface ITileSchema
    {
        string Name { get; }
        string Srs { get; }
        Extent Extent { get; }
        int GetTileWidth(string levelId);
        int GetTileHeight(string levelId);
        double GetOriginX(string levelId);
        double GetOriginY(string levelId);
        int GetMatrixWidth(string levelId);
        int GetMatrixHeight(string levelId);
        IDictionary<string, Resolution> Resolutions { get; }
        string Format { get; }
        AxisDirection Axis { get; }
        IEnumerable<TileInfo> GetTilesInView(Extent extent, string levelId);
        IEnumerable<TileInfo> GetTilesInView(Extent extent, double resolution);
        Extent GetExtentOfTilesInView(Extent extent, string levelId);
        int GetMatrixFirstCol(string levelId);
        int GetMatrixFirstRow(string levelId);
    }
}