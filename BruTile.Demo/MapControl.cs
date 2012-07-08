using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BruTile.Cache;
using BruTile.Web;
using SharpMap.Utilities;

namespace BruTile.Demo
{
    class MapControl : Grid
    {
        private readonly Canvas _canvas;
        private bool _invalid = true;
        private readonly Renderer _renderer;
        private Viewport _viewport;
        private readonly ITileSource _tileSource;
        private readonly ITileCache<Image> _tileCache = new MemoryCache<Image>(200, 300);
        private readonly Fetcher _fetcher = new Fetcher();

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
            _fetcher.DataChanged += FetcherDataChanged;
            _invalid = true;
        }

        void FetcherDataChanged(object sender, EventArgs e)
        {
            _invalid = true;
            InvalidateVisual();
        }

        void MapControlMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _viewport.Resolution = ZoomHelper.ZoomIn(_tileSource.Schema.Resolutions, _viewport.Resolution);
            }
            else if (e.Delta < 0)
            {
                _viewport.Resolution = ZoomHelper.ZoomOut(_tileSource.Schema.Resolutions, _viewport.Resolution);
            }

            _fetcher.Fetch(_viewport, _tileSource, _tileCache);
            e.Handled = true; //so that the scroll event is not sent to the html page.
            _invalid = true;
        }

        void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (_viewport == null)
            {
                _viewport = TryInitializeViewport(ActualWidth, ActualHeight, _tileSource.Schema);
            }
            if (_viewport == null) return; 
            if (!_invalid) return;

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
            var viewport = new Viewport();
            viewport.Width = actualWidth;
            viewport.Height = actualHeight;
            viewport.Resolution = schema.Extent.Width / actualWidth;
            viewport.Center = new Point(schema.Extent.CenterX, schema.Extent.CenterY);
            return viewport;
        }
    }
}
