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

using System.Windows;

namespace BruTileDemo
{
  class Transform
  {
    double resolution; //number of worldunits (meters or degrees) per screen unit (usually pixel, could be inch)
    Point center;
    double width;
    double height;
    Rect extent;

    public double Resolution
    {
      get 
      { 
        return resolution; 
      }
      set 
      { 
        resolution = value;
        extent = UpdateExtent(resolution, center, width, height);
      }
    }

    public Point Center
    {
      set 
      { 
        center = value;
        extent = UpdateExtent(resolution, center, width, height);
      }
    }

    public double Width
    {
      set 
      { 
        width = value;
        extent = UpdateExtent(resolution, center, width, height);
      }
      get { return width; }
    }

    public double Height
    {
      set 
      { 
        height = value;
        extent = UpdateExtent(resolution, center, width, height);
      }
      get { return height; } 
    }
    
    public void Pan(Point currentMap, Point previousMap)
    {
      Point current = this.MapToWorld(currentMap.X, currentMap.Y);
      Point previous = this.MapToWorld(previousMap.X, previousMap.Y);
      Vector diff = Point.Subtract(previous, current);
      this.Center = Point.Add(center, diff);
    }

    public Rect Extent
    {
      get { return extent; }
    }

    private static Rect UpdateExtent(double resolution, Point center, double width, double height)
    {
      double spanX = width * resolution;
      double spanY = height * resolution;
      return new Rect(center.X - spanX * 0.5, center.Y - spanY * 0.5, spanX, spanY);
    }

    public Point WorldToMap(double x, double y)
    {
      return new Point((x - extent.X) / resolution, ((extent.Y + extent.Height) - y) / resolution);
    }

    public Point MapToWorld(double x, double y)
    {
      return new Point(extent.X + x * resolution, (extent.Y + extent.Height) - y * resolution);
    }
  }
}
