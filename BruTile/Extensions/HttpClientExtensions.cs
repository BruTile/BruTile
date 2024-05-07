// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Web;

namespace BruTile.Extensions;
public static class HttpClientExtensions
{
    public static async Task<byte[]?> GetTileAsync(this HttpClient httpClient, TileInfo tileInfo,
        HttpTileSourceDefinition definition, CancellationToken cancellationToken)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.example.com");
        if (definition.ConfigureHttpRequestMessage is not null)
            definition.ConfigureHttpRequestMessage(requestMessage);
        if (httpClient.DefaultRequestHeaders.UserAgent.Count == 0 && requestMessage.Headers.UserAgent.Count == 0)
            throw new Exception("Set a User-Agent header that is specific to your application or to this tile service.");
        requestMessage.RequestUri = definition.GetUrl(tileInfo);
        var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    public static async Task<byte[]?[]> GetTilesAsync(this HttpClient httpClient, HttpTileSource tileSource, IEnumerable<TileInfo> tileInfos)
    {
        var tasks = tileInfos.Select(t => tileSource.GetTileAsync(httpClient, t, CancellationToken.None)).ToArray();
        var result = Task.WhenAll(tasks);
        return await result.ConfigureAwait(false);
    }
}
