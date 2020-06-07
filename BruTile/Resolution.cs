// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile
{
    public struct Resolution
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

        public int Level { get; private set; }
        public double UnitsPerPixel { get; private set; }
        public double ScaleDenominator { get; private set; }
        public double Top { get; private set; }
        public double Left { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public long MatrixWidth { get; private set; }
        public long MatrixHeight { get; private set; }
    }
}