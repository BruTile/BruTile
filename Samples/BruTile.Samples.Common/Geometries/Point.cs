// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Samples.Common.Geometries;

// Point is in its own namespace to avoid collisions with other Point types
public struct Point(double x, double y)
{
    public double X = x;
    public double Y = y;
}
