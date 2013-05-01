// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Cache;
using System;
using System.Net;

namespace BruTile.Web
{
    public class WebTileProvider : ITileProvider
    {
        private readonly IPersistentCache<byte[]> _persistentCache;
        private readonly IRequest _request;
        private readonly Func<Uri, HttpWebRequest> _webRequestFactory;

        public WebTileProvider(IRequest request = null, IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, HttpWebRequest> webRequestFactory = null)
        {
            _request = request ?? new NullRequest();
            _persistentCache = persistentCache ?? new NullCache();
            _webRequestFactory = webRequestFactory ?? (uri => (HttpWebRequest) WebRequest.Create(uri));
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            var bytes = _persistentCache.Find(tileInfo.Index);
            if (bytes == null)
            {
                bytes = RequestHelper.FetchImage(_webRequestFactory(_request.GetUri(tileInfo)));
                if (bytes != null) _persistentCache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }
    }
}
