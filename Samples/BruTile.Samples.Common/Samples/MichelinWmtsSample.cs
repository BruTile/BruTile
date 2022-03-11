using BruTile.Wmts;
using System.Linq;

namespace BruTile.Samples.Common.Samples
{
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
}
