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

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Globalization;

namespace BruTile
{
    public struct Extent
    {
        private double _minX;
        private double _minY;
        private double _maxX;
        private double _maxY;

        public double MinX
        {
            get
            {
                return this._minX;
            }
        }

        public double MinY
        {
            get
            {
                return this._minY;
            }
        }

        public double MaxX
        {
            get
            {
                return this._maxX;
            }
        }

        public double MaxY
        {
            get
            {
                return this._maxY;
            }
        }

        public double CenterX
        {
            get
            {
                return (this._minX + this._maxX) / 2.0;
            }
        }

        public double CenterY
        {
            get
            {
                return (this._minY + this._maxY) / 2.0;
            }
        }

        private double Width
        {
            get
            {
                return this._maxX - this._minX;
            }
        }

        private double Height
        {
            get
            {
                return this._maxY - this._minY;
            }
        }

        public double Area
        {
            get
            {
                return this.Width * this.Height;
            }
        }

        public Extent Intersect(Extent other) //TODO: check out how to name this method.
        {
            return new Extent(
              Math.Max(this._minX, other._minX),
              Math.Max(this._minY, other._minY),
              Math.Min(this._maxX, other._maxX),
              Math.Min(this._maxY, other._maxY));
        }

        public Extent(double minX, double minY, double maxX, double maxY)
        {
            this._minX = minX;
            this._minY = minY;
            this._maxX = maxX;
            this._maxY = maxY;
            if (minX > maxX || minY > maxY)
            {
                throw new ArgumentException("min should be smaller than max");
            }
        }

        public bool Intersects(Extent box)
        {
            return !(
              box.MinX > this.MaxX ||
              box.MaxX < this.MinX ||
              box.MinY > this.MaxY ||
              box.MaxY < this.MinY);
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture,
              "{0},{1},{2},{3}", this.MinX, this.MinY, this.MaxX, this.MaxY);
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
            if (this._minX != extent._minX)
            {
                return false;
            }

            if (this._minY != extent._minY)
            {
                return false;
            }

            if (this._maxX != extent._maxX)
            {
                return false;
            }

            if (this._maxY != extent._maxY)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return _minX.GetHashCode() ^ _minY.GetHashCode() ^ _maxX.GetHashCode() ^ _maxY.GetHashCode();
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
