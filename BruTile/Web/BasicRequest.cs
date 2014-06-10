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
        /// <summary>
        /// Tag to be replaced by column value.
        /// </summary>
        private const string XTag = "{x}";
        /// <summary>
        /// Tag to be replaced by row value.
        /// </summary>
        private const string YTag = "{y}";
        /// <summary>
        /// Tag to be replaced by zoom level value.
        /// </summary>
        private const string ZTag = "{z}";

        /// <summary>
        /// Tag to be replaced by server node entries, if any.
        /// </summary>
        private const string ServerNodeTag = "{s}";
        /// <summary>
        /// Tag to be replaced by api key, if defined.
        /// </summary>
        private const string ApiKeyTag = "{k}";

        private readonly string _urlFormatter;
        private int _nodeCounter;
        private readonly List<string> _serverNodes;
        //private readonly string _apiKey;

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="urlFormatter">The url formatter</param>
        /// <param name="serverNodes">The server nodes</param>
        /// <param name="apiKey">The API key</param>
        public BasicRequest(string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey= null)
        {
            _urlFormatter = urlFormatter;
            _serverNodes = serverNodes != null ? serverNodes.ToList() : null;

            // for backward compatibility
            _urlFormatter = _urlFormatter.Replace("{0}", ZTag);
            _urlFormatter = _urlFormatter.Replace("{1}", XTag);
            _urlFormatter = _urlFormatter.Replace("{2}", YTag);

            if (!string.IsNullOrEmpty(apiKey))
                _urlFormatter = _urlFormatter.Replace(ApiKeyTag, apiKey) ;
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
            
            //stringBuilder.Replace(ApiKeyTag, _apiKey);
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

        public override string ToString()
        {
            var sb = new StringBuilder("[BasicRequest:");
            sb.AppendFormat("{0}", _urlFormatter);
            if (_serverNodes != null)
            {
                sb.AppendFormat(",(\"{0}\"", _serverNodes[0]);
                foreach (var serverNode in _serverNodes.Skip(1))
                    sb.AppendFormat(",\"{0}\"", serverNode);
                sb.Append(")");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
