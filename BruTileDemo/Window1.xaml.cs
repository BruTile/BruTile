using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
      TileCountText.DataContext = map.TileLayer.Bitmaps;
      TileCountText.SetBinding(TextBlock.TextProperty, new Binding("TileCount"));
      FpsText.DataContext = map.FpsCounter;
      FpsText.SetBinding(TextBlock.TextProperty, new Binding("Fps"));
    }

    private void map_ErrorMessageChanged(object sender, EventArgs e)
    {
      this.Error.Text = map.ErrorMessage;
    }

  }
}
