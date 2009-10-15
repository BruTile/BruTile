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
using System.Windows.Media;
using BruTileMap;
using BruTile;

namespace BruTileDemo
{
    class SurfaceTransform : ITransform
    {
        double resolution; //number of worldunits (meters or degrees) per screen unit (usually pixel, could be inch)
        Point center;
        double width;
        double height;
        Extent extent;
        MatrixTransform transform = new MatrixTransform();

        public SurfaceTransform()
        {
            transform.Matrix = new Matrix();
        }

        public double Resolution
        {
            get
            {
                return resolution;
            }
            set
            {
                resolution = value;
                UpdateExtent(resolution, center, width, height);
            }
        }

        public Point Center
        {
            set
            {
                center = value;
                UpdateExtent(resolution, center, width, height);
            }
        }

        public double Width
        {
            set
            {
                width = value;
                UpdateExtent(resolution, center, width, height);
            }
            get { return width; }
        }

        public double Height
        {
            set
            {
                height = value;
                UpdateExtent(resolution, center, width, height);
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

        public void Pan(Vector translate)
        {
            Vector vector = new Vector(-translate.X * resolution, translate.Y * resolution);
            this.center = Point.Add(center, vector);
            this.UpdateExtent(resolution, center, width, height);
        }

        public Extent Extent
        {
            get { return extent; }
        }

        private void UpdateExtent(double resolution, Point center, double width, double height)
        {
            if ((width == 0) || (height == 0)) return;

            double spanX = width * resolution;
            double spanY = height * resolution;
            extent = new Extent(center.X - spanX * 0.5, center.Y - spanY * 0.5, center.X + spanX * 0.5, center.Y + spanY * 0.5);

            Matrix matrix = new Matrix();
            double mapCenterX = width * 0.5;
            double mapCenterY = height * 0.5;

            matrix.Translate(mapCenterX - center.X, mapCenterY - center.Y);

            matrix.ScaleAt(1 / resolution, 1 / resolution, mapCenterX, mapCenterY);

            matrix.Append(new Matrix(1, 0, 0, -1, 0, 0));
            matrix.Translate(0, height);

            transform.Matrix = matrix;
        }

        public void Zoom(Rect zoomRect, Rect prevZoomRect)
        {
            Matrix matrix = transform.Matrix;
            matrix.Translate(-GetCenterX(prevZoomRect), -GetCenterY(prevZoomRect));
            double scale = zoomRect.Width / prevZoomRect.Width;
            matrix.Scale(scale, scale);
            matrix.Translate(GetCenterX(zoomRect), GetCenterY(zoomRect));
            transform.Matrix = matrix;
        }

        public void ScaleAt(double scale, Point origin)
        {
            Matrix matrix = transform.Matrix;
            matrix.ScaleAt(scale, scale, origin.X, origin.Y);
            transform.Matrix = matrix;
            if (transform.Inverse == null) return; //happens when extermely zoomed out.
            center = transform.Inverse.Transform(new Point(this.width / 2, this.height / 2));
            resolution = resolution / scale;
            UpdateExtent(this.resolution, this.center, this.width, this.height);
        }

        public Point WorldToMap(double x, double y)
        {
            Point point;
            point = transform.Transform(new Point(x, y));
            return point;
        }

        public Point MapToWorld(double x, double y)
        {
            Point point;
            GeneralTransform inverseTransform = transform.Inverse;

            point = inverseTransform.Transform(new Point(x, y));
            return point;
        }

        BTPoint ITransform.WorldToMap(double x, double y)
        {
          Point point = WorldToMap(x, y);
          return new BTPoint((float)point.X, (float)point.Y);
        }

        BTPoint ITransform.MapToWorld(double x, double y)
        {
          Point point = MapToWorld(x, y);
          return new BTPoint((float)point.X, (float)point.Y);
        }

        public Vector MapToWorld(Vector vector)
        {
            Point point;
            GeneralTransform inverseTransform = transform.Inverse;
            point = inverseTransform.Transform(new Point(vector.X - center.X, vector.Y - center.Y));
            return new Vector(point.X, point.Y);
        }

        private static double GetCenterX(Rect rect)
        {
            return ((rect.Left + rect.Right) * 0.5F);
        }

        private static double GetCenterY(Rect rect)
        {
            return ((rect.Top + rect.Bottom) * 0.5F);
        }
  }
}
