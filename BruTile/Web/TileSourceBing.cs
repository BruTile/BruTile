using System;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class TileSourceBing : ITileSource
    {
        TileSchema tileSchema;
        WebTileProvider tileProvider;

        public TileSourceBing(String url, string token, MapType mapType)
        {
            tileSchema = new SchemaBing();
            tileProvider = new WebTileProvider(new RequestBing(url, token, mapType));
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
