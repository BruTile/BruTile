// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BruTile.Cache;

namespace BruTile.Web
{
    public class HttpTileProvider : ITileProvider, IRequest
    {
        private readonly Func<Uri, Task<byte[]>> _fetchTile;
        private readonly IRequest _request;
        private readonly HttpClient _httpClient = BruTile.Web.HttpClientBuilder.Build();

        public HttpTileProvider(IRequest request = null, IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, Task<byte[]>> fetchTile = null, string userAgent = null)
        {
            _request = request ?? new NullRequest();
            PersistentCache = persistentCache ?? new NullCache();
            _fetchTile = fetchTile ?? FetchTileAsync;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent ?? "BruTile Tile Library");
        }
        
        private async Task<byte[]> FetchTileAsync(Uri arg)
        {
            return await _httpClient.GetByteArrayAsync(arg).ConfigureAwait(false);
        }

        public IPersistentCache<byte[]> PersistentCache { get; }

        public Uri GetUri(TileInfo tileInfo)
        {
            return _request.GetUri(tileInfo);
        }

        public async Task<byte[]> GetTileAsync(TileInfo tileInfo)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);
            if (bytes != null) return bytes;
            bytes = await _fetchTile(_request.GetUri(tileInfo)).ConfigureAwait(false);
            if (bytes != null) PersistentCache.Add(tileInfo.Index, bytes);
            return bytes;
        }
    }
}