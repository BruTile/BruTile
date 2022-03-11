using BruTile.Predefined;
using BruTile.Web;
using System;
using System.Threading.Tasks;

namespace BruTile.Samples.Common.Samples
{
    public static class GoogleMapsSample
    {
        public static ITileSource Create()
        {
            return CreateGoogleTileSource("http://mt{s}.google.com/vt/lyrs=m@130&hl=en&x={x}&y={y}&z={z}");
        }

        public static ITileSource CreateGoogleTileSource(string urlFormatter)
        {
            return new HttpTileSource(new GlobalSphericalMercator(), urlFormatter, new[] { "0", "1", "2", "3" },
                tileFetcher: FetchGoogleTileAsync);
        }

        private static async Task<byte[]> FetchGoogleTileAsync(Uri arg)
        {
            var httpClient = BruTile.Web.HttpClientBuilder.Build();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "http://maps.google.com/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");

            return await httpClient.GetByteArrayAsync(arg).ConfigureAwait(false);
        }

    }
}
