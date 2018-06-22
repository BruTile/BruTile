// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace BruTile
{
    public struct Extent
    {
        private readonly double _minX;
        private readonly double _minY;
        private readonly double _maxX;
        private readonly double _maxY;

        public double MinX
        {
            get { return _minX; }
        }

        public double MinY
        {
            get { return _minY; }
        }

        public double MaxX
        {
            get { return _maxX; }
        }

        public double MaxY
        {
            get { return _maxY; }
        }

        public double CenterX => (MinX + MaxX) / 2.0;
        public double CenterY => (MinY + MaxY) / 2.0;
        public double Width => MaxX - MinX;
        public double Height => MaxY - MinY;
        public double Area => Width * Height;

        public Extent Intersect(Extent other)
        {
            return new Extent(
                Math.Max(MinX, other.MinX),
                Math.Max(MinY, other.MinY),
                Math.Min(MaxX, other.MaxX),
                Math.Min(MaxY, other.MaxY));
        }

        public Extent(double minX, double minY, double maxX, double maxY) : this()
        {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;

            if (minX > maxX || minY > maxY)
            {
                throw new ArgumentException("min should be smaller than max");
            }
        }

        public bool Intersects(Extent box)
        {
            return !(
                        box.MinX > MaxX ||
                        box.MaxX < MinX ||
                        box.MinY > MaxY ||
                        box.MaxY < MinY);
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture,
                                 "{0},{1},{2},{3}", MinX, MinY, MaxX, MaxY);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Extent))
            {
                return false;
            }
            return Equals((Extent)obj);
        }

        public bool Equals(Extent extent)
        {
            if (Math.Abs(_minX - extent.MinX) > double.Epsilon)
            {
                return false;
            }

            if (Math.Abs(_minY - extent.MinY) > double.Epsilon)
            {
                return false;
            }

            if (Math.Abs(_maxX - extent.MaxX) > double.Epsilon)
            {
                return false;
            }

            if (Math.Abs(_maxY - extent.MaxY) > double.Epsilon)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return MinX.GetHashCode() ^ MinY.GetHashCode() ^ MaxX.GetHashCode() ^ MaxY.GetHashCode();
        }

        public static bool operator ==(Extent extent1, Extent extent2)
        {
            return extent1.Equals(extent2);
        }

        public static bool operator !=(Extent extent1, Extent extent2)
        {
            return !extent1.Equals(extent2);
        }
    }
}