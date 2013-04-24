// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Cache;
using System;
using System.Net;

namespace BruTile.Web
{
    public class WebTileProvider : ITileProvider
    {
        private readonly Func<Uri, HttpWebRequest> _webRequestFactory;

        private ITileCache<byte[]> _memoryCache;
        public IRequest Request { get; private set; }

        public WebTileProvider(IRequest request = null, ITileCache<byte[]> memoryCache = null,
            Func<Uri, HttpWebRequest> webRequestFactory = null)
        {
            Request = request ?? new NullRequest();
            _memoryCache = memoryCache ?? new NullCache();
            _webRequestFactory = webRequestFactory ?? (uri => (HttpWebRequest) WebRequest.Create(uri));
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            var bytes = _memoryCache.Find(tileInfo.Index);
            if (bytes == null)
            {
                bytes = RequestHelper.FetchImage(_webRequestFactory(Request.GetUri(tileInfo)));
                if (bytes != null) _memoryCache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }
    }
}
