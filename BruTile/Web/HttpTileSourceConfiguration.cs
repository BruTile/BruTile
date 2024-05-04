// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace BruTile.Web;

public record HttpTileSourceConfiguration(ITileSchema TileSchema, string UrlFormatter, IEnumerable<string> ServerNodes = default,
       string ApiKey = null, string Name = null, Attribution Attribution = null, string UserAgent = null)
{
    private readonly BasicRequest _basicRequest = new(UrlFormatter, ServerNodes, ApiKey);

    public Uri GetUrl(TileInfo tileInfo) => _basicRequest.GetUri(tileInfo);
}
