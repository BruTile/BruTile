using BruTile.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace BruTile.Metro
{
    public static class Renderer
    {
        public static void Render(BruTile.Samples.Common.Viewport viewport, Canvas canvas, IEnumerable<Tile> tiles)
        {
            if (viewport == null) return;

            canvas.Children.Clear();

            var tileList = tiles.ToList();
            for (int i = 0; i < tileList.Count; i++)
            {
                var tile = tileList[i];
                if (tile.Image == null) continue;
                var point1 = viewport.WorldToScreen(tile.Info.Extent.MinX, tile.Info.Extent.MaxY);
                var point2 = viewport.WorldToScreen(tile.Info.Extent.MaxX, tile.Info.Extent.MinY);

                var dest = new Rect(point1.ToMetroPoint(), point2.ToMetroPoint());
                dest = RoundToPixel(dest);

                Canvas.SetLeft(tile.Image, dest.X);
                Canvas.SetTop(tile.Image, dest.Y);
                tile.Image.Width = dest.Width;
                tile.Image.Height = dest.Height;
                canvas.Children.Add(tile.Image);

                if (tile.Image.Tag == null)
                {
                    tile.Image.Tag = DateTime.Now.Ticks;
                    Animate(tile.Image, "Opacity", 0, 1, 600, (s, e) => { });
                }
            }
        }

        public static void Animate(DependencyObject target, string property, double from, double to, int duration, EventHandler<object> completed)
        {
            var animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.Duration = new TimeSpan(0, 0, 0, 0, duration);
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, property);

            var storyBoard = new Storyboard();
            storyBoard.Children.Add(animation);
            storyBoard.Completed += completed;
            storyBoard.Begin();
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
