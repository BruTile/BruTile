// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Cache
{
    public interface ITileCache<T>
    {
        /// <summary>
        /// Adds a tile that corresponds to the index
        /// </summary>
        /// <param name="index">The index of the tile to add. If the tile already exists no exception is thrown.</param>
        /// <param name="tile">The tile data</param>
        void Add(TileIndex index, T tile);
        /// <summary>
        /// Removes the tile that corresponds with the index passed as argument. When the tile is not found no exception is thrown.
        /// </summary>
        /// <param name="index">The index of the tile to be removed.</param>
        void Remove(TileIndex index);
        /// <summary>
        /// Tries to find a tile that corresponds with the index. Returns null if not found.
        /// </summary>
        /// <param name="index">The index of the tile to find</param>
        /// <returns>The tile data that corresponds with the index or null.</returns>
        T Find(TileIndex index);
    }
}
