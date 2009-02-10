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

namespace Tiling
{
  public struct Extent
  {
    double minX;
    double minY;
    double maxX;
    double maxY;

    public double MinX
    {
      get { return minX; }
    }

    public double MinY
    {
      get { return minY; }
    }

    public double MaxX
    {
      get { return maxX; }
    }

    public double MaxY
    {
      get { return maxY; }
    }

    public double CenterX
    {
      get { return (minX + maxX) / 2; }
    }

    public double CenterY
    {
      get { return (minY + maxY) / 2; }
    }

    private double Width
    {
      get { return maxX - minX; }
    }

    private double Height
    {
      get { return maxY - minY; }
    }

    public double Area
    {
      get { return Width * Height; }
    }

    public Extent Intersect(Extent other) //TODO: check out how to name this method.
    {
      return new Extent(
        Math.Max(this.minX, other.minX),
        Math.Max(this.minY, other.minY),
        Math.Min(this.maxX, other.maxX),
        Math.Min(this.maxY, other.maxY));
    }

    public Extent(double minX, double minY, double maxX, double maxY)
    {
      this.minX = minX;
      this.minY = minY;
      this.maxX = maxX;
      this.maxY = maxY;
      if (minX > maxX || minY > maxY)
        throw new ArgumentException("min should be smaller than max");
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
        "{0},{1},{2},{3}", MinX, MinY, MaxX, MaxY);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is Extent)) return false;;
      return Equals((Extent)obj);
    }

    public bool Equals(Extent extent)
    {
      if (this.minX != extent.minX) return false;
      if (this.minY != extent.minY) return false;
      if (this.maxX != extent.maxX) return false;
      if (this.maxY != extent.maxY) return false;
      return true;
    }

    public override int GetHashCode()
    {
      return minX.GetHashCode() ^ minY.GetHashCode() ^ maxX.GetHashCode() ^ maxY.GetHashCode();
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
