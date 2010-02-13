using System;
using System.Net;
using System.Windows;
using BruTile.UI.Windows;
using BruTile.Web;
using DemoConfig;

namespace BruTile.UI.Wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            InitializeComponent();
            map.ErrorMessageChanged += new EventHandler(map_ErrorMessageChanged);
            Loaded += new RoutedEventHandler(Window1_Loaded);
            ITileSource tileSource = new ConfigOsm().CreateTileSource();
            map.RootLayer = new TileLayer(tileSource);
            InitializeTransform(tileSource.Schema);
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            map.Refresh();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An Unhandled exception occurred, the application will shut down", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void InitializeTransform(ITileSchema schema)
        {
            map.Transform.Center = new Point(schema.Extent.CenterX, schema.Extent.CenterY);
            map.Transform.Resolution = schema.Resolutions[2];
        }

        private void map_ErrorMessageChanged(object sender, EventArgs e)
        {
            Error.Text = map.ErrorMessage;
            Renderer.AnimateOpacity(errorBorder, 0.75, 0, 8000);
        }

        private void Osm_Click(object sender, RoutedEventArgs e)
        {
            map.RootLayer = new TileLayer(new ConfigOsm().CreateTileSource());
        }

        private void GeodanWms_Click(object sender, RoutedEventArgs e)
        {
            map.RootLayer = new TileLayer(new ConfigWms().CreateTileSource());
        }

        private void GeodanTms_Click(object sender, RoutedEventArgs e)
        {
            map.RootLayer = new TileLayer(new ConfigTms().CreateTileSource());
        }

        private void BingMaps_Click(object sender, RoutedEventArgs e)
        {
            map.RootLayer = new TileLayer(new TileSourceBing(RequestBing.UrlBingStaging, String.Empty, MapType.Aerial));
        }

        private void GeodanWmsC_Click(object sender, RoutedEventArgs e)
        {
            map.RootLayer = new TileLayer(new ConfigWmsC().CreateTileSource());
        }

        private void SharpMap_Click(object sender, RoutedEventArgs e)
        {
            map.RootLayer = new TileLayer(new ConfigSharpMap().CreateTileSource());
        }

        private void MapTiler_Click(object sender, RoutedEventArgs e)
        {
            map.RootLayer = new TileLayer(new ConfigMapTiler().CreateTileSource());
        }

        private void TmsEduGis_Click(object sender, RoutedEventArgs e)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += GetServiceDescriptionCompleted;
            client.OpenReadAsync(new Uri("http://t4.edugis.nl/tiles/Nederland 17e eeuw (Blaeu)/"));
        }

        private void GetServiceDescriptionCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if ((!e.Cancelled) && (e.Error == null))
            {
                map.RootLayer = new TileLayer(new TileSourceTms(e.Result, "http://t4.edugis.nl/tiles/Nederland 17e eeuw (Blaeu)/"));
            }
        }
    }
}

