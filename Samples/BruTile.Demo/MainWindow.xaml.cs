﻿using BruTile.Predefined;
using BruTile.Web;
using System;
using System.Linq;
using System.Net.Http;
using System.Windows.Controls;
using BruTile.Samples.Common;
using BruTile.Wmts;

namespace BruTile.Demo
{
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

            var httpClient = new HttpClient();
            var stream = httpClient.GetStreamAsync("https://bertt.github.io/wmts/capabilities/michelin.xml").Result;
            var michelinTileSource = WmtsParser.Parse(stream).First();
            Layers.Children.Add(ToRadioButton("Michelin Map", () => michelinTileSource));

            Layers.Children.Add(ToRadioButton("Google Map", () => 
                CreateGoogleTileSource("http://mt{s}.google.com/vt/lyrs=m@130&hl=en&x={x}&y={y}&z={z}")));
            Layers.Children.Add(ToRadioButton("Google Terrain", () => 
                CreateGoogleTileSource("http://mt{s}.google.com/vt/lyrs=t@125,r@130&hl=en&x={x}&y={y}&z={z}")));

            Layers.Children.Add(ToRadioButton("WMS called through tile schema", TileSourceForWmsSample.Create));

            Layers.Children.Add(ToRadioButton("Here Maps", () =>
                new HttpTileSource(new GlobalSphericalMercator(0, 18),
                    "https://{s}.base.maps.cit.api.here.com/maptile/2.1/maptile/newest/normal.day/{z}/{x}/{y}/256/png8?app_id=xWVIueSv6JL0aJ5xqTxb&app_code=djPZyynKsbTjIUDOBcHZ2g",
                    new[] { "1", "2", "3", "4" }, name: "Here Maps Source")));

            Layers.Children.Add(ToRadioButton("LM topowebb", LantMaterietTopowebbTileSourceTest.Create));
        }
        
        private static ITileSource CreateGoogleTileSource(string urlFormatter)
        {
            return new HttpTileSource(new GlobalSphericalMercator(), urlFormatter, new[] {"0", "1", "2", "3"}, 
                tileFetcher: FetchGoogleTile); 
        }

        private static byte[] FetchGoogleTile(Uri arg)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "http://maps.google.com/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");
           
            return httpClient.GetByteArrayAsync(arg).ConfigureAwait(false).GetAwaiter().GetResult();
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