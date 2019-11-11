|   | Status  | 
| ------------- |:-------------:|
| Build | [![Build status](https://ci.appveyor.com/api/projects/status/5s4poobpfab9g8ny?svg=true)](https://ci.appveyor.com/project/pauldendulk/brutile) |
| NuGet | [![NuGet Status](http://img.shields.io/nuget/v/BruTile.svg?style=flat)](https://www.nuget.org/packages/BruTile/) |

### BruTile
BruTile is a .NET Standard 1.1 library to access tile services like OpenStreetMap and Bing. Such tile services store pre-rendered tiles for a certain area and for various levels of detail. BruTile helps to determine which tiles to fetch for a certain viewport of a map. 
BruTile returns tiles as raw image streams and has no dependency on a specific client platform. BruTile does not display those tiles. You need to use a mapping library like SharpMap, ArcBruTile or [Mapsui](https://github.com/Mapsui/Mapsui) or write your own code to display tiles. 

What BruTile does is:

1. Helps to construct a tile source, an object that has all information about a tile service.
2. Helps to calculate which tiles you need, given a certain map extent and a map resolution (units per pixel). 
3. Helps you fetch those tiles.

### BruTile is a .NET Standard library
BruTile 2.0 supports .NET Standard. The Profiles by NuGet package:

| Library                  |   Targeted Framework  |
| ------------------------ | --------------------- |
| BruTile                  |  .NET Standard 1.1    |
| BruTile.MbTiles          |  .NET Standard 1.1    |
| BruTile.Desktop          |  .NET Standard 1.6    |
| BruTile.Desktop.DbCache  |  .NET Standard 2.0    |

All the above libraries additionally target .Net Framework 4.5

### Demo
For a demo showing various data sources download the source code and run BruTile.Demo in the Samples folder

## Getting Started

[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/bertt/brutile/patch-1)

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
foreach (var tileInfo in tileInfos)
{
    var tile = tileSource.GetTile(tileInfo);

    Console.WriteLine(
        $"tile col: {tileInfo.Index.Col}, " +
        $"tile row: {tileInfo.Index.Row}, " +
        $"tile level: {tileInfo.Index.Level} , " +
        $"tile size {tile.Length}");
}
```
This will be the output:

    tile col: 0, tile row: 0, tile level: 1 , tile size 11276
    tile col: 0, tile row: 1, tile level: 1 , tile size 3260
    tile col: 1, tile row: 0, tile level: 1 , tile size 10679
    tile col: 1, tile row: 1, tile level: 1 , tile size 4009

### 5) Try other tile sources

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

The above code can also be found in the BruTile sample called BruTile.GettingStarted in the Samples folder of this repository.

### Supported tile service protocols:
* [WMTS](http://www.opengeospatial.org/standards/wmts)
* [TMS](https://wiki.osgeo.org/wiki/Tile_Map_Service_Specification)
* [OSM](http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames) (Like TMS with inverted y-axis)
* [WMS](http://www.opengeospatial.org/standards/wms) (tiled requests to a regular WMS)
* [ArcGIS Tile Server](http://resources.arcgis.com/en/help/rest/apiref/tile.html)
* [WMS-C](https://wiki.osgeo.org/wiki/WMS_Tile_Caching#WMS-C_as_WMS_Profile)

### Roadmap
- Stability is currently our primary focus.

### Projects that use BruTile

* [ArcBruTile](https://github.com/arcbrutile/arcbrutile/) a plugin for ArcGIS
* [SharpMap](https://github.com/SharpMap/SharpMap) a GIS library
* [Mapsui](https://github.com/pauldendulk/Mapsui) a MapComponent for Xamarin.Android. Xamarin.iOS, UWP and WPF
* [DotSpatial](https://dotspatial.codeplex.com/) a GIS library that is used in [HydroDesktop](https://hydrodesktop.codeplex.com/)
* [PDOK](https://www.pdok.nl/nl/producten/pdok-software/pdok-extensie-voor-arcgis) extensie voor ArcGIS

### License
[Apache 2.0](https://raw.githubusercontent.com/BruTile/BruTile/master/LICENSE.md)
