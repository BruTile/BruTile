using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BruTile.Web;

namespace BruTile.Wmts
{
    public class WmtsRequest : IRequest
    {
        public const string XTag = "{TileCol}";
        public const string YTag = "{TileRow}";
        public const string ZTag = "{TileMatrix}";
        public const string TileMatrixSetTag = "{TileMatrixSet}";
        public const string StyleTag = "{Style}";
        private readonly IList<ResourceUrl> _resourceUrls;
        private int _resourceUrlCounter;
        
        public WmtsRequest(IEnumerable<ResourceUrl> resourceUrls)
        {
            _resourceUrls = resourceUrls.ToList();
        }

        public Uri GetUri(TileInfo info)
        {
            if (_resourceUrlCounter >= _resourceUrls.Count()) _resourceUrlCounter = 0;
            var urlFormatter = _resourceUrls[_resourceUrlCounter];
            _resourceUrlCounter++;
            var stringBuilder = new StringBuilder(urlFormatter.Template);
            stringBuilder.Replace(XTag, info.Index.Col.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(YTag, info.Index.Row.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(ZTag, info.Index.Level);
            return new Uri(stringBuilder.ToString());
        }
    }
}
