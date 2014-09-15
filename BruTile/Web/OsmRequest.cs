// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System;

namespace BruTile.Web
{
    /// <summary>
    /// Known popular OSM renderers
    /// </summary>
    public enum KnownTileServers
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
        EsriWorldHydroBasemap,
        EsriWorldShadedRelief,
        EsriWorldReferenceOverlay,
        EsriWorldTransportation,
        EsriWorldBoundariesAndPlaces
    }

    public class OsmTileServerConfig
    {
        public readonly string UrlFormat;
        public readonly int NumberOfServers;
        public readonly string[] ServerIdentifier;
        public readonly int MinResolution;
        public readonly int MaxResolution;

        internal OsmTileServerConfig(int minResolution, int maxResolution)
        {
            MinResolution = minResolution;
            MaxResolution = maxResolution;
        }

        public OsmTileServerConfig(string urlFormat, int numberOfServers, string[] serverIdentifier, int minResolution, int maxResolution)
            : this(minResolution, maxResolution)
        {
            // make sure no trailing spaces. Produces errors on Mac (Monno?)
            UrlFormat = urlFormat.Trim();
            NumberOfServers = numberOfServers;
            ServerIdentifier = serverIdentifier;

        }

        protected static Int32 GetServerIdentifierIndex(TileIndex tileIndex, Int32 max)
        {
            return (tileIndex.Col + 2 * tileIndex.Row) % max;
        }

        public virtual Uri GetUri(TileIndex tileIndex)
        {
            var server = ServerIdentifier[GetServerIdentifierIndex(tileIndex, NumberOfServers)];

            return new Uri(string.Format(UrlFormat, server, tileIndex.Level, tileIndex.Col, tileIndex.Row));
        }

        public static OsmTileServerConfig Create(KnownTileServers knownTileServer, string apiKey)
        {
            switch (knownTileServer)
            {
                default:
                    return new OsmTileServerConfig("http://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18);
                case KnownTileServers.OpenCycleMap:
                    return new OsmTileServerConfig("http://{0}.tile.opencyclemap.org/cycle/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 16);
                case KnownTileServers.OpenCycleMapTransport:
                    return new OsmTileServerConfig("http://{0}.tile2.opencyclemap.org/transport/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18);
                case KnownTileServers.CloudMadeWebStyle:
                    return new OsmTileServerConfigWithApiKey("http://{0}.tile.cloudmade.com/{4}/1/256/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18, apiKey);
                case KnownTileServers.CloudMadeFineLineStyle:
                    return new OsmTileServerConfigWithApiKey("http://{0}.tile.cloudmade.com/{4}/2/256/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18, apiKey);
                case KnownTileServers.CloudMadeNoNames:
                    return new OsmTileServerConfigWithApiKey("http://{0}.tile.cloudmade.com/{4}/1/256/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18, apiKey);
                case KnownTileServers.MapQuest:
                    return new OsmTileServerConfig("http://otile{0}.mqcdn.com/tiles/1.0.0/osm/{1}/{2}/{3}.png", 4, new[] { "1", "2", "3", "4" }, 0, 18);
                case KnownTileServers.MapQuestAerial:
                    return new OsmTileServerConfig("http://oatile{0}.mqcdn.com/naip/{1}/{2}/{3}.png", 4, new[] { "1", "2", "3", "4" }, 0, 11);
             }
        }
    }

    internal class OsmTileServerConfigWithApiKey : OsmTileServerConfig
    {
        public readonly string ApiKey;

        public OsmTileServerConfigWithApiKey(string urlFormat, int numberOfServers, string[] serverIdentifier, int minResolution, int maxResolution, string apiKey)
            :base(urlFormat, numberOfServers, serverIdentifier, minResolution, maxResolution)
        {
            ApiKey = apiKey;
        }

        public override Uri GetUri(TileIndex tileIndex)
        {
            var server = ServerIdentifier[GetServerIdentifierIndex(tileIndex, NumberOfServers)];

            return new Uri(string.Format(UrlFormat, server, tileIndex.Level, tileIndex.Col, tileIndex.Row, ApiKey));
        }
    }

    public class OsmRequest : IRequest
    {
        public readonly OsmTileServerConfig OsmConfig;

        public OsmRequest()
            :this(KnownTileServers.Mapnik)
        {}

        public OsmRequest(KnownTileServers knownTileServers)
            :this(OsmTileServerConfig.Create(knownTileServers, null))
        {
        }

        public OsmRequest(KnownTileServers renderer, string apiKey)
            : this(OsmTileServerConfig.Create(renderer, apiKey))
        {
        }

        public OsmRequest(OsmTileServerConfig osmConfig)
        {
            OsmConfig = osmConfig;
        }

        #region Implementation of IRequest

        public Uri GetUri(TileInfo info)
        {
            return OsmConfig.GetUri(info.Index);
        }

        #endregion
    }
}