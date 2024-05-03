// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Drawing;

namespace BruTile.Samples.MbTiles;

public struct PointD(double x, double y)
{
    public double X { get; set; } = x;

    public double Y { get; set; } = y;

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
    private double _unitsPerPixel;
    private PointD _center;
    private double _width;
    private double _height;
    private Extent _extent;

    public MapTransform(PointD center, double unitsPerPixel, double width, double height)
    {
        _center = center;
        _unitsPerPixel = unitsPerPixel;
        _width = width;
        _height = height;
        UpdateExtent();
    }

    public double UnitsPerPixel
    {
        get => _unitsPerPixel;
        set
        {
            _unitsPerPixel = value;
            UpdateExtent();
        }
    }

    public PointD Center
    {
        get => _center;
        set
        {
            _center = value;
            UpdateExtent();
        }
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

    public Extent Extent => _extent;

    public PointF WorldToMap(double x, double y)
    {
        return new PointD((x - _extent.MinX) / _unitsPerPixel, (_extent.MaxY - y) / _unitsPerPixel);
    }

    public PointD MapToWorld(double x, double y)
    {
        return new PointD((_extent.MinX + (x * _unitsPerPixel)), (_extent.MaxY - (y * _unitsPerPixel)));
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

    private void UpdateExtent()
    {
        var spanX = _width * _unitsPerPixel;
        var spanY = _height * _unitsPerPixel;
        _extent = new Extent(
            _center.X - spanX * 0.5f, _center.Y - spanY * 0.5f,
            _center.X + spanX * 0.5f, _center.Y + spanY * 0.5f);
    }
}
