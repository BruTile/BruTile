using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using BruTile;
using BruTileMap;
using BruTileWindows;
using DemoConfig;

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
      ITileSource source = new ConfigOsm().CreateTileSource();
      map.RootLayer = new TileLayer<MemoryStream>(source, new TileFactory());
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
      ITileSource source = new ConfigOsm().CreateTileSource();
      map.RootLayer = new TileLayer<MemoryStream>(source, new TileFactory());
    }

    private void BingMaps_Click(object sender, RoutedEventArgs e)
    {
      ITileSource source = new ConfigVE().CreateTileSource();
      map.RootLayer = new TileLayer<MemoryStream>(source, new TileFactory());
    }

    private void map_ErrorMessageChanged(object sender, EventArgs e)
    {
      //todo: show to user
    }
  }
}
