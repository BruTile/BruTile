// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BruTile.Web;

public static class HttpTileSourceExtensions
{
    public static async Task<byte[][]> GetTilesAsync(this HttpTileSource tileSource, HttpClient httpClient, IEnumerable<TileInfo> tileInfos)
    {
        var tasks = tileInfos.Select(t => tileSource.GetTileAsync(httpClient, t)).ToArray();
        var result = Task.WhenAll(tasks);
        return await result.ConfigureAwait(false);
    }

    public static async Task<byte[]> GetTileAsync(this HttpTileSource tileSource, HttpClient httpClient, TileInfo tileInfo)
    {
        var bytes = tileSource.PersistentCache.Find(tileInfo.Index);
        if (bytes != null) return bytes;
        bytes = await httpClient.GetByteArrayAsync(tileSource.GetUri(tileInfo)).ConfigureAwait(false);
        if (bytes != null) tileSource.PersistentCache.Add(tileInfo.Index, bytes);
        return bytes;
    }
}
