using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BruTile.Cache
{
    class CachingTileProvider : ITileProvider
    {
        private ITileProvider provider;
        private ITileCache<byte[]> cache;

        public CachingTileProvider(ITileProvider provider, ITileCache<byte[]> cache)
        {
            this.provider = provider;
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = cache.Find(tileInfo.Index);

            if (bytes == null)
            {
                bytes = provider.GetTile(tileInfo);
                if (bytes != null) cache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }
    }
}
