// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

#nullable enable

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BruTile;

/// <summary>
/// Interface for a tile source.
/// </summary>
public interface IHttpTileSource : ITileSource
{
    Task<byte[]?> GetTileAsync(HttpClient httpClient, TileInfo tileInfo, CancellationToken cancellation);
}
