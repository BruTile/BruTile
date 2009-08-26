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
using System.Windows;
using BruTile;

namespace BruTileWindows
{
  static class Util
  {
    public static double Distance(double x1, double y1, double x2, double y2)
    {
      return Math.Sqrt(Math.Pow(x1 - x2, 2f) + Math.Pow(y1 - y2, 2f));
    }

    public static Extent ToExtent(Rect rect)
    {
      return new Extent(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
    }
  }
}
