// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Cache;
using System;
using System.Net;

namespace BruTile.Web
{
    public class WebTileProvider : ITileProvider
    {
        private readonly Func<Uri, HttpWebRequest> _webRequestFactory;

        private IPersistentCache<byte[]> _persistentCache;
        public IRequest Request { get; private set; }

        public WebTileProvider(IRequest request = null, IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, HttpWebRequest> webRequestFactory = null)
        {
            Request = request ?? new NullRequest();
            _persistentCache = persistentCache ?? new NullCache();
            _webRequestFactory = webRequestFactory ?? (uri => (HttpWebRequest) WebRequest.Create(uri));
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            var bytes = _persistentCache.Find(tileInfo.Index);
            if (bytes == null)
            {
                bytes = RequestHelper.FetchImage(_webRequestFactory(Request.GetUri(tileInfo)));
                if (bytes != null) _persistentCache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }
    }
}
