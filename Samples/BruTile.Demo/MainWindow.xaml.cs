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

            foreach (var knownTileServer in  Enum.GetValues(typeof(KnownTileServers)).Cast<KnownTileServers>())
            {
                if (knownTileServer.ToString().ToLower().Contains("cloudmade")) continue; // Exclude CloudMade

                KnownTileServers server = knownTileServer;
                var radioButton = ToRadioButton(knownTileServer.ToString(), () => TileSource.Create(server));
                Layers.Children.Add(radioButton);
            }

            Layers.Children.Add(ToRadioButton("Google Map", () => new GoogleTileSource(GoogleMapType.GoogleMap)));
            Layers.Children.Add(ToRadioButton("Google Satellite", () => new GoogleTileSource(GoogleMapType.GoogleSatellite)));
            Layers.Children.Add(ToRadioButton("Google Labels", () => new GoogleTileSource(GoogleMapType.GoogleLabels)));
            Layers.Children.Add(ToRadioButton("Google Terrain", () => new GoogleTileSource(GoogleMapType.GoogleTerrain)));
        }

        private RadioButton ToRadioButton(string name, Func<ITileSource> func)
        {
            var radioButton = new RadioButton
            {
                Content = name,
                Tag = new Func<ITileSource>(func)
            };
            radioButton.Click += (sender, args) => MapControl.SetTileSource(((Func<ITileSource>)((RadioButton)sender).Tag)());

            return radioButton;
        }
    }
}
