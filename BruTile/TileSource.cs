// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace BruTile;

/// <summary>
/// The default implementation of a <see cref="ITileSource"/>.
/// </summary>
/// <remarks>
/// Creates an instance of this class
/// </remarks>
/// <param name="tileProvider">The tile provider</param>
/// <param name="tileSchema">The tile schema</param>
public class TileSource(ITileProvider tileProvider, ITileSchema tileSchema) : ITileSource
{
    private readonly ITileProvider _provider = tileProvider;

    /// <summary>
    /// Gets a the Name of the tile source
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets the image content of the tile 
    /// </summary>
    public async Task<byte[]> GetTileAsync(TileInfo tileInfo, CancellationToken cancellationToken)
    {
        return await _provider.GetTileAsync(tileInfo, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a value indicating the tile schema
    /// </summary>
    public ITileSchema Schema { get; } = tileSchema;

    public override string ToString()
    {
        return $"[TileSource:{Name}]";
    }

    public Attribution Attribution { get; set; } = new Attribution();
}
