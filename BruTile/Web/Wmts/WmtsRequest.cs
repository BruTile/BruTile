using System;

namespace BruTile.Web.Wmts
{
    class WmtsRequest : IRequest
    {
        public WmtsRequest()
        {
        }

        public Uri GetUri(TileInfo info)
        {
            return new Uri("");
        }
    }
}
