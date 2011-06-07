#region License

// Copyright 2008 - Paul den Dulk (Geodan)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

using System;
using System.Globalization;

namespace BruTile
{
    public struct Extent
    {
        public double MinX { get; private set; }
        public double MinY { get; private set; }
        public double MaxX { get; private set; }
        public double MaxY { get; private set; }

        public double CenterX
        {
            get { return (MinX + MaxX)/2.0; }
        }

        public double CenterY
        {
            get { return (MinY + MaxY)/2.0; }
        }

        public double Width
        {
            get { return MaxX - MinX; }
        }

        public double Height
        {
            get { return MaxY - MinY; }
        }

        public double Area
        {
            get { return Width*Height; }
        }

        public Extent Intersect(Extent other) //TODO: check out how to name this method.
        {
            return new Extent(
                Math.Max(MinX, other.MinX),
                Math.Max(MinY, other.MinY),
                Math.Min(MaxX, other.MaxX),
                Math.Min(MaxY, other.MaxY));
        }

        public Extent(double minX, double minY, double maxX, double maxY) : this()
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;

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
            return Equals((Extent) obj);
        }

        public bool Equals(Extent extent)
        {
            if (MinX != extent.MinX)
            {
                return false;
            }

            if (MinY != extent.MinY)
            {
                return false;
            }

            if (MaxX != extent.MaxX)
            {
                return false;
            }

            if (MaxY != extent.MaxY)
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
            return Equals(extent1, extent2);
        }

        public static bool operator !=(Extent extent1, Extent extent2)
        {
            return !Equals(extent1, extent2);
        }
    }
}