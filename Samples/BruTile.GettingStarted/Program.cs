// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BruTile.MbTiles;
using BruTile.Predefined;
using BruTile.Web;
using SQLite;

namespace BruTile.GettingStarted;

internal class Program
{
    private static async Task Main()
    {
        // Dear BruTile maintainer,
        // If the code in this file does not compile and needs changes you 
        // also need to update the 'getting started' sample in the wiki.

        // 1) Create a tile source

        // This is an example that creates the OpenStreetMap tile source:
        var tileSource = new HttpTileSource(new GlobalSphericalMercator(0, 18),
            "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
            ["a", "b", "c"], "OSM");

        // 2) Calculate which tiles you need

        // The extent of the visible map changes but lets start with the whole world
        var extent = new Extent(-20037508, -20037508, 20037508, 20037508);
        var screenWidthInPixels = 400; // The width of the map on screen in pixels
        var resolution = extent.Width / screenWidthInPixels;
        var tileInfos = tileSource.Schema.GetTileInfos(extent, resolution);

        // 3) Fetch the tiles from the service

        Console.WriteLine("Show tile info");
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "User-Agent-For-BruTile-GettingStarted-Sample");
        foreach (var tileInfo in tileInfos)
        {
            var tile = await tileSource.GetTileAsync(httpClient, tileInfo, CancellationToken.None);

            Console.WriteLine(
                $"tile col: {tileInfo.Index.Col}, " +
                $"tile row: {tileInfo.Index.Row}, " +
                $"tile level: {tileInfo.Index.Level} , " +
                $"tile size {tile.Length}");
        }

        // 4) Try some of the known tile sources 

        // You can easily create an ITileSource for a number of predefined tile servers
        // with single line statements like:
        var tileSource1 = KnownTileSources.Create(); // The default is OpenStreetMap
        var tileSource2 = KnownTileSources.Create(KnownTileSource.BingAerial);
        var tileSource3 = KnownTileSources.Create(KnownTileSource.BingHybrid);
        var tileSource4 = KnownTileSources.Create(KnownTileSource.StamenTonerLite);
        var _ = KnownTileSources.Create(KnownTileSource.EsriWorldShadedRelief);

        // 6) Use MBTiles, the sqlite format for tile data, to work with tiles stored on your device.

        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString("Resources/world.mbtiles", false));
        var mbTilesTile = await mbTilesTileSource.GetTileAsync(new TileInfo { Index = new TileIndex(0, 0, 0) });
        Console.WriteLine();
        Console.WriteLine("MBTiles");
        Console.WriteLine($"This is a byte array of an image file loaded from MBTiles with size: {mbTilesTile.Length}");
    }
}
