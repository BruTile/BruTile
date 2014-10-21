// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Cache;
using System;
using System.Collections.Generic;

namespace BruTile.Web
{
    public class HttpTileSource : ITileSource
    {
        private readonly ITileSchema _tileSchema;
        private readonly WebTileProvider _webTileProvider;

        public  HttpTileSource(ITileSchema tileSchema, string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey = null, string name = null, IPersistentCache<byte[]> persistentCache = null, Func<Uri, byte[]> tileFetcher = null)
            : this(tileSchema, new BasicRequest(urlFormatter, serverNodes, apiKey), name, persistentCache, tileFetcher)
        {
        }

        public HttpTileSource(ITileSchema tileSchema, IRequest request, string name = null, IPersistentCache<byte[]> persistentCache = null, Func<Uri, byte[]> tileFetcher = null)
        {
            _webTileProvider = new WebTileProvider(request, persistentCache, tileFetcher);
            _tileSchema = tileSchema;
            Name = name ?? string.Empty;
        }

        public ITileProvider Provider { get { return _webTileProvider; } }
        public ITileSchema Schema { get { return _tileSchema;  } }
        public string Name { get; set; }
    }
}
