﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;

namespace BruTile
{
    public struct Resolution
    {
        public string Id { get; set; }
        public double UnitsPerPixel { get; set; }
        public double ScaleDenominator { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int MatrixWidth { get; set; }
        public int MatrixHeight { get; set; }
    }
}