// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using BruTile.Web;

namespace BruTile.Predefined
{
    /// <summary>
    /// Known popular OSM renderers
    /// </summary>
    public enum KnownTileServer
    {
        Mapnik,
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
        StamenWatercolor,
        EsriWorldTopo,
        EsriWorldPhysical,
        EsriWorldShadedRelief,
        EsriWorldReferenceOverlay,
        EsriWorldTransportation,
        EsriWorldBoundariesAndPlaces
    }

    public static class KnownTileSources
    {
        /// <summary>
        /// Static factory method for known tile services
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="apiKey">An (optional) API key</param>
        /// <returns>The tile source</returns>
        public static ITileSource Create(KnownTileServer source = KnownTileServer.Mapnik, string apiKey = null)
        {
            switch (source)
            {
                case KnownTileServer.Mapnik:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                            new[] {"a", "b", "c"})),
                        new GlobalSphericalMercator(0, 18)) {Title = source.ToString()};
                case KnownTileServer.OpenCycleMap:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.opencyclemap.org/cycle/{z}/{x}/{y}.png",
                            new[] {"a", "b", "c"})),
                        new GlobalSphericalMercator(0, 17)) {Title = source.ToString()};
                case KnownTileServer.OpenCycleMapTransport:
                    return new TileSource(
                        new WebTileProvider(
                            new BasicRequest("http://{s}.tile2.opencyclemap.org/transport/{z}/{x}/{y}.png",
                                new[] {"a", "b", "c"})),
                        new GlobalSphericalMercator(0, 20)) {Title = source.ToString()};
                case KnownTileServer.CloudMadeWebStyle:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.cloudmade.com/{k}/1/256/{z}/{x}/{y}.png",
                            new[] {"a", "b", "c"}, apiKey)),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.CloudMadeFineLineStyle:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.cloudmade.com/{k}/2/256/{z}/{x}/{y}.png",
                            new[] {"a", "b", "c"}, apiKey)),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.CloudMadeNoNames:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.cloudmade.com/{k}/3/256/{z}/{x}/{y}.png",
                            new[] {"a", "b", "c"}, apiKey)),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.MapQuest:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest(
                            "http://otile{s}.mqcdn.com/tiles/1.0.0/osm/{z}/{x}/{y}.png", new[] {"1", "2", "3", "4"})),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.MapQuestAerial:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest(
                            "http://otile{s}.mqcdn.com/tiles/1.0.0/sat/{z}/{x}/{y}.png", new[] {"1", "2", "3", "4"},
                            apiKey)),
                        new GlobalSphericalMercator(0, 11)) {Title = source.ToString()};
                case KnownTileServer.MapQuestRoadsAndLabels:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest(
                            "http://otile{s}.mqcdn.com/tiles/1.0.0/map/{z}/{x}/{y}.png", new[] {"1", "2", "3", "4"},
                            apiKey)),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.BingRoads:
                    return new TileSource(
                        new WebTileProvider(
                            new BingRequest(
                                "http://t{s}.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g={apiversion}&token={userkey}",
                                apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) {Title = source.ToString()};
                case KnownTileServer.BingAerial:
                    return new TileSource(
                        new WebTileProvider(
                            new BingRequest(
                                "http://t{s}.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g={apiversion}&token={userkey}",
                                apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) {Title = source.ToString()};
                case KnownTileServer.BingHybrid:
                    return new TileSource(
                        new WebTileProvider(
                            new BingRequest(
                                "http://t{s}.tiles.virtualearth.net/tiles/h{quadkey}.jpeg?g={apiversion}&token={userkey}",
                                apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) {Title = source.ToString()};
                case KnownTileServer.BingRoadsStaging:
                    return new TileSource(
                        new WebTileProvider(
                            new BingRequest(
                                "http://t{s}.staging.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g={apiversion}&token={userkey}",
                                apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) {Title = source.ToString()};
                case KnownTileServer.BingAerialStaging:
                    return new TileSource(
                        new WebTileProvider(
                            new BingRequest(
                                "http://t{s}.staging.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g={apiversion}&token={userkey}",
                                apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) {Title = source.ToString()};
                case KnownTileServer.BingHybridStaging:
                    return new TileSource(
                        new WebTileProvider(
                            new BingRequest(
                                "http://t{s}.staging.tiles.virtualearth.net/tiles/h{quadkey}.jpeg?g={apiversion}&token={userkey}",
                                apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) {Title = source.ToString()};
                case KnownTileServer.StamenToner:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.stamen.com/toner/{z}/{x}/{y}.png",
                            new[] {"a", "b", "c", "d"})),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.StamenWatercolor:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.stamen.com/watercolor/{z}/{x}/{y}.png",
                            new[] {"a", "b", "c", "d"})),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.EsriWorldTopo:
                    return new TileSource(
                        new WebTileProvider(
                            new BasicRequest(
                                "http://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.EsriWorldPhysical:
                    return new TileSource(
                        new WebTileProvider(
                            new BasicRequest(
                                "http://server.arcgisonline.com/ArcGIS/rest/services/World_Physical_Map/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.EsriWorldShadedRelief:
                    return new TileSource(
                        new WebTileProvider(
                            new BasicRequest(
                                "http://server.arcgisonline.com/ArcGIS/rest/services/World_Shaded_Relief/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.EsriWorldReferenceOverlay:
                    return new TileSource(
                        new WebTileProvider(
                            new BasicRequest(
                                "http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Reference_Overlay/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.EsriWorldTransportation:
                    return new TileSource(
                        new WebTileProvider(
                            new BasicRequest(
                                "http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                case KnownTileServer.EsriWorldBoundariesAndPlaces:
                    return new TileSource(
                        new WebTileProvider(
                            new BasicRequest(
                                "http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Boundaries_and_Places/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) {Title = source.ToString()};
                default:
                    throw new NotSupportedException("KnownTileServer not known");
            }
        }
    }
}
