// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Cache
{
    /// <summary>
    /// All caches that touch the disk (e.g.: FileCache, DbCache and MBTilesCache) should be derived
    /// from IPersistentCache so that it can be used in an interface where only a persistent cache 
    /// would make sense
    /// </summary>
    public interface IPersistentCache<T> : ITileCache<T>
    {
    }
}
