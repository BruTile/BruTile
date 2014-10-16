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
                var radioButton = new RadioButton {Content = layer.ToString(), Tag = layer};
                radioButton.Click += (sender, args) =>
                    {
                        var knownTileServer = (KnownTileServers)((RadioButton)sender).Tag;
                        if (!knownTileServer.ToString().ToLower().Contains("cloudmade")) // Exclude CloudMade
                            MapControl.SetTileSource(TileSource.Create(knownTileServer));
                    };
                Layers.Children.Add(radioButton);
            }
        }
    }
}
