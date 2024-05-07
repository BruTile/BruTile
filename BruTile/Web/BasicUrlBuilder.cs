// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BruTile.Web;

/// <summary>
/// A flexible request builder that can be used for a number of simple cases.
/// </summary>
public class BasicUrlBuilder : IUrlBuilder
{
    public const string QuadKeyTag = "{quadkey}";
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
    /// <summary>
    /// Tag to be replaced with the Bing quad key. This is a single number that 
    /// represents a combination of X, Y and Z. This can not be used in combination
    /// with the X, Y and Z tags.
    /// </summary>
    private readonly string _urlFormatter;
    private int _nodeCounter;
    private readonly List<string> _serverNodes;
    private readonly object _nodeCounterLock = new();

    /// <summary>
    /// Creates an instance of this class
    /// </summary>
    /// <param name="urlFormatter">The url formatter</param>
    /// <param name="serverNodes">The server nodes</param>
    /// <param name="apiKey">The API key</param>
    public BasicUrlBuilder(string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey = null)
    {
        _urlFormatter = urlFormatter;
        _serverNodes = serverNodes?.ToList();

        // For backward compatibility
        _urlFormatter = _urlFormatter.Replace("{0}", ZTag);
        _urlFormatter = _urlFormatter.Replace("{1}", XTag);
        _urlFormatter = _urlFormatter.Replace("{2}", YTag);

        if (!string.IsNullOrEmpty(apiKey))
            _urlFormatter = _urlFormatter.Replace(ApiKeyTag, apiKey);
    }

    /// <summary>
    /// Generates a URI at which to get the data for a tile.
    /// </summary>
    /// <param name="info">Information about a tile.</param>
    /// <returns>The URI at which to get the data for the specified tile.</returns>
    public Uri GetUrl(TileInfo info)
    {
        var stringBuilder = new StringBuilder(_urlFormatter);
        stringBuilder.Replace(XTag, info.Index.Col.ToString(CultureInfo.InvariantCulture));
        stringBuilder.Replace(YTag, info.Index.Row.ToString(CultureInfo.InvariantCulture));
        stringBuilder.Replace(ZTag, info.Index.Level.ToString(CultureInfo.InvariantCulture));
        stringBuilder.Replace(QuadKeyTag, TileXyToQuadKey(info.Index.Col, info.Index.Row, info.Index.Level));

        InsertServerNode(stringBuilder, _serverNodes);

        return new Uri(stringBuilder.ToString());
    }

    private void InsertServerNode(StringBuilder baseUrl, IList<string> serverNodes)
    {
        if (serverNodes != null && serverNodes.Count > 0)
        {
            var serverNode = GetNextServerNode();
            baseUrl.Replace(ServerNodeTag, serverNode);
        }
    }

    private string GetNextServerNode()
    {
        lock (_nodeCounterLock)
        {
            var serverNode = _serverNodes[_nodeCounter++];
            if (_nodeCounter >= _serverNodes.Count) _nodeCounter = 0;
            return serverNode;
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
            sb.Append(')');
        }
        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>
    /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
    /// </summary>
    /// <param name="tileX">Tile X coordinate.</param>
    /// <param name="tileY">Tile Y coordinate.</param>
    /// <param name="level">Level of detail, from 1 (lowest detail)
    /// to 23 (highest detail).</param>
    /// <returns>A string containing the QuadKey.</returns>
    /// Stole this method from this nice blog: http://www.silverlightshow.net/items/Virtual-earth-deep-zooming.aspx. PDD.
    private static string TileXyToQuadKey(int tileX, int tileY, int level)
    {
        var quadKey = new StringBuilder();

        for (var i = level; i > 0; i--)
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
}

[Obsolete("Use BasicUrlBuilder instead.")]
public class BasicRequest(string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey = null) :
    BasicUrlBuilder(urlFormatter, serverNodes, apiKey), IUrlBuilder
{ }