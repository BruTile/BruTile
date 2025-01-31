// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using BruTile.Cache;

namespace BruTile.FileSystem;

public class FileTileSource(ITileSchema tileSchema, string directory, string format, TimeSpan? cacheExpireTime = null,
    string? name = null, Attribution? attribution = null) : ILocalTileSource
{
    private readonly FileCache _fileCache = new(directory, format, cacheExpireTime ?? TimeSpan.Zero);

    public ITileSchema Schema { get; } = tileSchema;
    public string Name { get; } = name ?? directory;
    public Attribution Attribution { get; } = attribution ?? new Attribution();

    public Task<byte[]?> GetTileAsync(TileInfo tileInfo)
    {
        var bytes = _fileCache.Find(tileInfo.Index);
        return Task.FromResult(bytes);
    }
}
