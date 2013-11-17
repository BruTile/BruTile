// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BruTile.Web
{
    /// <summary>
    /// A flexible request builder that can be used for a number of simple cases.
    /// </summary>
    public class BasicRequest : IRequest
    {
        private readonly string _urlFormatter;
        private const string ServerNodeTag = "{s}";
        private const string XTag = "{x}";
        private const string YTag = "{y}";
        private const string ZTag = "{z}";
        private const string ApiKeyTag = "{k}";
        private int _nodeCounter;
        private readonly IList<string> _serverNodes;
        private readonly string _apiKey;

        public BasicRequest(string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey= null)
        {
            _urlFormatter = urlFormatter;
            _serverNodes = serverNodes != null ? serverNodes.ToList() : null;

            // for backward compatibility
            _urlFormatter = _urlFormatter.Replace("{0}", ZTag);
            _urlFormatter = _urlFormatter.Replace("{1}", XTag);
            _urlFormatter = _urlFormatter.Replace("{2}", YTag);
            _apiKey = apiKey;
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            var stringBuilder = new StringBuilder(_urlFormatter);
            stringBuilder.Replace(XTag, info.Index.Col.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(YTag, info.Index.Row.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(ZTag, info.Index.Level);
            stringBuilder.Replace(ApiKeyTag, _apiKey);
            InsertServerNode(stringBuilder, _serverNodes, ref _nodeCounter);
            return new Uri(stringBuilder.ToString());
        }

        private static void InsertServerNode(StringBuilder baseUrl, IList<string> serverNodes, ref int nodeCounter)
        {
            if (serverNodes != null && serverNodes.Count > 0)
            {
                baseUrl.Replace(ServerNodeTag, serverNodes[nodeCounter]);
                nodeCounter++;
                if (nodeCounter >= serverNodes.Count) nodeCounter = 0;
            }
        }
    }
}
