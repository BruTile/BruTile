// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using BruTile.Cache;

namespace BruTile.Web
{
    public class HttpTileSource : ITileSource, IRequest
    {
        private readonly HttpTileProvider _provider;
        private readonly ITileSchema _tileSchema;

        public  HttpTileSource(ITileSchema tileSchema, string urlFormatter, IEnumerable<string> serverNodes = null, 
            string apiKey = null, string name = null, IPersistentCache<byte[]> persistentCache = null, 
            Func<Uri, byte[]> tileFetcher = null)
            : this(tileSchema, new BasicRequest(urlFormatter, serverNodes, apiKey), name, persistentCache, tileFetcher)
        {
        }

        public HttpTileSource(ITileSchema tileSchema, IRequest request, string name = null, 
            IPersistentCache<byte[]> persistentCache = null, Func<Uri, byte[]> tileFetcher = null)
        {
            _provider = new HttpTileProvider(request, persistentCache, tileFetcher);
            _tileSchema = tileSchema;
            Name = name ?? string.Empty;
        }

        public IPersistentCache<byte[]> PersistentCache { get { return _provider.PersistentCache; } }

        public Uri GetUri(TileInfo tileInfo)
        {
            return _provider.GetUri(tileInfo);
        }

        public ITileSchema Schema { get { return _tileSchema;  } }

        public string Name { get; set; }

        /// <summary>
        /// Gets the actual image content of the tile as byte array
        /// </summary>
        public virtual byte[] GetTile(TileInfo tileInfo)
        {
            return _provider.GetTile(tileInfo);
        }


    }
}
