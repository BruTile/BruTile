using System;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class BingTileSource : ITileSource
    {
        readonly TileSchema _tileSchema;
        readonly WebTileProvider _tileProvider;
        
        public BingTileSource(String url, string token, BingMapType mapType)
            :this(new BingRequest(url, token, mapType))
        {
        }

        public BingTileSource(BingRequest bingRequest)
        {
            _tileSchema = new BingSchema();
            _tileProvider = new WebTileProvider(bingRequest);
        }

        #region ITileSource Members

        public ITileProvider Provider
        {
            get { return _tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return _tileSchema; }
        }

        #endregion
    }
}
