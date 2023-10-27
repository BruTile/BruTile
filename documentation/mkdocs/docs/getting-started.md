# Getting Started

### 1) Create an app and add the BruTile NuGet package
Create a .NET Console app in Visual Studio. The the BruTile NuGet package. Use the package manager tools in Visual Studio or add it from the package manager console:
```
PM> install-package BruTile 
```

### 2) Create a tile source
```c#
// This is an example that creates the OpenStreetMap tile source:
var tileSource = new HttpTileSource(new GlobalSphericalMercator(0, 18),
    "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
    new[] {"a", "b", "c"}, "OSM");
```
### 3) Calculate which tiles you need
```c#
// the extent of the visible map changes but lets start with the whole world
var extent = new Extent(-20037508, -20037508, 20037508, 20037508);
var screenWidthInPixels = 400; // The width of the map on screen in pixels
var resolution = extent.Width / screenWidthInPixels;
var tileInfos = tileSource.Schema.GetTileInfos(extent, resolution);
```

### 4) Fetch the tiles from the service

```c#
Console.WriteLine("Show tile info");
foreach (var tileInfo in tileInfos)
{
    var tile = await tileSource.GetTileAsync(tileInfo);

    Console.WriteLine(
        $"tile col: {tileInfo.Index.Col}, " +
        $"tile row: {tileInfo.Index.Row}, " +
        $"tile level: {tileInfo.Index.Level} , " +
        $"tile size {tile.Length}");
}
```
Output:
```console
    tile col: 0, tile row: 0, tile level: 1 , tile size 11276
    tile col: 0, tile row: 1, tile level: 1 , tile size 3260
    tile col: 1, tile row: 0, tile level: 1 , tile size 10679
    tile col: 1, tile row: 1, tile level: 1 , tile size 4009
```

### 5) Try some of the known tile sources 

```c#
// You can easily create an ITileSource for a number of predefined tile servers
// with single line statements like:
var tileSource1 = KnownTileSources.Create(); // The default is OpenStreetMap
var tileSource2 = KnownTileSources.Create(KnownTileSource.BingAerial);
var tileSource3 = KnownTileSources.Create(KnownTileSource.BingHybrid);
var tileSource4 = KnownTileSources.Create(KnownTileSource.StamenTonerLite);
var tileSource5 = KnownTileSources.Create(KnownTileSource.EsriWorldShadedRelief);
```
The predefined tile sources are defined in a single file. Take a look at that file [here](https://github.com/BruTile/BruTile/blob/master/BruTile/Predefined/KnownTileSources.cs) to learn how you could create any tile source.


### 6) Use MBTiles, the sqlite format for tile data, to work with tiles stored on your device.

```c#
var mbtilesTilesource = new MbTilesTileSource(new SQLiteConnectionString("Resources/world.mbtiles", false));
var mbTilesTile = await mbtilesTilesource.GetTileAsync(new TileInfo { Index = new TileIndex(0, 0, 0) });
Console.WriteLine($"This is a byte array of an image file loaded from MBTiles with size: {mbTilesTile.Length}");
```
Output:
```console
This is a byte array of an image file loaded from MBTiles with size: 27412
```

The above code can also be found in the BruTile sample called BruTile.GettingStarted in the Samples folder of this repository.
