// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;

namespace BruTile.Web;

public class HttpTileSource : ITileSource, IUrlBuilder, IDisposable
{
    private readonly Func<Uri, CancellationToken, Task<byte[]>>? _customTileFetchRequest;
    private readonly HttpClient? _httpClient;
    private bool disposed;
    private readonly HttpTileSourceDefinition definition;

    public HttpTileSource(
        ITileSchema tileSchema,
        string urlFormatter,
        IEnumerable<string>? serverNodes = null,
        string? apiKey = null,
        string? name = null,
        Func<Uri, CancellationToken, Task<byte[]>>? tileFetcher = null,
        Attribution? attribution = null,
        string? userAgent = null)
        : this(tileSchema, new BasicRequest(urlFormatter, serverNodes, apiKey), name ?? "", tileFetcher, attribution, userAgent)
    { }

    public HttpTileSource(
        ITileSchema tileSchema,
        IUrlBuilder urlBuilder,
        string? name = null,
        Func<Uri, CancellationToken, Task<byte[]>>? tileFetcher = null,
        Attribution? attribution = null,
        string? userAgent = null)
    {
        definition = new HttpTileSourceDefinition(tileSchema, urlBuilder, name ?? "", attribution ?? new Attribution(), userAgent);

        if (tileFetcher is not null)
            _customTileFetchRequest = tileFetcher;
        else
        {
            _httpClient = HttpClientBuilder.Build();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent ?? "If you use BruTile please specify a user-agent specific to your app");
        }
    }

    public ITileSchema Schema => definition.TileSchema;
    public string Name => definition.Name;
    public Attribution Attribution => definition.Attribution;
    public IPersistentCache<byte[]> PersistentCache { get; set; } = new NullCache();

    public Uri GetUri(TileInfo tileInfo)
    {
        return definition.UrlBuilder.GetUri(tileInfo);
    }

    /// <summary>
    /// Gets the actual image content of the tile as byte array
    /// </summary>
    public virtual async Task<byte[]?> GetTileAsync(TileInfo tileInfo, CancellationToken cancellationToken)
    {
        var bytes = PersistentCache.Find(tileInfo.Index);
        if (bytes != null)
            return bytes;

        if (_customTileFetchRequest is not null)
            bytes = await _customTileFetchRequest(definition.UrlBuilder.GetUri(tileInfo), cancellationToken).ConfigureAwait(false);
        else
            bytes = await FetchTileAsync(tileInfo, definition, cancellationToken).ConfigureAwait(false);

        if (bytes != null)
            PersistentCache.Add(tileInfo.Index, bytes);
        return bytes;
    }

    public void AddHeader(string key, string value)
    {
        if (_httpClient is null)
            throw new Exception("You can not add headers when using the tileFetcher method is set in the constructor.");

        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
    }

    public IEnumerable<string> GetHeaders(string key)
    {
        if (_httpClient is null)
            throw new Exception("You can not get headers when the custom tileFetcher method is set in the constructor.");

        return _httpClient.DefaultRequestHeaders.GetValues(key);
    }

    private async Task<byte[]> FetchTileAsync(TileInfo tileInfo, HttpTileSourceDefinition definition, CancellationToken cancellationToken)
    {
        if (_httpClient is null)
            throw new Exception($"{nameof(FetchTileAsync)} can not be used when the tileFetcher method is set in the constructor");

        return await _httpClient.GetByteArrayAsync(definition.UrlBuilder.GetUri(tileInfo), cancellationToken);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
