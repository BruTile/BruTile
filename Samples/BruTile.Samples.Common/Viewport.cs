using BruTile.Samples.Common.Geometries;
using System;
using System.Linq;

namespace BruTile.Samples.Common
{
    public class Viewport
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

        public Point WorldToScreen(Point worldPosition)
        {
            return WorldToScreen(worldPosition.X, worldPosition.Y);
        }

        public Point ScreenToWorld(Point screenPosition)
        {
            return ScreenToWorld(screenPosition.X, screenPosition.Y);
        }

        public Point WorldToScreen(double worldX, double worldY)
        {
            return new Point((worldX - _extent.MinX) / _resolution, (_extent.MaxY - worldY) / _resolution);
        }

        public Point ScreenToWorld(double screenX, double screenY)
        {
            return new Point((_extent.MinX + screenX * _resolution), (_extent.MaxY - (screenY * _resolution)));
        }

        public void Transform(double screenX, double screenY, double previousScreenX, double previousScreenY, double deltaScale = 1)
        {         
            var previous = ScreenToWorld(previousScreenX, previousScreenY);
            var current = ScreenToWorld(screenX, screenY);
            
            var newX = CenterX + previous.X - current.X;
            var newY = CenterY + previous.Y - current.Y;

            // When you pinch zoom outside the center of the map 
            // this will also affect the new center. 
            var scaleCorrectionX = (1 - deltaScale) * (current.X - CenterX);
            var scaleCorrectionY = (1 - deltaScale) * (current.Y - CenterY);

            Resolution = Resolution / deltaScale;
            CenterX = newX - scaleCorrectionX;
            CenterY = newY - scaleCorrectionY;
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
