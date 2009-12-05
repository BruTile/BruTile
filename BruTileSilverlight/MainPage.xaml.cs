using System;
using System.Windows;
using System.Windows.Controls;
using BruTileMap;
using BruTileWindows;
using DemoConfig;
using System.Windows.Media.Imaging;
using System.IO;

namespace BruTileSilverlight
{
  public partial class MainPage : UserControl
  {
    public MainPage()
    {
      InitializeComponent();
      this.map.Loaded += new RoutedEventHandler(map_Loaded);
    }

    void map_Loaded(object sender, RoutedEventArgs e)
    {
      InitTransform();
      ITileSource config = new ConfigOsm();
      map.RootLayer = new TileLayer<MemoryStream>(config.TileProvider, config.TileSchema, new TileFactory());
    }

    private void InitTransform()
    {
      map.Transform.Center = new BTPoint(629816, 6805085);
      map.Transform.Resolution = 1222.992452344;
      map.Transform.Width = (float)this.Width;
      map.Transform.Height = (float)this.Height;
    }

    private void Osm_Click(object sender, RoutedEventArgs e)
    {
      ITileSource config = new ConfigOsm();
      map.RootLayer = new TileLayer<MemoryStream>(config.TileProvider, config.TileSchema, new TileFactory());
    }

    private void BingMaps_Click(object sender, RoutedEventArgs e)
    {
      ITileSource config = new ConfigVE();
      map.RootLayer = new TileLayer<MemoryStream>(config.TileProvider, config.TileSchema, new TileFactory());
    }

    private void map_ErrorMessageChanged(object sender, EventArgs e)
    {
      //todo: show to user
    }
  }
}
