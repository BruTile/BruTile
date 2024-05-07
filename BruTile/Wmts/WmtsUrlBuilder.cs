// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BruTile.Web;

namespace BruTile.Wmts;

public class WmtsUrlBuilder(IEnumerable<ResourceUrl> resourceUrls, IDictionary<int, string> levelToIdentifier) : IUrlBuilder
{
    public const string XTag = "{TileCol}";
    public const string YTag = "{TileRow}";
    public const string ZTag = "{TileMatrix}";
    public const string TileMatrixSetTag = "{TileMatrixSet}";
    public const string StyleTag = "{Style}";

    private readonly List<ResourceUrl> _resourceUrls = resourceUrls.ToList();
    private int _resourceUrlCounter;
    private readonly object _syncLock = new();
    private readonly IDictionary<int, string> _levelToIdentifier = levelToIdentifier;

    public Uri GetUrl(TileInfo info)
    {
        var urlFormatter = GetNextServerNode();
        var stringBuilder = new StringBuilder(urlFormatter.Template);

        // For wmts we need to map the level int to an identifier of type string.
        var identifier = _levelToIdentifier[info.Index.Level];
        stringBuilder.Replace(XTag, info.Index.Col.ToString(CultureInfo.InvariantCulture));
        stringBuilder.Replace(YTag, info.Index.Row.ToString(CultureInfo.InvariantCulture));
        stringBuilder.Replace(ZTag, identifier);

        return new Uri(stringBuilder.ToString());
    }

    private ResourceUrl GetNextServerNode()
    {
        lock (_syncLock)
        {
            var serverNode = _resourceUrls[_resourceUrlCounter++];
            if (_resourceUrlCounter >= _resourceUrls.Count) _resourceUrlCounter = 0;
            return serverNode;
        }
    }
}

[Obsolete("Use WmtsUrlBuilder instead.")]
public class WmtsRequest(IEnumerable<ResourceUrl> resourceUrls, IDictionary<int, string> levelToIdentifier) :
    WmtsUrlBuilder(resourceUrls, levelToIdentifier), IRequest
{ }
