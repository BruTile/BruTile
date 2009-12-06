﻿// Copyright 2008 - Paul den Dulk (Geodan)
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
using BruTile;

namespace BruTile.UI.Windows
{
    public static class MapTransformHelper
    {
        public static void Pan(MapTransform transform, Point currentMap, Point previousMap)
        {
            Point current = transform.MapToWorld(currentMap.X, currentMap.Y);
            Point previous = transform.MapToWorld(previousMap.X, previousMap.Y);
            double diffX = previous.X - current.X;
            double diffY = previous.Y - current.Y;
            transform.Center = new Point(transform.Center.X + diffX, transform.Center.Y + diffY);
        }

        public static Rect WorldToMap(Extent extent, ITransform transform)
        {
            Point min = transform.WorldToMap(extent.MinX, extent.MinY);
            Point max = transform.WorldToMap(extent.MaxX, extent.MaxY);
            return new Rect(min.X, max.Y, max.X - min.X, min.Y - max.Y);
        }
    }
}
