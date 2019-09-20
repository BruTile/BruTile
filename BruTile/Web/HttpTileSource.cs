// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using BruTile.Cache;

namespace BruTile.Web
{
    public class HttpTileSource : ITileSource, IRequest
    {
        private readonly HttpTileProvider _provider;

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
            _provider = new HttpTileProvider(request, persistentCache, tileFetcher, userAgent);
            Schema = tileSchema;
            Name = name ?? string.Empty;
            Attribution = attribution ?? new Attribution();
        }

        public IPersistentCache<byte[]> PersistentCache => _provider.PersistentCache;

        public Uri GetUri(TileInfo tileInfo)
        {
            return _provider.GetUri(tileInfo);
        }

        public ITileSchema Schema { get; }

        public string Name { get; set; }

        public Attribution Attribution { get; set; }

        /// <summary>
        /// Gets the actual image content of the tile as byte array
        /// </summary>
        public virtual byte[] GetTile(TileInfo tileInfo)
        {
            return _provider.GetTile(tileInfo);
        }
    }
}