using System.Drawing;
using BruTile;

namespace WinFormsSample
{
    public struct PointD
    {
        private bool _isSet;
        private double _x;

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                _isSet = true;
            }
        }

        private double _y;

        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                _isSet = true;
            }
        }

        public PointD(double x, double y)
        {
            _x = x;
            _y = y;
            _isSet = false;
        }

        public bool IsEmpty { get { return !_isSet; } }

        public static implicit operator PointD(PointF pd)
        {
            return new PointD(pd.X, pd.Y);
        }

        public static implicit operator PointF(PointD pd)
        {
            return new PointF((float)pd.X, (float)pd.Y);
        }
    }

    internal class MapTransform
    {
        #region Fields

        double _resolution;
        PointD _center;
        double _width;
        double _height;
        Extent _extent;

        #endregion Fields

        #region Public Methods

        public MapTransform(PointD center, double resolution, double width, double height)
        {
            _center = center;
            _resolution = resolution;
            _width = width;
            _height = height;
            UpdateExtent();
        }

        public double Resolution
        {
            set
            {
                _resolution = value;
                UpdateExtent();
            }
            get
            {
                return _resolution;
            }
        }

        public PointD Center
        {
            set
            {
                _center = value;
                UpdateExtent();
            }
            get { return _center; }
        }

        public double Width
        {
            set
            {
                _width = value;
                UpdateExtent();
            }
        }

        public double Height
        {
            set
            {
                _height = value;
                UpdateExtent();
            }
        }

        public Extent Extent
        {
            get { return _extent; }
        }

        public PointF WorldToMap(double x, double y)
        {
            return new PointD((x - _extent.MinX) / _resolution, (_extent.MaxY - y) / _resolution);
        }

        public PointD MapToWorld(double x, double y)
        {
            return new PointD((_extent.MinX + (x * _resolution)), (_extent.MaxY - (y * _resolution)));
        }

        public RectangleF WorldToMap(Extent extent)
        {
            return WorldToMap(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);
        }

        public RectangleF WorldToMap(double x1, double y1, double x2, double y2)
        {
            var point1 = WorldToMap(x1, y1);
            var point2 = WorldToMap(x2, y2);
            return new RectangleF(point1.X, point2.Y, point2.X - point1.X, point1.Y - point2.Y);
        }

        #endregion Public Methods

        #region Private Methods

        private void UpdateExtent()
        {
            var spanX = _width * _resolution;
            var spanY = _height * _resolution;
            _extent = new Extent(
                _center.X - spanX * 0.5f, _center.Y - spanY * 0.5f,
                _center.X + spanX * 0.5f, _center.Y + spanY * 0.5f);
        }

        #endregion Private Methods
    }
}