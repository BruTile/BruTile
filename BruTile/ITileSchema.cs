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
        double OriginX { get; }
        double OriginY { get; }
        IDictionary<int, Resolution> Resolutions { get; }
        int Width { get; }
        int Height { get; }
        string Format { get; }
        AxisDirection Axis { get; }
        IEnumerable<TileInfo> GetTilesInView(Extent extent, int level);
        IEnumerable<TileInfo> GetTilesInView(Extent extent, double resolution);
        Extent GetExtentOfTilesInView(Extent extent, int level);
    }
}