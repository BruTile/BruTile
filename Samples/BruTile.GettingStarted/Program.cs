using System;
using BruTile.Predefined;
using BruTile.Web;
// ReSharper disable UnusedVariable

namespace BruTile.GettingStarted
{
    class Program
    {
        static void Main()
        {
            // Dear BruTile maintainer,
            // If the code in this file does not compile and needs changes you 
            // also need to update the 'getting started' sample in the wiki.

            // 1) Create a tile source

            // This is an example that creates the OpenStreetMap tile source:
            var tileSource = new HttpTileSource(new GlobalSphericalMercator(0, 18),
                "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                new[] { "a", "b", "c" }, "OSM");

            // 2) Calculate which tiles you need

            // the extent of the visible map changes but lets start with the whole world
            var extent = new Extent(-20037508, -20037508, 20037508, 20037508);
            var screenWidthInPixels = 400; // The width of the map on screen in pixels
            var resolution = extent.Width / screenWidthInPixels;
            var tileInfos = tileSource.Schema.GetTileInfos(extent, resolution);

            // 3) Fetch the tiles from the service

            foreach (var tileInfo in tileInfos)
            {
                var tile = tileSource.GetTile(tileInfo);

                Console.WriteLine(
                    $"tile col: {tileInfo.Index.Col}, " +
                    $"tile row: {tileInfo.Index.Row}, " +
                    $"tile level: {tileInfo.Index.Level} , " +
                    $"tile size {tile.Length}");
            }

            // 4) Try other tile sources

            // You can easily create an ITileSource for a number of predefined tile servers
            // with single line statements like:
            var tileSource1 = KnownTileSources.Create(); // The default is OpenStreetMap
            var tileSource2 = KnownTileSources.Create(KnownTileSource.BingAerial);
            var tileSource3 = KnownTileSources.Create(KnownTileSource.BingHybrid);
            var tileSource4 = KnownTileSources.Create(KnownTileSource.StamenTonerLite);
            var tileSource5 = KnownTileSources.Create(KnownTileSource.EsriWorldShadedRelief);
        }
    }
}
