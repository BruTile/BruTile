using System.Drawing;

namespace BruTile.Samples.SimpleStaticMap
{
    class Viewport
    {
        float _unitsPerPixel;
        PointF _center;
        float _width;
        float _height;
        Extent _extent;

        public Viewport(PointF center, float unitsPerPixel, float width, float height)
        {
            _center = center;
            _unitsPerPixel = unitsPerPixel;
            _width = width;
            _height = height;
            UpdateExtent();
        }

        public float UnitsPerPixel
        {
            get => _unitsPerPixel;
            set
            {
                _unitsPerPixel = value;
                UpdateExtent();
            }
        }

        public PointF Center
        {
            set
            {
                _center = value;
                UpdateExtent();
            }
        }

        public float Width
        {
            set
            {
                _width = value;
                UpdateExtent();
            }
        }

        public float Height
        {
            set
            {
                _height = value;
                UpdateExtent();
            }
        }

        public Extent Extent => _extent;

        public PointF WorldToScreen(double x, double y)
        {
            return new PointF((float)(x - _extent.MinX) / _unitsPerPixel, (float)(_extent.MaxY - y) / _unitsPerPixel);
        }

        public RectangleF WorldToScreen(double x1, double y1, double x2, double y2)
        {
            var point1 = WorldToScreen(x1, y1);
            var point2 = WorldToScreen(x2, y2);
            return new RectangleF(point1.X, point2.Y, point2.X - point1.X, point1.Y - point2.Y);
        }

        private void UpdateExtent()
        {
            float spanX = _width * _unitsPerPixel;
            float spanY = _height * _unitsPerPixel;
            _extent = new Extent(_center.X - spanX * 0.5f, _center.Y - spanY * 0.5f,
              _center.X + spanX * 0.5f, _center.Y + spanY * 0.5f);
        }
    }
}
