# BruTile
BruTile is a .NET Standard 2.0 library to access tile services like OpenStreetMap and Bing. Such tile services store pre-rendered tiles for a certain area and for various levels of detail. BruTile helps to determine which tiles to fetch for a certain viewport of a map. 
BruTile returns tiles as raw image streams and has no dependency on a specific client platform. BruTile does not display those tiles. You need to use a mapping library like SharpMap, ArcBruTile or [Mapsui](https://github.com/Mapsui/Mapsui) or write your own code to display tiles. 

What BruTile does is:

1. Helps to construct a tile source, an object that has all information about a tile service.
2. Helps to calculate which tiles you need, given a certain map extent and a map resolution (units per pixel). 
3. Helps you fetch those tiles.

### BruTile is a .NET Standard library

**BruTile V4** consists of 2 nugets that support .NET Standard 2.0.

| Library                  |   Targeted Framework  |
| ------------------------ | --------------------- |
| BruTile                  |  .NET Standard 2.0    |
| BruTile.MbTiles          |  .NET Standard 2.0    |

- Support for .NET Framework 4.5 has been removed (also the samples and tests have moved to .NET Core). 
- BruTile.Desktop and BruTile.Desktop.DbCache have been deleted and their content has moved to the BruTile nuget.

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

* [ArcBruTile](https://bertt.itch.io/arcbrutile) a plugin for ArcGIS
* [SharpMap](https://github.com/SharpMap/SharpMap) a GIS library
* [Mapsui](https://github.com/Mapsui/Mapsui) a MapComponent for Xamarin.Android. Xamarin.iOS, UWP and WPF
* [DotSpatial](https://github.com/DotSpatial/DotSpatial) a GIS library that is used in [HydroDesktop](https://github.com/CUAHSI/HydroDesktop)
