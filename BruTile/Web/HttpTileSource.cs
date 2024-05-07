// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;
using BruTile.Extensions;

namespace BruTile.Web;

public class HttpTileSource(
    ITileSchema tileSchema,
    IUrlBuilder urlBuilder,
    string? name = null,
    IPersistentCache<byte[]>? persistentCache = null,
    Attribution? attribution = null,
    Action<HttpRequestMessage>? configureHttpRequestMessage = null) : IHttpTileSource, IUrlBuilder
{
    private readonly HttpTileSourceDefinition definition = new(tileSchema, urlBuilder, name ?? "", attribution ?? new Attribution(), configureHttpRequestMessage);

    public HttpTileSource(
        ITileSchema tileSchema,
        string urlFormatter,
        IEnumerable<string>? serverNodes = null,
        string? apiKey = null,
        string? name = null,
        IPersistentCache<byte[]>? persistentCache = null,
        Attribution? attribution = null,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null)
        : this(tileSchema, new BasicRequest(urlFormatter, serverNodes, apiKey), name ?? "", persistentCache, attribution, configureHttpRequestMessage)
    { }

    public ITileSchema Schema => definition.TileSchema;
    public string Name => definition.Name;
    public Attribution Attribution => definition.Attribution;
    public IPersistentCache<byte[]> PersistentCache { get; set; } = persistentCache ?? new NullCache();

    public Uri GetUrl(TileInfo tileInfo)
    {
        return definition.UrlBuilder.GetUrl(tileInfo);
    }

    /// <summary>
    /// Gets the actual image content of the tile as byte array
    /// </summary>
    public virtual async Task<byte[]?> GetTileAsync(HttpClient httpClient, TileInfo tileInfo,
        CancellationToken cancellationToken)
    {
        var bytes = PersistentCache.Find(tileInfo.Index);

        if (bytes != null)
            return bytes;

        bytes = await httpClient.GetTileAsync(tileInfo, definition, cancellationToken);

        if (bytes != null)
            PersistentCache.Add(tileInfo.Index, bytes);

        return bytes;
    }
}
