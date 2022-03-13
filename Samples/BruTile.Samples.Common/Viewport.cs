using BruTile.Samples.Common.Geometries;

namespace BruTile.Samples.Common
{
    public class Viewport
    {
        private double _unitsPerPixel;
        private double _centerX;
        private double _centerY;
        private double _width;
        private double _height;
        Extent _extent;   

        public double UnitsPerPixel 
        {
            get => _unitsPerPixel;
            set
            {
                _unitsPerPixel = value;
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
            get => _width;
            set
            {
                _width = value;
                UpdateExtent();
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                UpdateExtent();
            }
        }

        public double CenterX
        {
            get => _centerX;
            set
            {
                _centerX = value;
                UpdateExtent();
            }
        }

        public double CenterY
        {
            get => _centerY;
            set
            {
                _centerY = value;
              UpdateExtent();
            }
        }
        
        public Extent Extent => (_extent == default(Extent)) ? (_extent = new Extent(0, 0, 0, 0)) : _extent;

        public Point WorldToScreen(double worldX, double worldY)
        {
            return new Point((worldX - _extent.MinX) / _unitsPerPixel, (_extent.MaxY - worldY) / _unitsPerPixel);
        }

        public Point ScreenToWorld(double screenX, double screenY)
        {
            return new Point((_extent.MinX + screenX * _unitsPerPixel), (_extent.MaxY - (screenY * _unitsPerPixel)));
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

            UnitsPerPixel = UnitsPerPixel / deltaScale;
            CenterX = newX - scaleCorrectionX;
            CenterY = newY - scaleCorrectionY;
        }

        private void UpdateExtent()
        {
            double spanX = _width * _unitsPerPixel;
            double spanY = _height * _unitsPerPixel;
            _extent = new Extent(
                CenterX - spanX * 0.5, CenterY - spanY * 0.5,
                CenterX + spanX * 0.5, CenterY + spanY * 0.5);
        }
    }
}
