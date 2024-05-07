// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using BruTile.Cache;

namespace BruTile.Web;

public class ArcGisHttpTileSource(string baseUrl, ITileSchema schema, IPersistentCache<byte[]> persistentCache = null)
        : HttpTileSource(schema, CreateArcGisUrlBuilder(baseUrl), persistentCache: persistentCache)
{
    public string BaseUrl { get; } = baseUrl;

    private static BasicRequest CreateArcGisUrlBuilder(string baseUrl) => new($"{baseUrl}/tile/{{0}}/{{2}}/{{1}}");
}
