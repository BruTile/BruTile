using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Samples.Common;

namespace BruTile.Demo
{
    class MapControl : Grid
    {
        private Fetcher<Image> _fetcher;
        private readonly Renderer _renderer;
        private readonly MemoryCache<Tile<Image>> _tileCache = new MemoryCache<Tile<Image>>(200, 300);
        private ITileSource _tileSource;
        private bool _invalid = true;
        private PointerPoint _previousMousePosition;
        private Viewport _viewport;

        public MapControl()
        {
            var canvas = new Canvas
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.Transparent),
            };

            Children.Add(canvas);
            _renderer = new Renderer(canvas);

            _tileSource = KnownTileSources.Create(); 
            CompositionTarget.Rendering += CompositionTargetRendering;
            SizeChanged += MapControlSizeChanged;
            PointerWheelChanged += MapControlMouseWheel;
            PointerMoved += MapControlMouseMove;
            PointerReleased += OnMouseUp;
            PointerCaptureLost += OnMouseLeave;

            _fetcher = new Fetcher<Image>(_tileSource, _tileCache);
            _fetcher.DataChanged += FetcherOnDataChanged;
        }

        public void SetTileSource(ITileSource source)
        {
            _fetcher.DataChanged -= FetcherOnDataChanged;
            _fetcher.AbortFetch();
            
            _tileSource = source;
            _viewport.CenterX = source.Schema.Extent.CenterX;
            _viewport.CenterY = source.Schema.Extent.CenterY;
            _viewport.UnitsPerPixel = Math.Max(source.Schema.Extent.Width / ActualWidth, source.Schema.Extent.Height / ActualHeight);
            _tileCache.Clear();
            _fetcher = new Fetcher<Image>(_tileSource, _tileCache);
            _fetcher.DataChanged += FetcherOnDataChanged;
            _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel);
            _invalid = true;
        }

        private void OnMouseLeave(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            _previousMousePosition = null;
        }

        private void OnMouseUp(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            _previousMousePosition = null;
        }

        void MapControlMouseMove(object sender, PointerRoutedEventArgs e)
        {
            var pointer = e.GetCurrentPoint(this);
            if (!pointer.Properties.IsLeftButtonPressed) return; 
            if (_previousMousePosition == null)
            {
                _previousMousePosition = e.GetCurrentPoint(this);
                return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
            }

            var currentMousePosition = e.GetCurrentPoint(this); //Needed for both MouseMove and MouseWheel event
            _viewport.Transform(currentMousePosition.Position.X, currentMousePosition.Position.Y, _previousMousePosition.Position.X, _previousMousePosition.Position.Y);
            _previousMousePosition = currentMousePosition;
            _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel);
            _invalid = true;
        }

        void MapControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_viewport == null) return;
            _viewport.Width = ActualWidth;
            _viewport.Height = ActualHeight;
            _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel);
            _invalid = true;
        }

        private async void FetcherOnDataChanged(object sender, DataChangedEventArgs<Image> e)
        {
            async void Func(DataChangedEventArgs<Image> e)
            {
                if (e.Error == null && e.Tile != null)
                {
                    e.Tile.Image = await TileToImage(e.Tile.Data).ConfigureAwait(true);
                    _tileCache.Add(e.Tile.Info.Index, e.Tile);
                    _invalid = true;
                }
            }

            if (!Dispatcher.HasThreadAccess)
                await Dispatcher.RunAsync(CoreDispatcherPriority.High,() => Func(e));
            else
            {
                Func(e);
            }
        }

        private static async Task<Image> TileToImage(byte[] tile)
        {
            var stream = new MemoryStream(tile);

            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());

            var image = new Image
            {
                Source = bitmapImage
            };
            return image;
        }

        void MapControlMouseWheel(object sender, PointerRoutedEventArgs e)
        {
            var currentPoint = e.GetCurrentPoint(this);
            if (currentPoint.Properties.MouseWheelDelta > 0)
            {
                _viewport.UnitsPerPixel = ZoomHelper.ZoomIn(_tileSource.Schema.Resolutions.Select(r => r.Value.UnitsPerPixel).ToList(), _viewport.UnitsPerPixel);
            }
            else if (currentPoint.Properties.MouseWheelDelta < 0)
            {
                _viewport.UnitsPerPixel = ZoomHelper.ZoomOut(_tileSource.Schema.Resolutions.Select(r => r.Value.UnitsPerPixel).ToList(), _viewport.UnitsPerPixel);
            }

            _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel);
            e.Handled = true; //so that the scroll event is not sent to the html page.
            _invalid = true;
        }

        void CompositionTargetRendering(object? sender, object o)
        {
            if (!_invalid) return;
            if (_renderer == null) return;

            if (_viewport == null)
            {
                if (!TryInitializeViewport(ref _viewport, ActualWidth, ActualHeight, _tileSource.Schema)) return;
                _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel); // start fetching when viewport is first initialized
            }
            
            _renderer.Render(_viewport, _tileSource, _tileCache);
            _invalid = false;
        }

        private static bool TryInitializeViewport(ref Viewport viewport, double actualWidth, double actualHeight, ITileSchema schema)
        {
            if (double.IsNaN(actualWidth)) return false;
            if (actualWidth <= 0) return false;

            var nearestLevel = Utilities.GetNearestLevel(schema.Resolutions, schema.Extent.Width / actualWidth);

            viewport = new Viewport
                {
                    Width = actualWidth,
                    Height = actualHeight,
                    UnitsPerPixel = schema.Resolutions[nearestLevel].UnitsPerPixel,
                    Center = new Samples.Common.Geometries.Point(schema.Extent.CenterX, schema.Extent.CenterY)
                };
            return true;
        }
    }
}
