using BruTile.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace BruTile.Metro
{
    public static class Renderer
    {
        public static void Render(BruTile.Samples.Common.Viewport viewport, Canvas canvas,
            ITileSource tileSource, ITileCache<Image> tileCache)
        {
            if (viewport == null) return;

            canvas.Children.Clear();

            var level = BruTile.Utilities.GetNearestLevel(tileSource.Schema.Resolutions, viewport.Resolution);
            var tileInfos = tileSource.Schema.GetTilesInView(viewport.Extent, level);

            foreach (var tileInfo in tileInfos)
            {
                var image = tileCache.Find(tileInfo.Index);
                if (image == null) continue;
                var point1 = viewport.WorldToScreen(tileInfo.Extent.MinX, tileInfo.Extent.MaxY);
                var point2 = viewport.WorldToScreen(tileInfo.Extent.MaxX, tileInfo.Extent.MinY);

                var dest = new Rect(point1.ToMetroPoint(), point2.ToMetroPoint());
                dest = RoundToPixel(dest);

                Canvas.SetLeft(image, dest.X);
                Canvas.SetTop(image, dest.Y);
                image.Width = dest.Width;
                image.Height = dest.Height;
                canvas.Children.Add(image);
            }
        }

        public static Rect RoundToPixel(Rect dest)
        {
            // To get seamless aligning you need to round the 
            // corner coordinates to pixel. The new width and
            // height will be a result of that.
            dest = new Rect(
                Math.Round(dest.Left),
                Math.Round(dest.Top),
                (Math.Round(dest.Right) - Math.Round(dest.Left)),
                (Math.Round(dest.Bottom) - Math.Round(dest.Top)));
            return dest;
        }

        public static Point ToMetroPoint(this BruTile.Samples.Common.Point point)
        {
            return new Point(point.X, point.Y);
        }

    }
}
