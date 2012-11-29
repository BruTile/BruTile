// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Cache
{
    [Serializable]
    public class CachingTileProvider : ITileProvider
    {
        private readonly ITileProvider _provider;
        private readonly ITileCache<byte[]> _cache;

        public CachingTileProvider(ITileProvider provider, ITileCache<byte[]> cache)
        {
            _provider = provider;
            _cache = cache;
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = _cache.Find(tileInfo.Index);

            if (bytes == null)
            {
                bytes = _provider.GetTile(tileInfo);
                if (bytes != null) _cache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }
    }
}