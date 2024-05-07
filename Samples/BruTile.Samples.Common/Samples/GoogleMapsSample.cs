// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Net.Http;
using BruTile.Predefined;
using BruTile.Web;

namespace BruTile.Samples.Common.Samples;

public static class GoogleMapsSample
{
    public static ITileSource Create()
    {
        return CreateGoogleTileSource("http://mt{s}.google.com/vt/lyrs=m@130&hl=en&x={x}&y={y}&z={z}");
    }

    public static IHttpTileSource CreateGoogleTileSource(string urlFormatter)
    {
        return new HttpTileSource(new GlobalSphericalMercator(), urlFormatter, ["0", "1", "2", "3"],
            configureHttpRequestMessage: ConfigureHttpRequestMessage);
    }

    private static void ConfigureHttpRequestMessage(HttpRequestMessage httpRequestMessage)
    {
        httpRequestMessage.Headers.TryAddWithoutValidation("Referer", "http://maps.google.com/");
        httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");
    }
}
