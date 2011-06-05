using System;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class BingTileSource : ITileSource
    {
        public ITileProvider Provider { get; private set; }
        public ITileSchema Schema { get; private set; }

        public BingTileSource(String url, string token, BingMapType mapType)
            :this(new BingRequest(url, token, mapType))
        {
        }

        public BingTileSource(IRequest bingRequest)
        {
            Schema = new BingSchema();
            Provider = new WebTileProvider(bingRequest);
        }
    }
}
