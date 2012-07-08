using System;
using System.Linq;
using System.Windows;

namespace BruTile.Demo
{
    class Viewport
    {
        #region Fields

        private double _resolution;
        private double _centerX;
        private double _centerY;
        private double _width;
        private double _height;
        Extent _extent;   

        #endregion

        #region Public Methods

        public double Resolution
        {
            get { return _resolution; }
            set
            {
                _resolution = value;
                UpdateExtent();
            }
        }

        public Point Center
        {
            set
            {
                _centerX = value.X;
                _centerY = value.Y;
                UpdateExtent();
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                UpdateExtent();
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                UpdateExtent();
            }
        }

        public double CenterX
        {
            get { return _centerX; }
            set
            {
                UpdateExtent();
                _centerX = value;
            }
        }

        public double CenterY
        {
            get { return _centerY; }
            set
            {
                UpdateExtent();
                _centerY = value;
            }
        }
        
        public Extent Extent
        {
            get { return (_extent == default(Extent)) ? (_extent = new Extent(0, 0, 0, 0)) : _extent; }
        }

        public Point WorldToView(Point point)
        {
            return WorldToView(point.X, point.Y);
        }

        public Point ViewToWorld(Point point)
        {
            return ViewToWorld(point.X, point.Y);
        }

        public Point WorldToView(double x, double y)
        {
            return new Point((x - _extent.MinX) / _resolution, (_extent.MaxY - y) / _resolution);
        }

        public Point ViewToWorld(double x, double y)
        {
            return new Point((_extent.MinX + x * _resolution), (_extent.MaxY - (y * _resolution)));
        }

        #endregion

        #region Private Methods

        private void UpdateExtent()
        {
            double spanX = _width * _resolution;
            double spanY = _height * _resolution;
            _extent = new Extent(
                CenterX - spanX * 0.5, CenterY - spanY * 0.5,
                CenterX + spanX * 0.5, CenterY + spanY * 0.5);
        }

        #endregion
    }
}
