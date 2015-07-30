using System;
using System.Drawing;

namespace BruTile.Samples.VectorTileToBitmap
{
    public class SphericalMercator
    {
        private const double Radius = 6378137;
        private const double D2R = Math.PI / 180;
        private const double HalfPi = Math.PI / 2;

        public static PointF FromLonLat(double lon, double lat)
        {
            var lonRadians = (D2R * lon);
            var latRadians = (D2R * lat);

            var x = Radius * lonRadians;
            var y = Radius * Math.Log(Math.Tan(Math.PI * 0.25 + latRadians * 0.5));

            return new PointF((float)x, (float)y);
        }

        public static PointF ToLonLat(double x, double y)
        {
            var ts = Math.Exp(-y / (Radius));
            var latRadians = HalfPi - 2 * Math.Atan(ts);

            var lonRadians = x / (Radius);

            var lon = (lonRadians / D2R);
            var lat = (latRadians / D2R);

            return new PointF((float)lon, (float)lat);
        }
    }
}