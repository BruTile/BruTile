// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Cache
{
    public interface ITileCache<T>
    {
        void Add(TileIndex index, T tile);
        void Remove(TileIndex index);
        T Find(TileIndex index);
    }
}