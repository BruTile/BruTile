###BruTile
BruTile is a C# open source library to access tile services like OpenStreetMap and Bing. BruTile has few dependencies, is platform independent and has a limited scope. It is intended for reuse by other more sophisticated libraries

###Projects that use BruTile

* [ArcBruTile] (https://arcbrutile.codeplex.com/) a plugin for ArcGIS
* [SharpMap] (https://sharpmap.codeplex.com/) a GIS library
* [Mapsui] (https://github.com/pauldendulk/Mapsui) a slippy map that runs in Silverlight
* [DotSpatial] (https://dotspatial.codeplex.com/) a GIS library that is used in [HyrdroDesktop] (https://hydrodesktop.codeplex.com/)
* [PDOK] (https://www.pdok.nl/nl/producten/pdok-software/pdok-extensie-voor-arcgis) extensie voor ArcGIS

###Demo
Go [here] (http://brutiledemo.appspot.com/) for an online Silverlight demo of BruTile used in Mapsui


###Compiled as a Portable Class Library (PCL) that targets:
* .NET for Windows Store apps
* .Net Framework 4.0.3 and higher
* Silverlight 5 and higher (upgraded from 4 to 5 necessary for Xamarin support)
* Windows Phone 8 and higher (upgraded from 7 to 8 necessary for Xamarin support)
* Xamarin.iOS
* Xamarin.Android

###For downward compatibility we also compile for:
* .Net Framework 4.0
* .Net Framework 3.5

###Supported tile services
* TMS
* WMS-C
* WMS called though WMS-C protocol
* WMTS
* ArcGIS Tile Server
 

There are also a number of predefined tile services, like:
* OpenStreetMap
* Bing
* Google

###Roadmap
Here are our future plans: 
* Use Resolution (iso TileSchema) dependent TileSize, MatrixSize and Origin in GetTilesInView
* Update automatic TileSchema generation to bring WMTS, WMS, TMS and WMSC in line.
* Rename all classes to get them in line with WMTS types.
* Replace specific classes with generic ones.
* Better samples
* Better documentation
* Release v1

###Warnings
Note, this library is work in progress. It is in BETA.
* At the moment there is no documentation.
* We will introduce breaking changed frequently. We change the API whenever we feel this is an improvement.
* We adopt new technologies relatively fast, dropping support for older frameworks.
* Although I do have a general plan of where to go with this library I do not have the resources to go towards that goal in a systematic way. I add functionality depending on what is needed in the projects I work on.

