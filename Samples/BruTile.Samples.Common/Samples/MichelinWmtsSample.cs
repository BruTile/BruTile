// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using BruTile.Wmts;

namespace BruTile.Samples.Common.Samples;

public static class MichelinWmtsSample
{
    public static ITileSource Create()
    {
        var httpClient = Web.HttpClientBuilder.Build();
        var stream = httpClient.GetStreamAsync("https://bertt.github.io/wmts/capabilities/michelin.xml").Result;
        var michelinTileSource = WmtsParser.Parse(stream).First();
        return michelinTileSource;
    }
}
