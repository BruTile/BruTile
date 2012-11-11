using BruTile.Cache;
using BruTile.Web;
using SharpMap.Fetcher;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BruTile.Metro
{
    public class MapControl : Canvas
    {
        ITileCache<Image> tileCache = new MemoryCache<Image>(200, 300);
        OsmTileSource osmTileSource = new OsmTileSource();
        Fetcher fetcher;
        Viewport viewport;
        Point previousPoint;
        
        public MapControl()
        {
            fetcher = new Fetcher(osmTileSource, tileCache);
            fetcher.DataChanged += fetcher_DataChanged;
              
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            SizeChanged += MapControl_SizeChanged;
            this.PointerMoved += MapControl_PointerMoved;
            this.PointerPressed += MapControl_PointerPressed;
            this.PointerReleased += MapControl_PointerReleased;
        }
               
        void MapControl_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (previousPoint == default(Point)) return;
           
            var position = e.GetCurrentPoint(this).Position;

            var currentWorld = viewport.ViewToWorld(position.X, position.Y);
            var previousWorld = viewport.ViewToWorld(previousPoint.X, previousPoint.Y); 
            var newX = viewport.CenterX + (previousWorld.X - currentWorld.X);
            var newY = viewport.CenterY + (previousWorld.Y - currentWorld.Y);
            
            viewport.Center = new Point(newX, newY);
            previousPoint = position;
            fetcher.ViewChanged(viewport.Extent, viewport.Resolution);
            //InvalidateArrange();
        }

        void MapControl_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            previousPoint = e.GetCurrentPoint(this).Position;
        }

        void MapControl_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            previousPoint = default(Point);
        }

        async void fetcher_DataChanged(object sender, DataChangedEventArgs e)
        {
            if (!Dispatcher.HasThreadAccess)
                 Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Low,
                    () => fetcher_DataChanged(sender, e));
            else
            {
                if (e.Error == null && e.Image != null)
                {
                    var feature = new Image()
                    {
                        Source = await BruTile.Metro.Utilities.TileToImage(e.Image)
                    };
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
            InvalidateArrange();
        }

        void CompositionTarget_Rendering(object sender, object e)
        {
            Renderer.Render(viewport, this, osmTileSource, tileCache);
        }

        private static bool CanInitializeViewport(double actualWidth, double actualHeight)
        {
            return (!double.IsNaN(actualWidth) && actualWidth > 0
                && !double.IsNaN(actualHeight) && actualHeight > 0); 
        }

        private static Viewport InitializeViewport(double actualWidth, double actualHeight, Extent extent)
        {
            var viewport = new Viewport();
            viewport.Width = actualWidth;
            viewport.Height = actualHeight;
            viewport.Resolution = extent.Width / actualWidth;
            viewport.Center = new Point(extent.CenterX, extent.CenterY);
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

        internal void Center(double x, double y)
        {
            viewport.Center = viewport.ViewToWorld(x, y);
            fetcher.ViewChanged(viewport.Extent, viewport.Resolution);
        }
    }
}
