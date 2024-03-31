// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile;

public readonly struct Resolution(int level,
    double unitsPerPixel,
    int tileWidth = 256,
    int tileHeight = 256,
    double left = 0,
    double top = 0,
    long matrixWidth = 0,
    long matrixHeight = 0,
    double scaledenominator = 0)
{
    public int Level { get; } = level;
    public double UnitsPerPixel { get; } = unitsPerPixel;
    public double ScaleDenominator { get; } = scaledenominator;
    public double Top { get; } = top;
    public double Left { get; } = left;
    public int TileWidth { get; } = tileWidth;
    public int TileHeight { get; } = tileHeight;
    public long MatrixWidth { get; } = matrixWidth;
    public long MatrixHeight { get; } = matrixHeight;
}
