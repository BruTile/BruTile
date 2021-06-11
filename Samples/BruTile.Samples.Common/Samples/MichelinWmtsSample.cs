using BruTile.Wmts;
using System.Linq;
using System.Net.Http;

namespace BruTile.Samples.Common.Samples
{
    public static class MichelinWmtsSample
    {
        public static ITileSource Create()
        {
            var httpClient = BruTile.Web.HttpClientBuilder.Build();
            var stream = httpClient.GetStreamAsync("https://bertt.github.io/wmts/capabilities/michelin.xml").Result;
            var michelinTileSource = WmtsParser.Parse(stream).First();
            return michelinTileSource;
        }
    }
}
