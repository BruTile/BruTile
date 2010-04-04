using System;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class BingTileSource : ITileSource
    {
        TileSchema tileSchema;
        WebTileProvider tileProvider;

        public BingTileSource(String url, string token, MapType mapType)
        {
            tileSchema = new BingSchema();
            tileProvider = new WebTileProvider(new BingRequest(url, token, mapType));
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
