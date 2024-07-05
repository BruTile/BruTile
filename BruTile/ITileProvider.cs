// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace BruTile;

[Obsolete("Use ILocalTileSource or IHttpTileSource instead", true)]
public interface ITileProvider
{
    Task<byte[]?> GetTileAsync(TileInfo tileInfo);
}