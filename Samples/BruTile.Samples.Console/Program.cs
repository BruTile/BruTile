using System.Collections.Generic;
using BruTile.Predefined;
using BruTile.Web;

namespace BruTile.Samples.Console
{
    class Program
    {
        static void Main()
        {
            // 1) Create a tile source

            //This is an example of the open street map tile source:
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

            var tiles = new Dictionary<TileInfo, byte[]>();
            foreach (var tileInfo in tileInfos)
            {
                tiles[tileInfo] = tileSource.GetTile(tileInfo);
            }

            // Show that something actually happended:

            foreach (var tile in tiles)
            {
                System.Console.WriteLine("Column: {0}, Row: {1}, level: {2}, bytes: {3}", 
                    tile.Key.Index.Col, tile.Key.Index.Row, tile.Key.Index.Level, tile.Value.Length);
            }

			System.Console.ReadKey();
        }
    }
}
