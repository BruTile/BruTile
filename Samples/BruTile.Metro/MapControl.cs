using BruTile.Cache;
using BruTile.Samples.Common;
using BruTile.Web;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace BruTile.Metro
{
    public class MapControl : Canvas
    {
        readonly ITileCache<Tile<Image>> _tileCache = new MemoryCache<Tile<Image>>(200, 300);
        readonly OsmTileSource _osmTileSource = new OsmTileSource();
        readonly Fetcher<Image> _fetcher;
        Viewport _viewport;
        Point _previousPosition;        

        public MapControl()
        {
            _fetcher = new Fetcher<Image>(_osmTileSource, _tileCache);
            _fetcher.DataChanged += FetcherDataChanged;

            CompositionTarget.Rendering += CompositionTargetRendering;
            SizeChanged += MapControlSizeChanged;

            ManipulationMode = ManipulationModes.Scale | ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            ManipulationDelta += OnManipulationDelta;
            ManipulationCompleted += OnManipulationCompleted;
            ManipulationInertiaStarting += OnManipulationInertiaStarting;
        }

        private static void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 25 * 96.0 / (1000.0 * 1000.0);
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_previousPosition == default(Point) || double.IsNaN(_previousPosition.X))
            {
                _previousPosition = e.Position;
                return;
            }

            if (Distance(e.Position.X, e.Position.Y, _previousPosition.X, _previousPosition.Y) > 50)
            {
                _previousPosition = default(Point);
                return;
            }

            _viewport.Transform(e.Position.X, e.Position.Y, _previousPosition.X, _previousPosition.Y, e.Delta.Scale);
            
            _previousPosition = e.Position;
            _fetcher.ViewChanged(_viewport.Extent, _viewport.Resolution);
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2.0) + Math.Pow(y1 - y2, 2.0));
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _previousPosition = default(Point);
        }

        async void FetcherDataChanged(object sender, DataChangedEventArgs<Image> e)
        {
            if (!Dispatcher.HasThreadAccess)
                Dispatcher.RunAsync(
                   Windows.UI.Core.CoreDispatcherPriority.High,
                   () => FetcherDataChanged(sender, e));
            else
            {
                if (e.Error == null && e.Tile != null)
                {
                    e.Tile.Image = await CreateImage(e.Tile.Data);
                    _tileCache.Add(e.Tile.Info.Index, e.Tile);
                    InvalidateArrange();
                }
            }
        }

        private static async System.Threading.Tasks.Task<Image> CreateImage(byte[] data)
        {
            return new Image
                {
                Source = await Utilities.TileToImage(data),
                Stretch = Stretch.Fill,
                IsHitTestVisible = false
            };
        }

        void MapControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_viewport == null)
            {
                if (CanInitializeViewport(ActualWidth, ActualHeight))
                {
                    _viewport = InitializeViewport(ActualWidth, ActualHeight, FullMapExtent);
                    _fetcher.ViewChanged(_viewport.Extent, _viewport.Resolution);
                }
            }
            else
            {
                _viewport.Width = e.NewSize.Width;
                _viewport.Height = e.NewSize.Height;
                _fetcher.ViewChanged(_viewport.Extent, _viewport.Resolution);
            }
            InvalidateArrange();
        }

        void CompositionTargetRendering(object sender, object e)
        {
            var tilesToRender = TileLayer<Image>.SelectTilesToRender(_tileCache, _osmTileSource.Schema, _viewport.Extent, _viewport.Resolution);
            Renderer.Render(_viewport, this, tilesToRender);
        }

        private static bool CanInitializeViewport(double actualWidth, double actualHeight)
        {
            return (!double.IsNaN(actualWidth) && actualWidth > 0
                && !double.IsNaN(actualHeight) && actualHeight > 0);
        }

        private static Viewport InitializeViewport(double actualWidth, double actualHeight, Extent extent)
        {
            return new Viewport
                {
                    Width = actualWidth,
                    Height = actualHeight,
                    Resolution = extent.Width/actualWidth,
                    Center = new Samples.Common.Geometries.Point(extent.CenterX, extent.CenterY)
                };
        }

        public Extent FullMapExtent
        {
            get { return _osmTileSource.Extent; }
        }

        internal void ZoomInOneStep()
        {
            _viewport.Resolution = _viewport.Resolution / 2;
            _fetcher.ViewChanged(_viewport.Extent, _viewport.Resolution);
        }

        internal void ZoomOutOneStep()
        {
            _viewport.Resolution = _viewport.Resolution * 2;
            _fetcher.ViewChanged(_viewport.Extent, _viewport.Resolution);
        }
    }
}
