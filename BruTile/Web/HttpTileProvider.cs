// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using BruTile.Cache;

namespace BruTile.Web
{
    public class HttpTileProvider : ITileProvider, IRequest
    {
        private readonly Func<Uri, byte[]> _fetchTile;
        private readonly IRequest _request;

        public HttpTileProvider(IRequest request = null, IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, byte[]> fetchTile = null)
        {
            _request = request ?? new NullRequest();
            PersistentCache = persistentCache ?? new NullCache();
            _fetchTile = fetchTile ?? (RequestHelper.FetchImage);
        }

        public IPersistentCache<byte[]> PersistentCache { get; }

        public Uri GetUri(TileInfo tileInfo)
        {
            return _request.GetUri(tileInfo);
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);
            if (bytes != null) return bytes;
            bytes = _fetchTile(_request.GetUri(tileInfo));
            if (bytes != null) PersistentCache.Add(tileInfo.Index, bytes);
            return bytes;
        }
    }
}