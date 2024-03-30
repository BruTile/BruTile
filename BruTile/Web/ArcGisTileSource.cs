// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;

namespace BruTile.Web;

public class ArcGisTileSource : TileSource
{
    public string BaseUrl { get; }

    public ArcGisTileSource(
            string baseUrl,
            ITileSchema schema,
            IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, CancellationToken, Task<byte[]>> fetchTile = null)
        : base(new HttpTileProvider(CreateArcGISRequest(baseUrl), persistentCache, fetchTile), schema)
    {
        BaseUrl = baseUrl;
    }

    private static IRequest CreateArcGISRequest(string baseUrl)
    {
        return new BasicRequest($"{baseUrl}/tile/{{0}}/{{2}}/{{1}}");
    }
}
