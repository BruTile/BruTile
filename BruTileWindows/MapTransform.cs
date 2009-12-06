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

using BruTile;
using System.Windows;

namespace BruTile.UI.Windows
{
    public class MapTransform : ITransform
    {
        #region Fields

        double resolution;
        Point center = new Point();
        float width;
        float height;
        Extent extent;

        #endregion

        #region Public Methods

        public double Resolution
        {
            set
            {
                resolution = value;
                UpdateExtent();
            }
            get
            {
                return resolution;
            }
        }

        public Point Center
        {
            set
            {
                center = value;
                UpdateExtent();
            }
            get
            {
                return center;
            }
        }

        public float Width
        {
            set
            {
                width = value;
                UpdateExtent();
            }
            get { return width; }
        }

        public float Height
        {
            set
            {
                height = value;
                UpdateExtent();
            }
            get { return height; }
        }

        public Extent Extent
        {
            get { return extent; }
        }

        public Point WorldToMap(double x, double y)
        {
            return new Point((x - extent.MinX) / resolution, (extent.MaxY - y) / resolution);
        }

        public Point MapToWorld(double x, double y)
        {
            return new Point((extent.MinX + x * resolution), (extent.MaxY - (y * resolution)));
        }

        #endregion

        #region Private Methods

        private void UpdateExtent()
        {
            float spanX = width * (float)resolution;
            float spanY = height * (float)resolution;
            extent = new Extent(center.X - spanX * 0.5f, center.Y - spanY * 0.5f,
              center.X + spanX * 0.5f, center.Y + spanY * 0.5f);
        }

        #endregion
    }
}
