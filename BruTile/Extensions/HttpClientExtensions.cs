// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

#nullable enable

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
        requestMessage.RequestUri = definition.GetUrl(tileInfo);
        var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}
