// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Samples.Common.Geometries;

// Point is in its own namespace to avoid collisions with other Point types
public struct Point
{
    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X;
    public double Y;
}
