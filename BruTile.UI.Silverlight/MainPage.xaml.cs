using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using BruTile;
using BruTile.UI;
using BruTile.UI.Windows;
using DemoConfig;
using BruTile.Web;
using System.Net;

namespace BruTile.UI.Silverlight
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            this.map.Loaded += new RoutedEventHandler(map_Loaded);
            App.Current.Host.Content.Resized += new EventHandler(Content_Resized);
        }

        void Content_Resized(object sender, EventArgs e)
        {
            this.Width = App.Current.Host.Content.ActualWidth;
            this.Height = App.Current.Host.Content.ActualHeight;
        }

        void map_Loaded(object sender, RoutedEventArgs e)
        {
            InitTransform();
            ITileSource source = new ConfigOsm().CreateTileSource();
            map.RootLayer = new TileLayer(source);
            GUI.SetMap(map);
        }

        private void InitTransform()
        {
            map.Transform.Center = new Point(629816, 6805085);
            map.Transform.Resolution = 1222.992452344;
        }

        private void Osm_Click(object sender, RoutedEventArgs e)
        {
            ITileSource source = new ConfigOsm().CreateTileSource();
            map.RootLayer = new TileLayer(source);
        }

        private void BingMaps_Click(object sender, RoutedEventArgs e)
        {
            TileSourceBing tileSource = new TileSourceBing("http://t1.staging.tiles.virtualearth.net/tiles/h");
            map.RootLayer = new TileLayer(tileSource);
        }

        private void map_ErrorMessageChanged(object sender, EventArgs e)
        {
            //todo: show to user
        }
    }
}
