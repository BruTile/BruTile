using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BruTile.Predefined;
using BruTile.Samples.Common.Samples;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BruTile.Demo.Uno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            foreach (var knownTileSource in Enum.GetValues(typeof(KnownTileSource)).Cast<KnownTileSource>())
            {
                var httpTileSource = KnownTileSources.Create(knownTileSource);
                Layers.Children.Add(ToRadioButton(knownTileSource.ToString(), () => httpTileSource));
            }

            Layers.Children.Add(ToRadioButton("Michelin WMTS", MichelinWmtsSample.Create));
            Layers.Children.Add(ToRadioButton("Google Maps", GoogleMapsSample.Create));
            Layers.Children.Add(ToRadioButton("Google Terrain", GoogleMapsTerrainSample.Create));
            Layers.Children.Add(ToRadioButton("WMS called through tile schema", TileSourceForWmsSample.Create));
            Layers.Children.Add(ToRadioButton("Here Maps", HereMapsSample.Create));
            Layers.Children.Add(ToRadioButton("Lant Materiet Topowebb", LantMaterietTopowebbSample.Create));
            Layers.Children.Add(ToRadioButton("World MbTiles", MbTilesSample.Create));
        }

        private RadioButton ToRadioButton(string name, Func<ITileSource> func)
        {
            var radioButton = new RadioButton
            {
                Content = new TextBlock
                {
                    Text = name,
                    Foreground = new SolidColorBrush(Color.FromArgb(255,0,0,0)),
                },
                Tag = func,
                IsEnabled = true,
            };
            radioButton.Click += OnClick;
            return radioButton;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                if (radioButton.Tag is Func<ITileSource> func)
                {
                    var tileSource = func();
                    this.MapControl.SetTileSource(tileSource);
                }
            }
        }
    }
}
