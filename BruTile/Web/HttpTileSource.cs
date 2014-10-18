// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Cache;
using System;
using System.Collections.Generic;

namespace BruTile.Web
{
    class HttpTileSource : ITileSource
    {
        private readonly ITileSchema _tileSchema;
        private readonly WebTileProvider _webTileProvider;

        public HttpTileSource(string urlFormatter, ITileSchema tileSchema, string title = null,
            IEnumerable<string> serverNodes = null, string apiKey = null,
            IPersistentCache<byte[]> persistentCache = null, Func<Uri, byte[]> tileFetcher = null)
            : this(new BasicRequest(urlFormatter, serverNodes, apiKey), tileSchema, title, persistentCache, tileFetcher)
        {
        }

        public HttpTileSource(IRequest request, ITileSchema tileSchema, string title = null,  IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, byte[]> tileFetcher = null)
        {
            _webTileProvider = new WebTileProvider(request, persistentCache, tileFetcher);
            _tileSchema = tileSchema;
            Title = title ?? string.Empty;
        }

        public ITileProvider Provider { get { return _webTileProvider; } }
        public ITileSchema Schema { get { return _tileSchema;  } }
        public string Title { get; set; }
    }
}
