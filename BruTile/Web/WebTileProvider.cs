// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Net;
using BruTile.Cache;

namespace BruTile.Web
{
    public class WebTileProvider : ITileProvider
    {
        private readonly Func<Uri, HttpWebRequest> _webRequestFactory; 

        public ITileCache<byte[]> PersistentCache { get; private set; }
        public IRequest Request { get; private set; }

        public WebTileProvider(IRequest request = null, ITileCache<byte[]> persistentCache = null,
            Func<Uri, HttpWebRequest> webRequestFactory = null)
        {
            Request = request ?? new NullRequest();
            PersistentCache = persistentCache ?? new NullCache();
            _webRequestFactory = webRequestFactory ?? (uri => (HttpWebRequest) WebRequest.Create(uri));
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);
            if (bytes == null)
            {
                bytes = RequestHelper.FetchImage(_webRequestFactory(Request.GetUri(tileInfo)));
                if (bytes != null)
                    PersistentCache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }
    }
}
