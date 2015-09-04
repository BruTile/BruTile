using System.Collections.Generic;
using FrameBufferObject2DToBitmap;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using OpenTK.Graphics.ES11;

namespace BruTile.Samples.VectorTileToBitmap
{
    public class GeoJSONToOpenTKRenderer
    {
        private readonly int _canvasWidth;
        private readonly int _canvasHeight;
        private readonly float _extentMinX;
        private readonly float _extentMinY;
        private readonly float _extentWidth;
        private readonly float _extentHeight;
        private IEnumerable<FeatureCollection> _featureCollections;

        public GeoJSONToOpenTKRenderer(int canvasWidth, int canvasHeight, double[] boundingBox)
        {
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _extentMinX = (float)boundingBox[0];
            _extentMinY = (float)boundingBox[1];
            _extentWidth = (float)boundingBox[2] - _extentMinX;
            _extentHeight = (float)boundingBox[3] - _extentMinY;
        }

        public byte[] Render(IEnumerable<FeatureCollection> featureCollections)
        {
            _featureCollections = featureCollections; // ouch
            var rasterizer = new OpenTKRasterizer();
            var byteArray = rasterizer.Rasterize(_canvasWidth, _canvasHeight, PolygonRenderer);
            return byteArray;
        }

        private void PolygonRenderer()
        {
            foreach (var featureCollection in _featureCollections)
            {
                foreach (var feature in featureCollection.Features)
                {
                    if (feature.Geometry.Type == GeoJSONObjectType.Polygon)
                    {
                        var polygon = (Polygon) feature.Geometry;

                        foreach (var lineString in polygon.Coordinates)
                        {
                            //canvas.Transform = CreateTransformMatrix(_canvasWidth, _canvasHeight, _extentMinX, _extentMinY, _extentWidth, _extentHeight);
                            RenderLineString(lineString);
                        }
                    }
                }
            }
        }

        private void RenderLineString(LineString lineString)
        {
            float lineWidth = 1;

            float[] points = ToOpenTK(lineString);
       
            GL.PushMatrix();
            GL.Scale(_canvasWidth / _extentWidth, _canvasHeight / _extentHeight, 1);
            GL.Translate(-_extentMinX, -_extentMinY, 0);

            GL.LineWidth(lineWidth);
            GL.Color4(0, 0, 0, 255);
            GL.EnableClientState(All.VertexArray);
            GL.VertexPointer(2, All.Float, 0, points);
            GL.DrawArrays(All.LineLoop, 0, points.Length/2);
            GL.DisableClientState(All.VertexArray);

            GL.PopMatrix();
        }

        private static float[] ToOpenTK(LineString lineString)
        {
            const int dimensions = 2; // x and y are both in one array
            var points = new float[lineString.Coordinates.Count* dimensions];

            var counter = 0;
            foreach (var coordinate in lineString.Coordinates)
            {
                var position = (GeographicPosition)coordinate;
                var point = SphericalMercator.FromLonLat(position.Longitude, position.Latitude);
                points[counter * 2 + 0] = point.X;
                points[counter * 2 + 1] = point.Y;
                counter++;
            }

            return points;
        }

        //private static Matrix4 CreateTransformMatrix(int canvasWidth, int canvasHeight, float minX, float minY, float width, float height)
        //{
        //    // The code below needs no comments, it is fully intuitive.
        //    // I wrote in in one go and it ran correctly right away.
        //    var matrix = new Matrix4();
        //    var flipMatrix = new Matrix4(1, 0, 0, -1, 0, 0);
        //    matrix..Multiply(flipMatrix);
        //    matrix.Scale(canvasWidth / width, canvasHeight / height);
        //    var maxY = minY + height;
        //    matrix..Translate(-minX, -maxY);
        //    return matrix;
        //}

    }
}
