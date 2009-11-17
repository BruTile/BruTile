using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BruTileMap;
using BruTileWindows;
using DemoConfig;

namespace BruTileWpf
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
            this.map.Loaded += new RoutedEventHandler(map_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(Window1_Closing);
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

        void map_Loaded(object sender, RoutedEventArgs e)
        {
            InitTransform();
            this.SetConfig(new ConfigOsm());

            FpsText.DataContext = map.FpsCounter;
            FpsText.SetBinding(TextBlock.TextProperty, new Binding("Fps"));
        }

        private void InitTransform()
        {
            map.Transform.Center = new BTPoint(629816, 6805085);
            map.Transform.Resolution = 1222.992452344;
            map.Transform.Width = (float)this.Width;
            map.Transform.Height = (float)this.Height;
        }

        private void map_ErrorMessageChanged(object sender, EventArgs e)
        {
            this.Error.Text = map.ErrorMessage;
        }

        private void Osm_Click(object sender, RoutedEventArgs e)
        {
            this.SetConfig(new ConfigOsm());
        }

        private void GeodanWms_Click(object sender, RoutedEventArgs e)
        {
            this.SetConfig(new ConfigWms());
        }

        private void GeodanTms_Click(object sender, RoutedEventArgs e)
        {
            this.SetConfig(new ConfigTms());
        }

        private void BingMaps_Click(object sender, RoutedEventArgs e)
        {
            this.SetConfig(new ConfigVE());
        }

        private void SetConfig(IConfig config)
        {
            if (map.RootLayer != null)
            {
                var v = map.RootLayer;
                map.RootLayer = null;
                v.Dispose();
            }

            map.RootLayer = new TileLayer<MemoryStream>(config.TileProvider, config.TileSchema, new TileFactory());

            //todo: move elsewhere
            TileCountText.DataContext = map.RootLayer.MemoryCache;
            TileCountText.SetBinding(TextBlock.TextProperty, new Binding("TileCount"));
        }

        private void GeodanWmsC_Click(object sender, RoutedEventArgs e)
        {
            this.SetConfig(new ConfigWmsC());
        }

        private void SharpMap_Click(object sender, RoutedEventArgs e)
        {
            this.SetConfig(new ConfigSharpMap());
        }

        private void MapTiler_Click(object sender, RoutedEventArgs e)
        {
            this.SetConfig(new ConfigMapTiler());
        }
    }
}

