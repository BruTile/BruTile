using System;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class BingTileSource : ITileSource
    {
        TileSchema tileSchema;
        WebTileProvider tileProvider;
        
        public BingTileSource(String url, string token, BingMapType mapType)
            :this(new BingRequest(url, token, mapType))
        {
        }

        public BingTileSource(BingRequest bingRequest)
        {
            tileSchema = new BingSchema();
            tileProvider = new WebTileProvider(bingRequest);
        }

        #region ITileSource Members

        public ITileProvider Provider
        {
            get { return tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return tileSchema; }
        }

        #endregion
    }
}
