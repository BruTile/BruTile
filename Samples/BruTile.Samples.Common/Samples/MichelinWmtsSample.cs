// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using System.Net.Http;
using BruTile.Wmts;

namespace BruTile.Samples.Common.Samples;

public static class MichelinWmtsSample
{
    public static ITileSource Create()
    {
        using var httpClient = new HttpClient();
        var stream = httpClient.GetStreamAsync("https://bertt.github.io/wmts/capabilities/michelin.xml").Result;
        var michelinTileSource = WmtsCapabilitiesParser.Parse(stream).First();
        return michelinTileSource;
    }
}
