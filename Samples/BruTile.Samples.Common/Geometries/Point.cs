using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BruTile.Samples.Common.Geometries
{
    // Point is in its own namespace to avoid collisions with other Point types
    public struct Point
    {
        public Point(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public double X;
        public double Y;
    }
}
