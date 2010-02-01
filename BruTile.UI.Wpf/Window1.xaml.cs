using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BruTile.UI.Windows;
using DemoConfig;
using BruTile;
using BruTile.Web;

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
            this.Closing += new System.ComponentModel.CancelEventHandler(Window1_Closing);
            this.map.ErrorMessageChanged += new EventHandler(map_ErrorMessageChanged);
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            FpsText.DataContext = map.FpsCounter;
            FpsText.SetBinding(TextBlock.TextProperty, new Binding("Fps"));

            SetConfig(new ConfigOsm());

            InitTransform();
            
            this.map.Refresh();

        }

        void Window1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (map.RootLayer != null)
            {
                var v = map.RootLayer;
                map.RootLayer = null;
                v.Dispose();
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An Unhandled exception occurred, the application will shut down", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void InitTransform()
        {
            map.Transform.Center = new Point(629816, 6805085);
            map.Transform.Resolution = 1222.992452344;
        }

        private void map_ErrorMessageChanged(object sender, EventArgs e)
        {
            this.Error.Text = map.ErrorMessage;
        }

        private void Osm_Click(object sender, RoutedEventArgs e)
        {
            SetConfig(new ConfigOsm());
        }

        private void GeodanWms_Click(object sender, RoutedEventArgs e)
        {
            SetConfig(new ConfigWms());
        }

        private void GeodanTms_Click(object sender, RoutedEventArgs e)
        {
            TileSourceTms tileSource = new TileSourceTms("http://t4.edugis.nl/tiles/Nederland 17e eeuw (Blaeu)/", "http://t4.edugis.nl/tiles/Nederland 17e eeuw (Blaeu)/");
            map.RootLayer = new TileLayer(tileSource);
        }

        private void BingMaps_Click(object sender, RoutedEventArgs e)
        {
            TileSourceBing tileSource = new TileSourceBing("http://t1.staging.tiles.virtualearth.net/tiles/h");
            map.RootLayer = new TileLayer(tileSource);
        }

        private void SetConfig(IConfig config)
        {
            if (map.RootLayer != null)
            {
                var v = map.RootLayer;
                map.RootLayer = null;
                v.Dispose();
            }

            map.RootLayer = new TileLayer(config.CreateTileSource());

            TileCountText.DataContext = map.RootLayer.MemoryCache;
            TileCountText.SetBinding(TextBlock.TextProperty, new Binding("TileCount"));
        }

        private void GeodanWmsC_Click(object sender, RoutedEventArgs e)
        {
            SetConfig(new ConfigWmsC());
        }

        private void SharpMap_Click(object sender, RoutedEventArgs e)
        {
            SetConfig(new ConfigSharpMap());
        }

        private void MapTiler_Click(object sender, RoutedEventArgs e)
        {
            SetConfig(new ConfigMapTiler());
        }
    }
}

