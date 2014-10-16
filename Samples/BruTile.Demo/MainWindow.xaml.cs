using BruTile.Web;
using System;
using System.Linq;
using System.Windows.Controls;

namespace BruTile.Demo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            foreach (var layer in  Enum.GetValues(typeof(KnownTileServers)).Cast<KnownTileServers>())
            {
                if (layer.ToString().ToLower().Contains("cloudmade")) continue; // Exclude CloudMade
                
                var radioButton = new RadioButton {Content = layer.ToString(), Tag = layer};
                radioButton.Click += (sender, args) =>
                    {
                        var knownTileServer = (KnownTileServers)((RadioButton)sender).Tag;
                        MapControl.SetTileSource(TileSource.Create(knownTileServer));
                    };
                Layers.Children.Add(radioButton);
            }
        }
    }
}
