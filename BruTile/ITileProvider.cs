// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace BruTile;

public interface ITileProvider
{
    async Task<byte[]?> GetTileAsync(TileInfo tileInfo)
    {
        return await GetTileAsync(tileInfo, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// May return null
    /// </summary>
    /// <param name="tileInfo"></param>
    /// <returns></returns>
    Task<byte[]?> GetTileAsync(TileInfo tileInfo, CancellationToken cancellationToken);
}
