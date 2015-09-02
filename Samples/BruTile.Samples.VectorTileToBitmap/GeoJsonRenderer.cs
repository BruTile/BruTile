using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace BruTile.Samples.VectorTileToBitmap
{
    public class GeoJsonRenderer
    {
        private const int TileWidth = 256;
        private const int TileHeight = 256;

        public static byte[] ToBitmap(IEnumerable<FeatureCollection> featureCollections, float minX, float minY, float width, float height)
        {
            var random = new Random();

            using (var bitmap = new Bitmap(TileWidth, TileHeight))
            using (var canvas = Graphics.FromImage(bitmap))
            {
                foreach (var featureCollection in featureCollections)
                {
                    foreach (var feature in featureCollection.Features)
                    {
                        if (feature.Geometry.Type == GeoJSONObjectType.Polygon)
                        {
                            var polygon = (Polygon) feature.Geometry;

                            foreach (var lineString in polygon.Coordinates)
                            {
                                canvas.Transform = CreateTransformMatrix(minX, minY, width, height);
                                using (var brush = new SolidBrush(
                                    Color.FromArgb(random.Next(256), random.Next(256), random.Next(256))))
                                {
                                    canvas.FillPolygon(brush, ToGdi(lineString));

                                }
                            }
                        }
                    }
                }

                return ToBytes(bitmap);
            }
        }

        private static Matrix CreateTransformMatrix(float minX, float minY, float width, float height)
        {
            // The code below needs no comments, it is fully intuitive.
            // I wrote in in one go and it ran correctly right away.
            var matrix = new Matrix();
            var flipMatrix = new Matrix(1, 0, 0, -1, 0, 0);
            matrix.Multiply(flipMatrix);
            matrix.Scale(TileWidth/width, TileHeight/height);
            var maxY = minY + height;
            matrix.Translate(-minX, -maxY);
            return matrix;
        }

        private static PointF[] ToGdi(LineString lineString)
        {
            var result = new List<PointF>();

            foreach (var coordinate in lineString.Coordinates)
            {
                var position = (GeographicPosition) coordinate;
                result.Add(SphericalMercator.FromLonLat(position.Longitude, position.Latitude));
            }
            return result.ToArray();
        }

        public static byte[] ToBytes(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
