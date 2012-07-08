// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
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
        readonly string _baseUrl;
        readonly string _token;
        readonly char _mapType;

        private const string VersionBingMaps = "517";

        /// <remarks>You need a token for the the staging and the proper bing maps server, see:
        /// http://msdn.microsoft.com/en-us/library/cc980844.aspx</remarks>
        public BingRequest(string baseUrl, string token, BingMapType mapType)
        {
            _baseUrl = baseUrl;
            _token = token;
            _mapType = ToMapTypeChar(mapType);
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            //todo: use different nodes
            string url = string.Format(CultureInfo.InvariantCulture, "{0}/{1}" + "{2}.jpeg?g={4}&token={3}",
              _baseUrl, _mapType, TileXyToQuadKey(info.Index.Col, info.Index.Row, int.Parse(info.Index.LevelId)), _token, VersionBingMaps);
            return new Uri(url);
        }

        public static string UrlBingStaging
        {
            get { return "http://t0.staging.tiles.virtualearth.net/tiles"; }
        }

        public static string UrlBing
        {
            get { return "http://t0.tiles.virtualearth.net/tiles"; }
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
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>A string containing the QuadKey.</returns>
        /// Stole this methode from this nice blog: http://www.silverlightshow.net/items/Virtual-earth-deep-zooming.aspx. PDD.
        private static string TileXyToQuadKey(int tileX, int tileY, int levelOfDetail)
        {
            var quadKey = new StringBuilder();

            for (int i = levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);

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
    }
}
