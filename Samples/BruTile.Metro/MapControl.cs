using BruTile.Cache;
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
        ITileCache<Image> tileCache = new MemoryCache<Image>(200, 300);
        OsmTileSource osmTileSource = new OsmTileSource();
        BruTile.Samples.Common.Fetcher<Image> fetcher;
        BruTile.Samples.Common.Viewport viewport;
        Windows.Foundation.Point previousPosition;        

        public MapControl()
        {
            fetcher = new BruTile.Samples.Common.Fetcher<Image>(osmTileSource, tileCache);
            fetcher.DataChanged += fetcher_DataChanged;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            SizeChanged += MapControl_SizeChanged;

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
            if (previousPosition == default(Point) || double.IsNaN(previousPosition.X))
            {
                previousPosition = e.Position;
                return;
            }

            if (Distance(e.Position.X, e.Position.Y, previousPosition.X, previousPosition.Y) > 50)
            {
                previousPosition = default(Point);
                return;
            }
            PanAndZoom(e.Position.X, e.Position.Y, e.Delta.Scale);
            
            previousPosition = e.Position;
            fetcher.ViewChanged(viewport.Extent, viewport.Resolution);
        }

        private void PanAndZoom(double positionX, double positionY, double deltaScale)
        {
            var previous = viewport.ScreenToWorld(previousPosition.X, previousPosition.Y);
            var current = viewport.ScreenToWorld(positionX, positionY);
            var diffX = previous.X - current.X;
            var diffY = previous.Y - current.Y;
            var newX = viewport.CenterX + diffX;
            var newY = viewport.CenterY + diffY;
            var zoomCorrectionX = (1 - deltaScale) * (current.X - viewport.CenterX);
            var zoomCorrectionY = (1 - deltaScale) * (current.Y - viewport.CenterY);
            viewport.Resolution = viewport.Resolution / deltaScale;
            
            viewport.Center = new BruTile.Samples.Common.Point(newX - zoomCorrectionX, newY - zoomCorrectionY);
        }
        
        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2.0) + Math.Pow(y1 - y2, 2.0));
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            previousPosition = default(Point);
        }

        async void fetcher_DataChanged(object sender, BruTile.Samples.Common.DataChangedEventArgs e)
        {
            if (!Dispatcher.HasThreadAccess)
                Dispatcher.RunAsync(
                   Windows.UI.Core.CoreDispatcherPriority.High, 
                   () => fetcher_DataChanged(sender, e));
            else
            {
                if (e.Error == null && e.Image != null)
                {
                    var feature = new Image()
                    {
                        Source = await BruTile.Metro.Utilities.TileToImage(e.Image),
                    };
                    feature.Stretch = Stretch.Fill;
                    feature.IsHitTestVisible = false;
                    tileCache.Add(e.TileInfo.Index, feature);
                    InvalidateArrange();
                }
            }
        }

        void MapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (viewport == null)
            {
                if (CanInitializeViewport(ActualWidth, ActualHeight))
                {
                    viewport = InitializeViewport(ActualWidth, ActualHeight, FullMapExtent);
                    fetcher.ViewChanged(viewport.Extent, viewport.Resolution);
                }
            }
            else
            {
                viewport.Width = e.NewSize.Width;
                viewport.Height = e.NewSize.Height;
                fetcher.ViewChanged(viewport.Extent, viewport.Resolution);
            }
            InvalidateArrange();
        }

        void CompositionTarget_Rendering(object sender, object e)
        {
            var tilesToRender = TileLayer.GetTilesInView(viewport.Extent, viewport.Resolution,
                osmTileSource.Schema, tileCache);
            Renderer.Render(viewport, this, tilesToRender);
        }

        private static bool CanInitializeViewport(double actualWidth, double actualHeight)
        {
            return (!double.IsNaN(actualWidth) && actualWidth > 0
                && !double.IsNaN(actualHeight) && actualHeight > 0);
        }

        private static BruTile.Samples.Common.Viewport InitializeViewport(double actualWidth, double actualHeight, Extent extent)
        {
            var viewport = new BruTile.Samples.Common.Viewport();
            viewport.Width = actualWidth;
            viewport.Height = actualHeight;
            viewport.Resolution = extent.Width / actualWidth;
            viewport.Center = new BruTile.Samples.Common.Point(extent.CenterX, extent.CenterY);
            return viewport;
        }

        public Extent FullMapExtent
        {
            get { return osmTileSource.Extent; }
        }

        internal void ZoomInOneStep()
        {
            viewport.Resolution = viewport.Resolution / 2;
            fetcher.ViewChanged(viewport.Extent, viewport.Resolution);
        }

        internal void ZoomOutOneStep()
        {
            viewport.Resolution = viewport.Resolution * 2;
            fetcher.ViewChanged(viewport.Extent, viewport.Resolution);
        }
    }
}
