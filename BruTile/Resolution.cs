// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile
{
    public struct Resolution
    {
        private readonly string _id;
        private readonly double _unitsPerPixel;
        private readonly double _scaleDenominator;
        private readonly double _top;
        private readonly double _left;
        private readonly int _tileWidth;
        private readonly int _tileHeight;
        private readonly long _matrixWidth;
        private readonly long _matrixHeight;

        public Resolution(string id, double unitsPerPixel, 
            int tileWidth = 256, int tileHeight = 256,
            double left = 0, double top = 0,
            long matrixWidth = 0, long matrixHeight = 0,
            double scaledenominator = 0)
        {
            _id = id;
            _unitsPerPixel = unitsPerPixel;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            _matrixWidth = matrixWidth;
            _matrixHeight = matrixHeight;
            _top = top;
            _left = left;
            _scaleDenominator = scaledenominator;
        }

        public string Id
        {
            get { return _id; }
        }

        public double UnitsPerPixel
        {
            get { return _unitsPerPixel; }
        }

        public double ScaleDenominator
        {
            get { return _scaleDenominator; }
        }

        public double Top
        {
            get { return _top; }
        }

        public double Left
        {
            get { return _left; }
        }

        public int TileWidth
        {
            get { return _tileWidth; }
        }

        public int TileHeight
        {
            get { return _tileHeight; }
        }

        public long MatrixWidth
        {
            get { return _matrixWidth; }
        }

        public long MatrixHeight
        {
            get { return _matrixHeight; }
        }
    }
}