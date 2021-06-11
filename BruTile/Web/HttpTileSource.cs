// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using BruTile.Cache;

namespace BruTile.Web
{
    public class HttpTileSource : ITileSource, IRequest
    {
        private readonly Func<Uri, byte[]> _fetchTile;
        private readonly IRequest _request;
        private readonly HttpClient _httpClient = BruTile.Web.HttpClientBuilder.Build();

        public HttpTileSource(ITileSchema tileSchema, string urlFormatter, IEnumerable<string> serverNodes = null,
            string apiKey = null, string name = null, IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, byte[]> tileFetcher = null, Attribution attribution = null, string userAgent = null)
            : this(tileSchema, new BasicRequest(urlFormatter, serverNodes, apiKey), name, persistentCache, tileFetcher, attribution, userAgent)
        {
        }

        public HttpTileSource(ITileSchema tileSchema, IRequest request, string name = null,
            IPersistentCache<byte[]> persistentCache = null, Func<Uri, byte[]> tileFetcher = null, 
            Attribution attribution = null, string userAgent = null)
        {
            _request = request ?? new NullRequest();
            PersistentCache = persistentCache ?? new NullCache();
            _fetchTile = tileFetcher ?? FetchTile;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent ?? "If you use BruTile please specify a user-agent specific to your app");
            Schema = tileSchema;
            Name = name ?? string.Empty;
            Attribution = attribution ?? new Attribution();
        }

        public IPersistentCache<byte[]> PersistentCache { get; set; }

        public Uri GetUri(TileInfo tileInfo)
        {
            return _request.GetUri(tileInfo);
        }

        public ITileSchema Schema { get; }

        public string Name { get; set; }

        public Attribution Attribution { get; set; }

        /// <summary>
        /// Gets the actual image content of the tile as byte array
        /// </summary>
        public virtual byte[] GetTile(TileInfo tileInfo)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);
            if (bytes != null) return bytes;
            bytes = _fetchTile(_request.GetUri(tileInfo));
            if (bytes != null) PersistentCache.Add(tileInfo.Index, bytes);
            return bytes;
        }

        private byte[] FetchTile(Uri arg)
        {
            return _httpClient.GetByteArrayAsync(arg).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}