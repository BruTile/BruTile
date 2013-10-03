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

            foreach (var layer in  Enum.GetValues(typeof(KnownTileServers)).Cast<KnownTileServers>())
            {
                var radioButton = new RadioButton {Content = layer.ToString(), Tag = layer};
                radioButton.Click += (sender, args) =>
                    {
                        var knownTileServer = (KnownTileServers)((RadioButton)sender).Tag;
                        var apiKey = (knownTileServer.ToString().ToLower().Contains("cloudmade")) ? "481143122a9445f4a94aee9c67a5151d" : null;
                        MapControl.SetTileSource(TileSource.Create(knownTileServer, apiKey));
                    };
                Layers.Children.Add(radioButton);
            }
        }
    }
}
