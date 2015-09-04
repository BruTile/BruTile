using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace BruTile.Samples.VectorTileToBitmap
{
    class GraphicsContextToBitmapConverter
    {
        public static byte[] ToBitmap(int width, int height)
        {
            var bitmap = GrabScreenshot(width, height);
            var byteArray = BitmapToByteArray(bitmap);
            return byteArray;
        }

        private static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

        private static Bitmap GrabScreenshot(int width, int height)
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();

            var bitmap = new Bitmap(width, height);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, width, height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            
            bitmap.UnlockBits(data);
            //bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }
    }
}
