// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
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
        string baseUrl;
        string token;
        char mapType;
        IDictionary<BingMapType, char> mapTypes = new Dictionary<BingMapType, char>();

        /// <remarks>You need a token for the the staging and the proper bing maps server, see:
        /// http://msdn.microsoft.com/en-us/library/cc980844.aspx</remarks>
        public BingRequest(string baseUrl, string token, BingMapType mapType)
        {
            this.baseUrl = baseUrl;
            this.token = token;
            this.mapType = ToMapTypeChar(mapType);
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="tile">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            //todo: use different nodes
            string url = string.Format(CultureInfo.InvariantCulture, "{0}/{1}" + "{2}.jpeg?g=203&token={3}",
              baseUrl, mapType, TileXYToQuadKey(info.Index.Col, info.Index.Row, info.Index.Level + 1), token);
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

        private char ToMapTypeChar(BingMapType mapType)
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
        private static string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
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
