// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Windows.Controls;
using BruTile.Predefined;
using BruTile.Samples.Common.Samples;

namespace BruTile.Demo;

public partial class MainWindow
{
    public MainWindow()
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
        Layers.Children.Add(ToRadioButton("Lant Materiet Topowebb", () => LantMaterietTopowebbSample.CreateAsync().Result));
        Layers.Children.Add(ToRadioButton("World MbTiles", MbTilesSample.Create));
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
