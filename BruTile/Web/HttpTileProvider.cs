// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;

namespace BruTile.Web
{
    public class HttpTileProvider : ITileProvider, IRequest
    {
        private readonly Func<Uri, CancellationToken, Task<byte[]>> _fetchTile;
        private readonly IRequest _request;
        private readonly HttpClient _httpClient = HttpClientBuilder.Build();

        public HttpTileProvider(IRequest request = null, IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, CancellationToken, Task<byte[]>> fetchTile = null, string userAgent = null)
        {
            _request = request ?? new NullRequest();
            PersistentCache = persistentCache ?? new NullCache();
            _fetchTile = fetchTile ?? FetchTileAsync;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent ?? "BruTile Tile Library");
        }

        private async Task<byte[]> FetchTileAsync(Uri arg, CancellationToken cancellationToken)
        {
#if NET6_0_OR_GREATER
            return await _httpClient.GetByteArrayAsync(arg, cancellationToken).ConfigureAwait(false);
#else
            return await _httpClient.GetByteArrayAsync(arg).ConfigureAwait(false);
#endif
        }

        public IPersistentCache<byte[]> PersistentCache { get; }

        public Uri GetUri(TileInfo tileInfo)
        {
            return _request.GetUri(tileInfo);
        }

        public async Task<byte[]> GetTileAsync(TileInfo tileInfo, CancellationToken cancellationToken)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);
            if (bytes != null) return bytes;
            bytes = await _fetchTile(_request.GetUri(tileInfo), cancellationToken).ConfigureAwait(false);
            if (bytes != null) PersistentCache.Add(tileInfo.Index, bytes);
            return bytes;
        }
    }
}
