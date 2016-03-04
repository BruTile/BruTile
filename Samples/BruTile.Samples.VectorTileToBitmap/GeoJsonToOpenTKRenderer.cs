using System;
using System.Collections.Generic;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using OpenTK;
using OpenTK.Graphics;
using All = OpenTK.Graphics.ES11.All;
using ClearBufferMask = OpenTK.Graphics.ES20.ClearBufferMask;
using GL = OpenTK.Graphics.ES11.GL;
using MatrixMode = OpenTK.Graphics.ES11.MatrixMode;
using StringName = OpenTK.Graphics.ES11.StringName;

namespace BruTile.Samples.VectorTileToBitmap
{
    public class GeoJSONToOpenTKRenderer
    {
        private readonly int _pixelWidth;
        private readonly int _pixelHeight;
        private readonly float _extentMinX;
        private readonly float _extentMinY;
        private readonly float _extentWidth;
        private readonly float _extentHeight;
        private readonly object _syncRoot = new object();

        public GeoJSONToOpenTKRenderer(int pixelWidth, int pixelHeight, double[] boundingBox)
        {
            _pixelWidth = pixelWidth;
            _pixelHeight = pixelHeight;
            _extentMinX = (float)boundingBox[0];
            _extentMinY = (float)boundingBox[1];
            _extentWidth = (float)boundingBox[2] - _extentMinX;
            _extentHeight = (float)boundingBox[3] - _extentMinY;
        }

        public byte[] Render(IEnumerable<FeatureCollection> featureCollections)
        {
            lock (_syncRoot)
            {
                // There needs to be a gamewindow even though we don't write to screen. It is created but not used explicitly in our code.
                using (var gameWindow = new GameWindow(_pixelWidth, _pixelHeight))
                {
                    if (!GL.GetString(StringName.Extensions).Contains("GL_EXT_framebuffer_object"))
                    {
                        throw new NotSupportedException(
                            "GL_EXT_framebuffer_object extension is required. Please update your drivers.");
                    }

                    FrameBufferObjectHelper.StartFrameBufferObject(_pixelWidth, _pixelHeight);

                    OpenTK.Graphics.ES20.GL.ClearColor(Color4.White);
                    OpenTK.Graphics.ES20.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    Set2DViewport(_pixelWidth, _pixelHeight);

                    GL.PushMatrix();
                    
                    GL.Scale(_pixelWidth / _extentWidth, _pixelHeight / _extentHeight, 1);
                    GL.Translate(-_extentMinX, -_extentMinY, 0);
                    
                    PolygonRenderer(featureCollections);
                    var byteArray = GraphicsContextToBitmapConverter.ToBitmap(_pixelWidth, _pixelHeight);
                    
                    GL.PopMatrix();

                    FrameBufferObjectHelper.StopFrameBufferObject();

                    return byteArray;
                }
            }
        }

        private static void Set2DViewport(int pixelWidth, int pixelHeight)
        {
            GL.Viewport(0, 0, pixelWidth, pixelHeight);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            OpenTK.Graphics.OpenGL.GL.Ortho(0, pixelWidth, pixelHeight, 0, -1, 1); // This has no effect: OpenTK.Graphics.ES11.GL.Ortho(0, width, height, 0, 0, 1); 
        }

        private void PolygonRenderer(IEnumerable<FeatureCollection> featureCollections)
        {
            foreach (var featureCollection in featureCollections)
            {
                foreach (var feature in featureCollection.Features)
                {
                    if (feature.Geometry.Type == GeoJSONObjectType.Polygon)
                    {
                        var polygon = (Polygon)feature.Geometry;

                        foreach (var lineString in polygon.Coordinates)
                        {
                            RenderPolygon(lineString);
                        }
                    }
                }
            }
        }

        private void RenderPolygon(LineString lineString)
        {
            float lineWidth = 0;

            float[] points = ToOpenTK(lineString);

            GL.LineWidth(lineWidth);
            GL.Color4(0, 0, 0, 255);
            GL.EnableClientState(All.VertexArray);
            GL.VertexPointer(2, All.Float, 0, points);
            GL.DrawArrays(All.LineLoop, 0, points.Length / 2);
            GL.DisableClientState(All.VertexArray);
        }

        private static float[] ToOpenTK(LineString lineString)
        {
            const int dimensions = 2; // x and y are both in one array
            var points = new float[lineString.Coordinates.Count * dimensions];

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
    }
}
