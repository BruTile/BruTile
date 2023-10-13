// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BruTile.Cache;

namespace BruTile.Web
{
    public class HttpTileSource : ITileSource, IRequest
    {
        private readonly Func<Uri, Task<byte[]>> _fetchTile;
        private readonly IRequest _request;
        private readonly HttpClient _httpClient = HttpClientBuilder.Build();

        public HttpTileSource(ITileSchema tileSchema, string urlFormatter, IEnumerable<string> serverNodes = null,
            string apiKey = null, Func<Uri, Task<byte[]>> tileFetcher = null, string userAgent = null)
            : this(tileSchema, new BasicRequest(urlFormatter, serverNodes, apiKey), tileFetcher, userAgent)
        { }

        public HttpTileSource(ITileSchema tileSchema, IRequest request, Func<Uri, Task<byte[]>> tileFetcher = null, string userAgent = null)
        {
            _request = request ?? new NullRequest();
            _fetchTile = tileFetcher ?? FetchTileAsync;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent ?? "If you use BruTile please specify a user-agent specific to your app");
            Schema = tileSchema;
        }

        public IPersistentCache<byte[]> PersistentCache { get; set; } = new NullCache();

        public Uri GetUri(TileInfo tileInfo)
        {
            return _request.GetUri(tileInfo);
        }

        public ITileSchema Schema { get; }

        public string Name { get; set; } = string.Empty;

        public Attribution Attribution { get; set; } = new Attribution();

        /// <summary>
        /// Gets the actual image content of the tile as byte array
        /// </summary>
        public virtual async Task<byte[]> GetTileAsync(TileInfo tileInfo)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);
            if (bytes != null) return bytes;
            bytes = await _fetchTile(_request.GetUri(tileInfo)).ConfigureAwait(false);
            if (bytes != null) PersistentCache.Add(tileInfo.Index, bytes);
            return bytes;
        }

        public void AddHeader(string key, string value)
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }

        public IEnumerable<string> GetHeaders(string key)
        {
            return _httpClient.DefaultRequestHeaders.GetValues(key);
        }

        private async Task<byte[]> FetchTileAsync(Uri arg)
        {
            return await _httpClient.GetByteArrayAsync(arg).ConfigureAwait(false);
        }
    }
}
