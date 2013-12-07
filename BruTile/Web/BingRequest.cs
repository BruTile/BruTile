// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BruTile.Web
{
    public enum BingMapType
    {
        Roads,
        Aerial,
        Hybrid
    }

    public class BingRequest : IRequest
    {
        private const string DefaultApiVersion = "517";
        public const string ServerNodeTag = "{s}";
        public const string QuadKeyTag = "{quadkey}";
        public const string UserKeyTag = "{userkey}";
        public const string ApiVersionTag = "{apiversion}";
        private readonly string _urlFormatter;
        private readonly string _userKey;
        private int _nodeCounter;
        private readonly IList<string> _serverNodes = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7" };

        /// <remarks>You need a token for the the staging and the proper bing maps server, see:
        /// http://msdn.microsoft.com/en-us/library/cc980844.aspx</remarks>
        public BingRequest(string baseUrl, string token, BingMapType mapType, string apiVersion = DefaultApiVersion)
        {
            _urlFormatter = baseUrl + "/" + ToMapTypeChar(mapType) +  QuadKeyTag + ".jpeg?g=" + 
                ApiVersionTag + "&token=" + UserKeyTag;
            _userKey = token;
            ApiVersion = apiVersion;
        }

        public BingRequest(string urlFormatter, string userKey, string apiVersion = DefaultApiVersion, IEnumerable<string> serverNodes = null)
        {
            _urlFormatter = urlFormatter;
            _userKey = userKey;
            ApiVersion = apiVersion;
            if (serverNodes != null) _serverNodes = serverNodes.ToList();
        }

        public static string UrlBingStaging
        {
            get { return "http://t{s}.staging.tiles.virtualearth.net/tiles"; }
        }

        public static string UrlBing
        {
            get { return "http://t{s}.tiles.virtualearth.net/tiles"; }
        }

        public string ApiVersion { get; set; }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            var stringBuilder = new StringBuilder(_urlFormatter);
            stringBuilder.Replace(QuadKeyTag, TileXyToQuadKey(info.Index.Col, info.Index.Row, info.Index.Level));
            stringBuilder.Replace(ApiVersionTag, ApiVersion);
            stringBuilder.Replace(UserKeyTag, _userKey);
            InsertServerNode(stringBuilder, _serverNodes, ref _nodeCounter);
            return new Uri(stringBuilder.ToString());
        }

        private static char ToMapTypeChar(BingMapType mapType)
        {
            switch (mapType)
            {
                case BingMapType.Roads:
                    return 'r';
                case BingMapType.Aerial:
                    return 'a';
                case BingMapType.Hybrid:
                    return 'h';
                default:
                    throw new ArgumentException("Unknown MapType");
            }
        }
        /// <summary>
        /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="levelId">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>A string containing the QuadKey.</returns>
        /// Stole this methode from this nice blog: http://www.silverlightshow.net/items/Virtual-earth-deep-zooming.aspx. PDD.
        private static string TileXyToQuadKey(int tileX, int tileY, string levelId)
        {
            var quadKey = new StringBuilder();

            var levelOfDetail = int.Parse(levelId);

            for (var i = levelOfDetail; i > 0; i--)
            {
                var digit = '0';
                var mask = 1 << (i - 1);

                if ((tileX & mask) != 0)
                {
                    digit++;
                }

                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }

                quadKey.Append(digit);
            }

            return quadKey.ToString();
        }

        private static void InsertServerNode(StringBuilder baseUrl, IList<string> serverNodes, ref int nodeCounter)
        {
            if (serverNodes != null && serverNodes.Count > 0)
            {
                baseUrl.Replace(ServerNodeTag, serverNodes[nodeCounter++]);
                if (nodeCounter >= serverNodes.Count) nodeCounter = 0;
            }
        }
    }
}
