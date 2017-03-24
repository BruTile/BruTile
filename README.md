[![NuGet Status](http://img.shields.io/nuget/v/BruTile.svg?style=flat)](https://www.nuget.org/packages/BruTile/)
[![TeamCity CodeBetter](https://img.shields.io/teamcity/codebetter/bt428.svg)](http://teamcity.codebetter.com/project.html?projectId=BruTile&tab=projectOverview)

### BruTile
BruTile is a C# open source library to access tile services like OpenStreetMap and Bing. BruTile has few dependencies, is platform independent and has a limited scope. It is intended for reuse by other more sophisticated libraries

### Get it from NuGet 
`
PM> Install-Package BruTile
`

### Getting Started

Take a look here on the [wiki](https://github.com/BruTile/BruTile/wiki/Getting-Started-with-BruTile)

### Projects that use BruTile

* [ArcBruTile](https://github.com/arcbrutile/arcbrutile/) a plugin for ArcGIS
* [SharpMap](https://github.com/SharpMap/SharpMap) a GIS library
* [Mapsui](https://github.com/pauldendulk/Mapsui) a slippy map that runs in Silverlight
* [DotSpatial](https://dotspatial.codeplex.com/) a GIS library that is used in [HydroDesktop](https://hydrodesktop.codeplex.com/)
* [PDOK](https://www.pdok.nl/nl/producten/pdok-software/pdok-extensie-voor-arcgis) extensie voor ArcGIS

### Demo
For a demo download the source code and run BruTile.Demo in the Samples folder

### Portable Class Library (PCL)
BruTile is a PCL with Profile111 which targets: .Net Framework 4.5, ASP.NET Core 5.0, Windows 8, Windows Phone 8.1, Xamarin.Android, Xamarin.iOS, Xamarin.iOS (Classic)

### Supported tile service protocols:
* [TMS](https://wiki.osgeo.org/wiki/Tile_Map_Service_Specification)
* [OSM](http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames) (Like TMS with inverted y-axis)
* [WMS-C](https://wiki.osgeo.org/wiki/WMS_Tile_Caching#WMS-C_as_WMS_Profile)
* [WMS](http://www.opengeospatial.org/standards/wms) (tiled requests to a regular WMS - like WMS-C)
* [WMTS](http://www.opengeospatial.org/standards/wmts)
* [ArcGIS Tile Server](http://resources.arcgis.com/en/help/rest/apiref/tile.html)

### Known tile sources:
You can easily create an `ITileSource` for a number of specific tile servers with statements like:

    var tileSource1 = KnownTileSources.Create(KnownTileSource.OpenStreetMap)
    var tileSource2 = KnownTileSources.Create(KnownTileSource.MapQuestAerial)
    var tileSource3 = KnownTileSources.Create(KnownTileSource.BingHybrid)
    var tileSource4 = KnownTileSources.Create(KnownTileSource.StamenTonerLite)
    var tileSource5 = KnownTileSources.Create(KnownTileSource.EsriWorldShadedRelief)

### Roadmap
Here are our future plans: 

* Update automatic TileSchema generation to bring WMTS, WMS, TMS and WMSC in line.
* Rename all classes to get them in line with WMTS types.
* Better samples
* Better documentation
* Release v1

### Warnings
Note, this library is work in progress. It is in BETA.

* At the moment there is no documentation.
* We will introduce breaking changed frequently. We change the API whenever we feel this is an improvement.
* We adopt new technologies relatively fast, dropping support for older frameworks.
* Although I do have a general plan of where to go with this library I do not have the resources to go towards that goal in a systematic way. I add functionality depending on what is needed in the projects I work on.
