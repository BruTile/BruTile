// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;

namespace BruTile.Web;

public class HttpTileSource(
    ITileSchema tileSchema,
    IUrlBuilder urlBuilder,
    string? name = null,
    IPersistentCache<byte[]>? persistentCache = null,
    Attribution? attribution = null,
    Action<HttpRequestMessage>? configureHttpRequestMessage = null) : IHttpTileSource, IUrlBuilder
{
    private readonly IUrlBuilder _urlBuilder = urlBuilder;

    public HttpTileSource(
        ITileSchema tileSchema,
        string urlFormatter,
        IEnumerable<string>? serverNodes = null,
        string? apiKey = null,
        string? name = null,
        IPersistentCache<byte[]>? persistentCache = null,
        Attribution? attribution = null,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null)
        : this(tileSchema, new BasicUrlBuilder(urlFormatter, serverNodes, apiKey), name ?? "", persistentCache, attribution, configureHttpRequestMessage)
    { }

    public string Name { get; set; } = name ?? "";
    public ITileSchema Schema => tileSchema;
    public Attribution Attribution { get; set; } = attribution ?? new Attribution();
    public IPersistentCache<byte[]> PersistentCache { get; set; } = persistentCache ?? new NullCache();
    public Action<HttpRequestMessage>? ConfigureHttpRequestMessage => configureHttpRequestMessage;

    public Uri GetUrl(TileInfo tileInfo) => _urlBuilder.GetUrl(tileInfo);

    /// <summary>
    /// Gets the actual image content of the tile as byte array
    /// </summary>
    public virtual async Task<byte[]?> GetTileAsync(HttpClient httpClient, TileInfo tileInfo,
        CancellationToken? cancellationToken = null)
    {
        var bytes = PersistentCache.Find(tileInfo.Index);

        if (bytes != null)
            return bytes;

        bytes = await GetTileAsync(httpClient, tileInfo, cancellationToken ?? CancellationToken.None);

        if (bytes != null)
            PersistentCache.Add(tileInfo.Index, bytes);

        return bytes;
    }

    private async Task<byte[]?> GetTileAsync(HttpClient httpClient, TileInfo tileInfo,
        CancellationToken cancellationToken)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, GetUrl(tileInfo));
        if (ConfigureHttpRequestMessage is not null)
            ConfigureHttpRequestMessage(requestMessage);
        if (httpClient.DefaultRequestHeaders.UserAgent.Count == 0 && requestMessage.Headers.UserAgent.Count == 0)
            throw new Exception("Set a User-Agent header that is specific to your application or to this tile service.");
        using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}
