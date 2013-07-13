// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Cache
{
    public class NullCache : IPersistentCache<byte[]>
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