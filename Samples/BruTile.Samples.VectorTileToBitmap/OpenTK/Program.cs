using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics.ES11;

namespace FrameBufferObject2DToBitmap
{
    class Program 
    {
        private const int Width = 400;
        private const int Height = 300;

        public static void Main() 
        {
            var rasterizer = new OpenTKRasterizer();
            var byteArray = rasterizer.Rasterize(Width, Height, TestRender);
            SaveToDisk(byteArray);
        }

        private static void SaveToDisk(byte[] byteArray)
        {
            var bitmap = Image.FromStream(new MemoryStream(byteArray));
            if (!Directory.Exists("Images")) Directory.CreateDirectory("Images");
            bitmap.Save(@"Images\\image.png");
        }

        public static void TestRender()
        {
            float lineWidth = 1;
            var lineColor = Color.Blue;

            var lineString = new List<PointF> { new PointF(0, 0), new PointF(100f, 100f), new PointF(100f, -100f), new PointF(0, 0), new PointF(-100f, 100f), new PointF(-100f, -100f), new PointF(0f, 0f) };

            float[] points = ToOpenTK(lineString);
            
            GL.LineWidth(lineWidth);
            GL.Color4((byte)lineColor.R, (byte)lineColor.G, (byte)lineColor.B, (byte)lineColor.A);
            GL.EnableClientState(All.VertexArray);
            GL.VertexPointer(2, All.Float, 0, points);
            GL.DrawArrays(All.Lines, 0, points.Length / 2);
            GL.DisableClientState(All.VertexArray);
        }

        private static float[] ToOpenTK(IList<PointF> vertices)
        {
            const int dimensions = 2; // x and y are both in one array
            int numberOfCoordinates = vertices.Count * 2 - 2; // Times two because of duplicate begin en end. Minus two because the very begin and end need no duplicate
            var points = new float[numberOfCoordinates * dimensions];

            for (var i = 0; i < vertices.Count - 1; i++)
            {
                points[i * 4 + 0] = vertices[i].X;
                points[i * 4 + 1] = vertices[i].Y;
                points[i * 4 + 2] = vertices[i + 1].X;
                points[i * 4 + 3] = vertices[i + 1].Y;
            }
            return points;
        }
    }
}
