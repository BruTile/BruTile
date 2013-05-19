﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using BruTile.Predefined;
using BruTile.Web;

namespace BruTile
{
    public class TileSource : ITileSource
    {
        public ITileProvider Provider { get; private set; }
        public ITileSchema Schema { get; private set; }

        public TileSource(ITileProvider tileProvider, ITileSchema tileSchema)
        {
            Provider = tileProvider;
            Schema = tileSchema;
        }

        public static ITileSource Create(KnownOsmTileServers source, string apiKey = null)
        {
            switch (source)
            {
                default:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{S}.tile.openstreetmap.org/{Z}/{X}/{Y}.png", new[] { "a", "b", "c" })), 
                        new GlobalSphericalMercator());
                case KnownOsmTileServers.OpenCycleMap:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{S}.tile.opencyclemap.org/cycle/{Z}/{X}/{Y}.png", new[] {"a", "b", "c"})),
                        new GlobalSphericalMercator(0, 17));
                case KnownOsmTileServers.OpenCycleMapTransport:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{S}.tile2.opencyclemap.org/transport/{Z}/{X}/{Y}.png", new[] { "a", "b", "c" })),
                        new GlobalSphericalMercator(0, 20));
                case KnownOsmTileServers.CloudMadeWebStyle:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{S}.tile.cloudmade.com/{K}/1/256/{Z}/{X}/{Y}.png", new[] { "a", "b", "c" }, apiKey)),
                        new GlobalSphericalMercator()); 
                case KnownOsmTileServers.CloudMadeFineLineStyle:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{S}.tile.cloudmade.com/{K}/2/256/{Z}/{X}/{Y}.png", new[] { "a", "b", "c" }, apiKey)),
                        new GlobalSphericalMercator()); 
                case KnownOsmTileServers.CloudMadeNoNames:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://{S}.tile.cloudmade.com/{K}/1/256/{Z}/{X}/{Y}.png", new[] { "a", "b", "c" }, apiKey)),
                        new GlobalSphericalMercator()); 
                case KnownOsmTileServers.MapQuest:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://otile{S}.mqcdn.com/tiles/1.0.0/osm/{Z}/{X}/{Y}.png", new[] { "1", "2", "3", "4" })),
                        new GlobalSphericalMercator());
                case KnownOsmTileServers.MapQuestAerial:
                    return new TileSource(
                        new WebTileProvider(new BasicRequest("http://oatile{S}.mqcdn.com/naip/{Z}/{X}/{Y}.png", new[] { "1", "2", "3", "4" }, apiKey)),
                        new GlobalSphericalMercator(0, 11));
                case KnownOsmTileServers.BingRoads:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{S}.tiles.virtualearth.net/tiles/r{QuadKey}.jpeg?g={ApiVersion}&token={UserKey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing"));
                case KnownOsmTileServers.BingAerial:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{S}.tiles.virtualearth.net/tiles/a{QuadKey}.jpeg?g={ApiVersion}&token={UserKey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing"));
                case KnownOsmTileServers.BingHybrid:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{S}.tiles.virtualearth.net/tiles/h{QuadKey}.jpeg?g={ApiVersion}&token={UserKey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing"));
                case KnownOsmTileServers.BingRoadsStaging:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{S}.staging.tiles.virtualearth.net/tiles/r{QuadKey}.jpeg?g={ApiVersion}&token={UserKey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing"));
                case KnownOsmTileServers.BingAerialStaging:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{S}.staging.tiles.virtualearth.net/tiles/a{QuadKey}.jpeg?g={ApiVersion}&token={UserKey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing"));
                case KnownOsmTileServers.BingHybridStaging:
                    return new TileSource(
                        new WebTileProvider(new BingRequest("http://t{S}.staging.tiles.virtualearth.net/tiles/h{QuadKey}.jpeg?g={ApiVersion}&token={UserKey}", apiKey)),
                        new GlobalSphericalMercator("jpg", true, 1, 19, "Bing"));
            }
        }
    }
}