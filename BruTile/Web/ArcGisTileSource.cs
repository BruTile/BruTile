// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;

namespace BruTile.Web;

public class ArcGisTileSource(string baseUrl, ITileSchema schema, IPersistentCache<byte[]> persistentCache = null,
    Func<Uri, CancellationToken, Task<byte[]>> fetchTile = null) 
        : TileSource(new HttpTileProvider(CreateArcGISRequest(baseUrl), persistentCache, fetchTile), schema)
{
    public string BaseUrl { get; } = baseUrl;

    private static BasicRequest CreateArcGISRequest(string baseUrl) => new($"{baseUrl}/tile/{{0}}/{{2}}/{{1}}");
}
