﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using BruTile.Predefined;
using BruTile.Web;

namespace BruTile
{
    /// <summary>
    /// The default implementation of a <see cref="ITileSource"/>.
    /// </summary>
    public class TileSource : ITileSource
    {
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="tileProvider">The tile provider</param>
        /// <param name="tileSchema">The tile schema</param>
        public TileSource(ITileProvider tileProvider, ITileSchema tileSchema)
        {
            Provider = tileProvider;
            Schema = tileSchema;
        }
        
        /// <summary>
        /// Gets a value indicating the title of the tile source
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets a value indicating the tile provider
        /// </summary>
        public ITileProvider Provider { get; private set; }

        /// <summary>
        /// Gets a value indicating the tile schema
        /// </summary>
        public ITileSchema Schema { get; private set; }

        public override string ToString()
        {
            return string.Format("[TileSource:{0}]", Title);
        }

        /// <summary>
        /// Static factory method for known tile services
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="apiKey">An (optional) API key</param>
        /// <returns>The tile source</returns>
        public static ITileSource Create(KnownTileServers source = KnownTileServers.Mapnik, string apiKey = null)
        {
            switch (source)
            {
                case KnownTileServers.Mapnik:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", new[] { "a", "b", "c" })), 
                        new GlobalSphericalMercator(0, 18)) {Title = source.ToString()} ;
                case KnownTileServers.OpenCycleMap:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.opencyclemap.org/cycle/{z}/{x}/{y}.png", new[] {"a", "b", "c"})),
                        new GlobalSphericalMercator(0, 17)) { Title = source.ToString() };
                case KnownTileServers.OpenCycleMapTransport:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile2.opencyclemap.org/transport/{z}/{x}/{y}.png", new[] { "a", "b", "c" })),
                        new GlobalSphericalMercator(0, 20)) { Title = source.ToString() };
                case KnownTileServers.CloudMadeWebStyle:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.cloudmade.com/{k}/1/256/{z}/{x}/{y}.png", new[] { "a", "b", "c" }, apiKey)),
                        new GlobalSphericalMercator()) { Title = source.ToString() }; 
                case KnownTileServers.CloudMadeFineLineStyle:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.cloudmade.com/{k}/2/256/{z}/{x}/{y}.png", new[] { "a", "b", "c" }, apiKey)),
                        new GlobalSphericalMercator()) { Title = source.ToString() }; 
                case KnownTileServers.CloudMadeNoNames:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.cloudmade.com/{k}/3/256/{z}/{x}/{y}.png", new[] { "a", "b", "c" }, apiKey)),
                        new GlobalSphericalMercator()) { Title = source.ToString() }; 
                case KnownTileServers.MapQuest:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://otile{s}.mqcdn.com/tiles/1.0.0/osm/{z}/{x}/{y}.png", new[] { "1", "2", "3", "4" })),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                case KnownTileServers.MapQuestAerial:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://mtile0{s}.mqcdn.com/tiles/1.0.0/vx/sat/{z}/{x}/{y}.png", new[] { "1", "2", "3", "4" }, apiKey)),
                        new GlobalSphericalMercator(0, 13)) { Title = source.ToString() };
                case KnownTileServers.MapQuestRoadsAndLabels:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://mtile0{s}.mqcdn.com/tiles/1.0.0/vx/hyb/{z}/{x}/{y}.png", new[] { "1", "2", "3", "4" }, apiKey)),
                        new GlobalSphericalMercator(2)) { Title = source.ToString() };
                case KnownTileServers.BingRoads:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{s}.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g={apiversion}&token={userkey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) { Title = source.ToString() };
                case KnownTileServers.BingAerial:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{s}.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g={apiversion}&token={userkey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) { Title = source.ToString() };
                case KnownTileServers.BingHybrid:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{s}.tiles.virtualearth.net/tiles/h{quadkey}.jpeg?g={apiversion}&token={userkey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) { Title = source.ToString() };
                case KnownTileServers.BingRoadsStaging:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{s}.staging.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g={apiversion}&token={userkey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) { Title = source.ToString() };
                case KnownTileServers.BingAerialStaging:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{s}.staging.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g={apiversion}&token={userkey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) { Title = source.ToString() };
                case KnownTileServers.BingHybridStaging:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{s}.staging.tiles.virtualearth.net/tiles/h{quadkey}.jpeg?g={apiversion}&token={userkey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing")) { Title = source.ToString() };
                case KnownTileServers.StamenToner:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.stamen.com/toner/{z}/{x}/{y}.png", new[] { "a", "b", "c", "d" })),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                case KnownTileServers.StamenWatercolor:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{s}.tile.stamen.com/watercolor/{z}/{x}/{y}.png", new[] { "a", "b", "c", "d" })),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                case KnownTileServers.EsriWorldTopo:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                case KnownTileServers.EsriWorldPhysical:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://server.arcgisonline.com/ArcGIS/rest/services/World_Physical_Map/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                case KnownTileServers.EsriWorldHydroBasemap:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://184.72.212.114:6080/ArcGIS/rest/services/WorldHydroReferenceOverlay/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                case KnownTileServers.EsriWorldShadedRelief:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://server.arcgisonline.com/ArcGIS/rest/services/World_Shaded_Relief/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                case KnownTileServers.EsriWorldReferenceOverlay:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Reference_Overlay/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                case KnownTileServers.EsriWorldTransportation:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                case KnownTileServers.EsriWorldBoundariesAndPlaces:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Boundaries_and_Places/MapServer/tile/{z}/{y}/{x}")),
                        new GlobalSphericalMercator()) { Title = source.ToString() };
                default:
                    throw new NotSupportedException("KnownTileServer not known");
            }
        }
    }
}