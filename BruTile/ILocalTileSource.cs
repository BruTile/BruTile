// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace BruTile;

public interface ILocalTileSource : ITileSource
{
    Task<byte[]?> GetTileAsync(TileInfo tileInfo);
}
