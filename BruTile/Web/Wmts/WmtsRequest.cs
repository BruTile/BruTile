using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BruTile.Web.Wmts
{
    public class WmtsRequest : IRequest
    {
        private const string XTag = "{TileCol}";
        private const string YTag = "{TileRow}";
        private const string ZTag = "{TileMatrix}";
        private const string TileMatrixSetTag = "{TileMatrixSet}";
        private const string StyleTag = "{Style}";
        private readonly IEnumerator<ResourceUrl> _resourceUrlEnumerator;
        private readonly IEnumerable<ResourceUrl> _resourceUrls;
        private readonly string _tileMatrixLink;
        private readonly string _style;
        
        public WmtsRequest(IEnumerable<ResourceUrl> resourceUrls, string tileMatrixLink, string style)
        {
            _resourceUrls = resourceUrls;
            _tileMatrixLink = tileMatrixLink;
            _style = style;
            _resourceUrlEnumerator = _resourceUrls.GetEnumerator();
        }

        public Uri GetUri(TileInfo info)
        {
            if (!_resourceUrlEnumerator.MoveNext())
            {
                _resourceUrlEnumerator.Reset();
                _resourceUrlEnumerator.MoveNext();
            }
            var urlFormatter = _resourceUrlEnumerator.Current;
            var stringBuilder = new StringBuilder(urlFormatter.Template);
            stringBuilder.Replace(XTag, info.Index.Col.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(YTag, info.Index.Row.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(ZTag, info.Index.Level.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(TileMatrixSetTag, _tileMatrixLink);
            stringBuilder.Replace(StyleTag, _style);
            return new Uri(stringBuilder.ToString());
        }
    }
}
