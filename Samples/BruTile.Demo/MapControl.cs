using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BruTile.Cache;
using BruTile.Samples.Common;
using BruTile.Web;

namespace BruTile.Demo
{
    class MapControl : Grid
    {
        private readonly Canvas _canvas;
        private readonly Fetcher<Image> _fetcher;
        private readonly Renderer _renderer;
        private readonly ITileCache<Tile<Image>> _tileCache = new MemoryCache<Tile<Image>>(200, 300);
        private readonly ITileSource _tileSource;
        private bool _invalid = true;
        private Viewport _viewport;

        public MapControl()
        {
            _canvas = new Canvas
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.Transparent),
            };

            Children.Add(_canvas);
            _renderer = new Renderer(_canvas);

            _tileSource = new OsmTileSource();

            CompositionTarget.Rendering += CompositionTargetRendering;
            MouseWheel += MapControlMouseWheel;
            _fetcher = new Fetcher<Image>(_tileSource, _tileCache);
            _fetcher.DataChanged += FetcherOnDataChanged;
            _invalid = true;
        }

        private void FetcherOnDataChanged(object sender, DataChangedEventArgs<Image> e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new Action(() => FetcherOnDataChanged(sender, e)));
            else
            {
                if (e.Error == null && e.Tile != null)
                {
                    e.Tile.Image = TileToImage(e.Tile.Data);
                    _tileCache.Add(e.Tile.Info.Index, e.Tile);
                    _invalid = true;
                }
            }
        }

        private static Image TileToImage(byte[] tile)
        {
            var stream = new MemoryStream(tile);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            var image = new Image();
            image.BeginInit();
            image.Source = bitmapImage;
            image.EndInit();
            return image;
        }

        void MapControlMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _viewport.Resolution = ZoomHelper.ZoomIn(_tileSource.Schema.Resolutions.Select(r => r.Value.UnitsPerPixel).ToList(), _viewport.Resolution);
            }
            else if (e.Delta < 0)
            {
                _viewport.Resolution = ZoomHelper.ZoomOut(_tileSource.Schema.Resolutions.Select(r => r.Value.UnitsPerPixel).ToList(), _viewport.Resolution);
            }

            _fetcher.ViewChanged(_viewport.Extent, _viewport.Resolution);
            e.Handled = true; //so that the scroll event is not sent to the html page.
            _invalid = true;
        }

        void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (!_invalid) return;

            if (_viewport == null)
            {
                _viewport = TryInitializeViewport(ActualWidth, ActualHeight, _tileSource.Schema);
                if (_viewport != null) _fetcher.ViewChanged(_viewport.Extent, _viewport.Resolution); // Fetch on first initialized 
            }
            if (_viewport == null) return; // failed to initialize so return

            if (_renderer != null)
            {
                _renderer.Render(_viewport, _tileSource, _tileCache);
                _invalid = false;
            }
        }

        private static Viewport TryInitializeViewport(double actualWidth, double actualHeight, ITileSchema schema)
        {
            if (double.IsNaN(actualWidth)) return null;
            if (actualWidth <= 0) return null;

            var nearestLevel = Utilities.GetNearestLevel(schema.Resolutions, schema.Extent.Width / actualWidth);

            return new Viewport
                {
                    Width = actualWidth,
                    Height = actualHeight,
                    Resolution = schema.Resolutions[nearestLevel].UnitsPerPixel,
                    Center = new Samples.Common.Geometries.Point(schema.Extent.CenterX, schema.Extent.CenterY)
                };
        }
    }
}
