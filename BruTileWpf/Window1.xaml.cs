using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BruTile;
using DemoConfig;
using BruTileMap;
using BruTileWindows;
using System.IO;

namespace BruTileDemo
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
    }

    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      MessageBox.Show("An Unhandled exception occurred, the application will shut down", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    void map_Loaded(object sender, RoutedEventArgs e)
    {
      InitTransform();
      IConfig config = new ConfigOsm();
      map.RootLayer = new TileLayer<MemoryStream>(new FetchTileWeb(config.RequestBuilder), config.TileSchema, new TileFactory());
      GC.Collect();
      //if you want to use caching to local file system for this layer use this line instead:
      //map.RootLayer = new TileLayer(new WebTileProvider(config.RequestBuilder), config.TileSchema, config.FileCache);

      FpsText.DataContext = map.FpsCounter;
      FpsText.SetBinding(TextBlock.TextProperty, new Binding("Fps"));
    }

    private void InitTransform()
    {
      map.Transform.Center = new PointF(629816, 6805085);
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
      IConfig config = new ConfigOsm();
      map.RootLayer = new TileLayer<MemoryStream>(new FetchTileWeb(config.RequestBuilder), config.TileSchema, new TileFactory());
      
      //todo: move elsewhere
      TileCountText.DataContext = map.RootLayer.MemoryCache;
      TileCountText.SetBinding(TextBlock.TextProperty, new Binding("TileCount"));
    }

    private void GeodanWms_Click(object sender, RoutedEventArgs e)
    {
      IConfig config = new ConfigWms();
      map.RootLayer = new TileLayer<MemoryStream>(new FetchTileWeb(config.RequestBuilder), config.TileSchema, new TileFactory());
      
      //todo: move elsewhere
      TileCountText.DataContext = map.RootLayer.MemoryCache;
      TileCountText.SetBinding(TextBlock.TextProperty, new Binding("TileCount"));
    }

    private void GeodanTms_Click(object sender, RoutedEventArgs e)
    {
      IConfig config = new ConfigTms();
      map.RootLayer = new TileLayer<MemoryStream>(new FetchTileWeb(config.RequestBuilder), config.TileSchema, new TileFactory());
      
      //todo: move elsewhere
      TileCountText.DataContext = map.RootLayer.MemoryCache;
      TileCountText.SetBinding(TextBlock.TextProperty, new Binding("TileCount"));
    }

    private void BingMaps_Click(object sender, RoutedEventArgs e)
    {
      IConfig config = new ConfigVE();
      map.RootLayer = new TileLayer<MemoryStream>(new FetchTileWeb(config.RequestBuilder), config.TileSchema, new TileFactory());
      
      //todo: move elsewhere
      TileCountText.DataContext = map.RootLayer.MemoryCache;
      TileCountText.SetBinding(TextBlock.TextProperty, new Binding("TileCount"));
    }

  }
}
