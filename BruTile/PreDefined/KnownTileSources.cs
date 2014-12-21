﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using BruTile.Cache;
using BruTile.Web;

namespace BruTile.Predefined
{
    /// <summary>
    /// Known tile sources
    /// </summary>
    public enum KnownTileSource
    {
        OpenStreetMap,
        OpenCycleMap,
        OpenCycleMapTransport,
        CloudMadeWebStyle,
        CloudMadeFineLineStyle,
        CloudMadeNoNames,
        MapQuest,
        MapQuestAerial,
        MapQuestRoadsAndLabels,
        BingAerial,
        BingHybrid,
        BingRoads,
        BingAerialStaging,
        BingHybridStaging,
        BingRoadsStaging,
        StamenToner,
        StamenTonerLite,
        StamenWatercolor,
        StamenTerrain,
        EsriWorldTopo,
        EsriWorldPhysical,
        EsriWorldShadedRelief,
        EsriWorldReferenceOverlay,
        EsriWorldTransportation,
        EsriWorldBoundariesAndPlaces,
        EsriWorldDarkGrayBase
    }

    public static class KnownTileSources
    {
        /// <summary>
        /// Static factory method for known tile services
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="apiKey">An (optional) API key</param>
        /// <param name="persistentCache">A place to permanently store tiles (file of database)</param>
        /// <param name="tileFetcher">Option to override the web request</param>
        /// <returns>The tile source</returns>
        public static ITileSource Create(KnownTileSource source = KnownTileSource.OpenStreetMap, string apiKey = null,
            IPersistentCache<byte[]> persistentCache = null, Func<Uri, byte[]> tileFetcher = null)
        {
            switch (source)
            {
                case KnownTileSource.OpenStreetMap:
                    return new HttpTileSource(new GlobalSphericalMercator(0, 18),
                        "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", 
                        new[] {"a", "b", "c"}, name: source.ToString(), 
                        persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.OpenCycleMap:
                    return new HttpTileSource(new GlobalSphericalMercator(0, 17), 
                        "http://{s}.tile.opencyclemap.org/cycle/{z}/{x}/{y}.png", 
                        new[] {"a", "b", "c"}, name: source.ToString(), 
                        persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.OpenCycleMapTransport:
                    return new HttpTileSource(new GlobalSphericalMercator(0, 20), 
                        "http://{s}.tile2.opencyclemap.org/transport/{z}/{x}/{y}.png", 
                         new[] {"a", "b", "c"}, name: source.ToString(), 
                         persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.CloudMadeWebStyle:
                    return new HttpTileSource(new GlobalSphericalMercator(), 
                        "http://{s}.tile.cloudmade.com/{k}/1/256/{z}/{x}/{y}.png", 
                        new[] {"a", "b", "c"}, apiKey, source.ToString(), persistentCache, tileFetcher);
                case KnownTileSource.CloudMadeFineLineStyle:
                    return new HttpTileSource(new GlobalSphericalMercator(), 
                        "http://{s}.tile.cloudmade.com/{k}/2/256/{z}/{x}/{y}.png",
                        new[] {"a", "b", "c"}, apiKey, source.ToString(), persistentCache, tileFetcher);
                case KnownTileSource.CloudMadeNoNames:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://{s}.tile.cloudmade.com/{k}/3/256/{z}/{x}/{y}.png",
                        new[] {"a", "b", "c"}, apiKey, source.ToString(), persistentCache, tileFetcher);
                case KnownTileSource.MapQuest:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://otile{s}.mqcdn.com/tiles/1.0.0/osm/{z}/{x}/{y}.png", 
                        new[] {"1", "2", "3", "4"}, name: source.ToString(), 
                        persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.MapQuestAerial:
                    return new HttpTileSource(new GlobalSphericalMercator(0, 11),
                        "http://otile{s}.mqcdn.com/tiles/1.0.0/sat/{z}/{x}/{y}.png", 
                        new[] {"1", "2", "3", "4"}, name: source.ToString(), 
                        persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.MapQuestRoadsAndLabels:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://otile{s}.mqcdn.com/tiles/1.0.0/map/{z}/{x}/{y}.png", 
                        new[] {"1", "2", "3", "4"}, name: source.ToString(), 
                        persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.BingAerial:
                    return new HttpTileSource(new GlobalSphericalMercator(1),
                        "http://t{s}.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g=517&token={k}", 
                        new [] { "0", "1", "2", "3", "4", "5", "6", "7" }, apiKey, source.ToString(), 
                        persistentCache, tileFetcher);
                case KnownTileSource.BingHybrid:
                    return new HttpTileSource(new GlobalSphericalMercator(1),
                        "http://t{s}.tiles.virtualearth.net/tiles/h{quadkey}.jpeg?g=517&token={k}", 
                        new [] { "0", "1", "2", "3", "4", "5", "6", "7" }, apiKey, source.ToString(), 
                        persistentCache, tileFetcher);
                case KnownTileSource.BingRoads:
                    return new HttpTileSource(new GlobalSphericalMercator(1),
                        "http://t{s}.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g=517&token={k}",
                        new[] { "0", "1", "2", "3", "4", "5", "6", "7" }, apiKey, source.ToString(), 
                        persistentCache, tileFetcher);
                case KnownTileSource.BingAerialStaging:
                    return new HttpTileSource(new GlobalSphericalMercator(1),
                        "http://t{s}.staging.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g=517&token={k}", 
                        new [] { "0", "1", "2", "3", "4", "5", "6", "7" }, apiKey, source.ToString(), 
                        persistentCache, tileFetcher);
                case KnownTileSource.BingHybridStaging:
                    return new HttpTileSource(new GlobalSphericalMercator(1),
                        "http://t{s}.staging.tiles.virtualearth.net/tiles/h{quadkey}.jpeg?g=517&token={k}", 
                        new [] { "0", "1", "2", "3", "4", "5", "6", "7" }, apiKey, source.ToString(), 
                        persistentCache, tileFetcher);
                case KnownTileSource.BingRoadsStaging:
                    return new HttpTileSource(new GlobalSphericalMercator(1),
                        "http://t{s}.staging.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g=517&token={k}",
                        new[] { "0", "1", "2", "3", "4", "5", "6", "7" }, apiKey, source.ToString(), 
                        persistentCache, tileFetcher);
                case KnownTileSource.StamenToner:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://{s}.tile.stamen.com/toner/{z}/{x}/{y}.png", 
                        new[] {"a", "b", "c", "d"}, name: source.ToString(), 
                        persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.StamenTonerLite:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://{s}.tile.stamen.com/toner-lite/{z}/{x}/{y}.png", 
                        new[] {"a", "b", "c", "d"}, name: source.ToString(), 
                        persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.StamenWatercolor:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://{s}.tile.stamen.com/watercolor/{z}/{x}/{y}.png",
                        new[] {"a", "b", "c", "d"}, name: source.ToString(), 
                        persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.StamenTerrain:
                    return new HttpTileSource(new GlobalSphericalMercator(4) { Extent = new Extent(-14871588.04,2196494.41775,-5831227.94199995,10033429.95725) },
                        "http://{s}.tile.stamen.com/terrain/{z}/{x}/{y}.png",
                        new[] { "a", "b", "c", "d" }, name: source.ToString(), 
                        persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.EsriWorldTopo:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}",
                        name: source.ToString(), persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.EsriWorldPhysical:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://server.arcgisonline.com/ArcGIS/rest/services/World_Physical_Map/MapServer/tile/{z}/{y}/{x}",
                        name: source.ToString(), persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.EsriWorldShadedRelief:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://server.arcgisonline.com/ArcGIS/rest/services/World_Shaded_Relief/MapServer/tile/{z}/{y}/{x}",
                        name: source.ToString(), persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.EsriWorldReferenceOverlay:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Reference_Overlay/MapServer/tile/{z}/{y}/{x}",
                        name: source.ToString(), persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.EsriWorldTransportation:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer/tile/{z}/{y}/{x}",
                        name: source.ToString(), persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.EsriWorldBoundariesAndPlaces:
                    return new HttpTileSource(new GlobalSphericalMercator(),
                        "http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Boundaries_and_Places/MapServer/tile/{z}/{y}/{x}",
                        name: source.ToString(), persistentCache:persistentCache, tileFetcher:tileFetcher);
                case KnownTileSource.EsriWorldDarkGrayBase:
                    return new HttpTileSource(new GlobalSphericalMercator(0, 10),
                        "http://server.arcgisonline.com/arcgis/rest/services/Canvas/World_Dark_Gray_Base/MapServer/tile/{z}/{y}/{x}",
                        name: source.ToString(), persistentCache:persistentCache, tileFetcher:tileFetcher);
                default:
                    throw new NotSupportedException("KnownTileSource not known");
            }
        }
    }
}
