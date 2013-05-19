using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BruTile.Web;

namespace BruTile.Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            foreach (var layer in  Enum.GetValues(typeof(KnownOsmTileServers)).Cast<KnownOsmTileServers>())
            {
                var radioButton = new RadioButton {Content = layer.ToString(), Tag = layer};
                radioButton.Click += (sender, args) => MapControl.SetTileSource(TileSource.Create((KnownOsmTileServers)((RadioButton)sender).Tag));
                Layers.Children.Add(radioButton);
            }
        }
    }
}
