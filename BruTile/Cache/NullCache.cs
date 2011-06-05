using System;

namespace BruTile.Cache
{
    internal class NullCache : ITileCache<byte[]>
    {
        public void Add(TileIndex index, byte[] image)
        {
            //do nothing
        }

        public void Remove(TileIndex index)
        {
            throw new NotImplementedException(); //and should not
        }

        public byte[] Find(TileIndex index)
        {
            return null;
        }
    }
}