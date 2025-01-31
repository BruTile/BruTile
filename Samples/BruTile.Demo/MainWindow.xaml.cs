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

        foreach (var knownTileSource in GetKnownTileSourceValues())
        {
            var httpTileSource = KnownTileSources.Create(knownTileSource);
            Layers.Children.Add(ToRadioButton(knownTileSource.ToString(), () => httpTileSource));
        }

        Layers.Children.Add(ToRadioButton("Michelin WMTS", MichelinWmtsSample.Create));
        Layers.Children.Add(ToRadioButton("Google Maps", GoogleMapsSample.Create));
        Layers.Children.Add(ToRadioButton("Google Terrain", GoogleMapsTerrainSample.Create));
        Layers.Children.Add(ToRadioButton("WMS called through tile schema", TileSourceForWmsSample.Create));
        Layers.Children.Add(ToRadioButton("Here Maps", HereMapsSample.Create));
        Layers.Children.Add(ToRadioButton("Lantmateriet Topowebb", () => LantmaterietTopowebbSample.CreateAsync().Result));
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

    private static KnownTileSource[] GetKnownTileSourceValues()
    {
        return
        [
            KnownTileSource.OpenStreetMap,
            KnownTileSource.OpenCycleMap,
            KnownTileSource.OpenCycleMapTransport,
            KnownTileSource.BingAerial,
            KnownTileSource.BingHybrid,
            KnownTileSource.BingRoads,
            KnownTileSource.BingAerialStaging,
            KnownTileSource.BingHybridStaging,
            KnownTileSource.BingRoadsStaging,
            KnownTileSource.StamenToner,
            KnownTileSource.StamenTonerLite,
            KnownTileSource.StamenWatercolor,
            KnownTileSource.StamenTerrain,
            KnownTileSource.EsriWorldTopo,
            KnownTileSource.EsriWorldPhysical,
            KnownTileSource.EsriWorldShadedRelief,
            KnownTileSource.EsriWorldReferenceOverlay,
            KnownTileSource.EsriWorldTransportation,
            KnownTileSource.EsriWorldBoundariesAndPlaces,
            KnownTileSource.EsriWorldDarkGrayBase,
            KnownTileSource.BKGTopPlusColor,
            KnownTileSource.BKGTopPlusGrey,
            KnownTileSource.HereNormal,
            KnownTileSource.HereSatellite,
            KnownTileSource.HereHybrid,
            KnownTileSource.HereTerrain
        ];
    }
}
