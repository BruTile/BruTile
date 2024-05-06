// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Net.Http;

namespace BruTile.Web;

public record HttpTileSourceDefinition(
    ITileSchema TileSchema,
    IUrlBuilder UrlBuilder,
    string Name = "",
    Attribution Attribution = new Attribution(),
    Action<HttpRequestMessage>? ConfigureHttpRequestMessage = null)
{
    public Uri GetUrl(TileInfo tileInfo) => UrlBuilder.GetUrl(tileInfo);
}
