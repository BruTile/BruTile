// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System;

namespace BruTile.Web
{
    /// <summary>
    /// Known popular OSM renderers
    /// </summary>
    public enum KnownOsmTileServers
    {
        Custom,
        /// <summary>
        /// Mapnic
        /// </summary>
        Mapnic,
        /// <summary>
        /// OSMA
        /// </summary>
        Osma,
        /// <summary>
        /// Open Cycle Map
        /// </summary>
        OpenCycleMap,
        /// <summary>
        /// Open cycle map (experimental)
        /// </summary>
        OpenCycleMapTransport,

        /// <summary>
        /// Cloud Made (Web Style)
        /// </summary>
        CloudMadeWebStyle,

        /// <summary>
        /// Cloud Made (Fine line style)
        /// </summary>
        CloudMadeFineLineStyle,

        /// <summary>
        /// Cloud Made (No names style)
        /// </summary>
        CloudMadeNoNames,

        /// <summary>
        /// Map Quest
        /// </summary>
        MapQuest,

        /// <summary>
        /// Map Quest (aerial)
        /// </summary>
        MapQuestAerial,
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
            UrlFormat = urlFormat;
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

            return new Uri(string.Format(UrlFormat, server, tileIndex.LevelId, tileIndex.Col, tileIndex.Row));
        }

        public static OsmTileServerConfig Create(KnownOsmTileServers knownOsmRenderer, string apiKey)
        {
            switch (knownOsmRenderer)
            {
                case KnownOsmTileServers.Mapnic:
                default:
                    return new OsmTileServerConfig("http://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png ", 3, new[] { "a", "b", "c" }, 0, 18);
                case KnownOsmTileServers.Osma:
                    return new OsmTileServerConfig("http://{0}.tah.openstreetmap.org/Tiles/tile/{1}/{2}/{3}.png ", 3, new[] { "a", "b", "c" }, 0, 17);
                case KnownOsmTileServers.OpenCycleMap:
                    return new OsmTileServerConfig("http://{0}.tile.opencyclemap.org/cycle/{1}/{2}/{3}.png ", 3, new[] { "a", "b", "c" }, 0, 17);
                case KnownOsmTileServers.OpenCycleMapTransport:
                    return new OsmTileServerConfig("http://{0}.tile2.opencyclemap.org/transport/{1}/{2}/{3}.png ", 3, new[] { "a", "b", "c" }, 0, 17);
                case KnownOsmTileServers.CloudMadeWebStyle:
                    return new OsmTileServerConfigWithApiKey("http://{0}.tile.cloudmade.com/{4}/1/256/{1}/{2}/{3}.png ", 3, new[] { "a", "b", "c" }, 0, 18, apiKey);
                case KnownOsmTileServers.CloudMadeFineLineStyle:
                    return new OsmTileServerConfigWithApiKey("http://{0}.tile.cloudmade.com/{4}/2/256/{1}/{2}/{3}.png ", 3, new[] { "a", "b", "c" }, 0, 18, apiKey);
                case KnownOsmTileServers.CloudMadeNoNames:
                    return new OsmTileServerConfigWithApiKey("http://{0}.tile.cloudmade.com/{4}/1/256/{1}/{2}/{3}.png ", 3, new[] { "a", "b", "c" }, 0, 18, apiKey);
                case KnownOsmTileServers.MapQuest:
                    return new OsmTileServerConfig("http://{0}.mqcdn.com/tiles/1.0.0/osm/{1}/{2}/{3}.png ", 4, new[] { "otile1", "otile2", "otile3", "otile4" }, 0, 18);
                case KnownOsmTileServers.MapQuestAerial:
                    return new OsmTileServerConfig("http://{0}.mqcdn.com/naip/{1}/{2}/{3}.png ", 4, new[] { "oatile1", "oatile2", "oatile3", "oatile4" }, 0, 11);
                case KnownOsmTileServers.Custom:
                    throw new InvalidOperationException("Cannot create a custom 'OsmTileServerConfig' using Create(...) statement.");
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

            return new Uri(string.Format(UrlFormat, server, tileIndex.LevelId, tileIndex.Col, tileIndex.Row, ApiKey));
        }
    }

    public class OsmRequest : IRequest
    {
        public readonly OsmTileServerConfig OsmConfig;

        public OsmRequest(KnownOsmTileServers renderer, string apiKey)
            : this(OsmTileServerConfig.Create(renderer, apiKey))
        {
        }

        public OsmRequest(KnownOsmTileServers knownOsmTileServers)
            :this(OsmTileServerConfig.Create(knownOsmTileServers, null))
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