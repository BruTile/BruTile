// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Windows.Controls;
using BruTile.Cache;
using BruTile.Samples.Common;

namespace BruTile.Demo
{
    internal class Renderer
    {
        private readonly Canvas _canvas;

        public Renderer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void Render(Viewport viewport, ITileSource tileSource, ITileCache<Tile<Image>> tileCache)
        {
            _canvas.Children.Clear();

            var level = Utilities.GetNearestLevel(tileSource.Schema.Resolutions, viewport.UnitsPerPixel);
            var tileInfos = tileSource.Schema.GetTileInfos(viewport.Extent, level);
            foreach (var tileInfo in tileInfos)
            {
                var tile = tileCache.Find(tileInfo.Index);
                if (tile != null)
                {
                    _canvas.Children.Add(tile.Image);
                    PositionImage(tile.Image, tileInfo.Extent, viewport);
                }
            }
        }

        public static void PositionImage(Image image, Extent extent, Viewport viewport)
        {
            var min = viewport.WorldToScreen(extent.MinX, extent.MinY);
            var max = viewport.WorldToScreen(extent.MaxX, extent.MaxY);

            Canvas.SetLeft(image, min.X);
            Canvas.SetTop(image, max.Y);
            image.Width = max.X - min.X;
            image.Height = min.Y - max.Y;
        }
    }
}
