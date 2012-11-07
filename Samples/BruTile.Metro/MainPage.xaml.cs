using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BruTile.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BruTile.Metro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool first = true;
        Viewport viewport;           

        public MainPage()
        {
            this.InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        void Button_Click_Zoomin(object sender, RoutedEventArgs e)
        {
            viewport.Resolution = viewport.Resolution / 2;
        }

        void Button_Click_Zoomout(object sender, RoutedEventArgs e)
        {
            viewport.Resolution = viewport.Resolution * 2;
        }

        async void CompositionTarget_Rendering(object sender, object e)
        {
            //if (!first) return;
            
            var osmTileSource = new OsmTileSource();
            if (viewport == null)
            {
                viewport = TryInitializeViewport(viewport, ActualWidth, ActualHeight, osmTileSource.Schema);
                if (viewport == null) return;
            }
            if (viewport == null) return;
            var level = Utilities.GetNearestLevel(osmTileSource.Schema.Resolutions, viewport.Resolution);
            var tileInfos = osmTileSource.Schema.GetTilesInView(viewport.Extent, level);

            foreach (var tileInfo in tileInfos)
            {
                var tile = osmTileSource.Provider.GetTile(tileInfo);
                var image = new Image();
                image.Source = await TileToImage(tile);
                var point1 = viewport.WorldToView(tileInfo.Extent.MinX, tileInfo.Extent.MaxY);
                var point2 = viewport.WorldToView(tileInfo.Extent.MaxX, tileInfo.Extent.MinY);

                Canvas.SetLeft(image, point1.X);
                Canvas.SetTop(image, point1.Y);
                image.Width = point2.X - point1.X;
                image.Height = point2.Y - point1.Y;
                canvas.Children.Add(image);                
            }

            first = false;
        }

        private async Task<BitmapImage> TileToImage(byte[] tile)
        {
            try
            {
                var ims = await ByteArrayToRandomAccessStream(tile);
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(ims);
               return bitmapImage;
            }
            catch (Exception ex)
            {
                var stromg = ex.Message;
                return null;
            }
        }

        private static async Task<InMemoryRandomAccessStream> ByteArrayToRandomAccessStream(byte[] tile)
        {
            var stream = new InMemoryRandomAccessStream();
            DataWriter dataWriter = new DataWriter(stream);
            dataWriter.WriteBytes(tile);
            await dataWriter.StoreAsync();
            stream.Seek(0);
            return stream;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private static Viewport TryInitializeViewport(Viewport viewport, double actualWidth, double actualHeight, ITileSchema schema)
        {
            if (double.IsNaN(actualWidth)) return null;
            if (actualWidth <= 0) return null;
            viewport = new Viewport();
            viewport.Width = actualWidth;
            viewport.Height = actualHeight;
            viewport.Resolution = schema.Extent.Width / actualWidth;
            viewport.Center = new Point(schema.Extent.CenterX, schema.Extent.CenterY);
            return viewport;
        }

        private void canvas_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            var x = e.GetCurrentPoint(canvas).Position.X;
            var y = e.GetCurrentPoint(canvas).Position.Y;

        }
    }
}
