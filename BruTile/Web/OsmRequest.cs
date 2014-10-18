// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System;
using BruTile.Predefined;

namespace BruTile.Web
{
    [Obsolete("Replaced with KnownTileSources")]
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

        public static OsmTileServerConfig Create(KnownTileSource knownTileSource, string apiKey)
        {
            switch (knownTileSource)
            {
                default:
                    return new OsmTileServerConfig("http://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18);
                case KnownTileSource.OpenCycleMap:
                    return new OsmTileServerConfig("http://{0}.tile.opencyclemap.org/cycle/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 16);
                case KnownTileSource.OpenCycleMapTransport:
                    return new OsmTileServerConfig("http://{0}.tile2.opencyclemap.org/transport/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18);
                case KnownTileSource.CloudMadeWebStyle:
                    return new OsmTileServerConfigWithApiKey("http://{0}.tile.cloudmade.com/{4}/1/256/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18, apiKey);
                case KnownTileSource.CloudMadeFineLineStyle:
                    return new OsmTileServerConfigWithApiKey("http://{0}.tile.cloudmade.com/{4}/2/256/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18, apiKey);
                case KnownTileSource.CloudMadeNoNames:
                    return new OsmTileServerConfigWithApiKey("http://{0}.tile.cloudmade.com/{4}/1/256/{1}/{2}/{3}.png", 3, new[] { "a", "b", "c" }, 0, 18, apiKey);
                case KnownTileSource.MapQuest:
                    return new OsmTileServerConfig("http://otile{0}.mqcdn.com/tiles/1.0.0/osm/{1}/{2}/{3}.png", 4, new[] { "1", "2", "3", "4" }, 0, 18);
                case KnownTileSource.MapQuestAerial:
                    return new OsmTileServerConfig("http://oatile{0}.mqcdn.com/naip/{1}/{2}/{3}.png", 4, new[] { "1", "2", "3", "4" }, 0, 11);
             }
        }
    }

    [Obsolete("Replaced with KnownTileSources")]
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

    [Obsolete("Replaced with BasicRequest")]
    public class OsmRequest : IRequest
    {
        public readonly OsmTileServerConfig OsmConfig;

        public OsmRequest()
            :this(KnownTileSource.Mapnik)
        {}

        public OsmRequest(KnownTileSource knownTileSources)
            :this(OsmTileServerConfig.Create(knownTileSources, null))
        {
        }

        public OsmRequest(KnownTileSource renderer, string apiKey)
            : this(OsmTileServerConfig.Create(renderer, apiKey))
        {
        }

        public OsmRequest(OsmTileServerConfig osmConfig)
        {
            OsmConfig = osmConfig;
        }

        public Uri GetUri(TileInfo info)
        {
            return OsmConfig.GetUri(info.Index);
        }
    }
}