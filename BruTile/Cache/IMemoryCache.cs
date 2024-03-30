// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Cache;

internal interface IMemoryCache<T> : ITileCache<T>
{
    int MinTiles { get; set; }
    int MaxTiles { get; set; }
}
