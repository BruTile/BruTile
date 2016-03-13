// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace BruTile.Web
{
    [Obsolete("Use KnownTileSources instead", true)]
    public enum BingMapType
    {
        Roads,
        Aerial,
        Hybrid
    }

    [Obsolete("Use BasicRequest with {quadkey} tag instead", true)]
    public class BingRequest : IRequest
    {
        const string DefaultApiVersion = "";

        public BingRequest(string baseUrl, string token, BingMapType mapType, string apiVersion = DefaultApiVersion)
        {
            throw new NotImplementedException();
        }

        public BingRequest(string urlFormatter, string userKey, string apiVersion = DefaultApiVersion, IEnumerable<string> serverNodes = null)
        {
            throw new NotImplementedException();
        }

        public static string UrlBingStaging
        {
            get { throw new NotImplementedException(); }
        }

        public static string UrlBing
        {
            get { throw new NotImplementedException(); }
        }

        public string ApiVersion { get; set; }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
