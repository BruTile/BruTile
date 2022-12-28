// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile
{
    public readonly struct Resolution
    {
        public Resolution(int level,
            double unitsPerPixel,
            int tileWidth = 256,
            int tileHeight = 256,
            double left = 0,
            double top = 0,
            long matrixWidth = 0,
            long matrixHeight = 0,
            double scaledenominator = 0)
        {
            Level = level;
            UnitsPerPixel = unitsPerPixel;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            MatrixWidth = matrixWidth;
            MatrixHeight = matrixHeight;
            Top = top;
            Left = left;
            ScaleDenominator = scaledenominator;
        }

        public int Level { get; }
        public double UnitsPerPixel { get; }
        public double ScaleDenominator { get; }
        public double Top { get; }
        public double Left { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public long MatrixWidth { get; }
        public long MatrixHeight { get; }
    }
}
