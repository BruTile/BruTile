// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BruTile.Web;

namespace BruTile.Wmsc
{
    public class WmscRequest : IRequest
    {
        readonly Uri _baseUrl;
        readonly IDictionary<string, string> _customParameters;
        readonly IList<string> _layers;
        private readonly ITileSchema _schema;
        readonly IList<string> _styles;
        private readonly string _version;

        public WmscRequest(Uri baseUrl, ITileSchema schema, IList<string> layers, IList<string> styles, IDictionary<string, string> customParameters = null, string version = null)
        {
            _baseUrl = baseUrl;
            _customParameters = customParameters;
            _layers = layers;
            _schema = schema;
            _styles = styles;
            _version = version;
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            var url = new StringBuilder(_baseUrl.AbsoluteUri);
            url.Append("&SERVICE=WMS");
            if (!string.IsNullOrEmpty(_version)) url.AppendFormat("&VERSION={0}", _version);
            url.Append("&REQUEST=GetMap");
            url.AppendFormat("&BBOX={0}", TileTransform.TileToWorld(new TileRange(info.Index.Col, info.Index.Row), info.Index.Level, _schema));
            url.AppendFormat("&FORMAT={0}", _schema.Format);
            url.AppendFormat("&WIDTH={0}", _schema.GetTileWidth(info.Index.Level));
            url.AppendFormat("&HEIGHT={0}", _schema.GetTileHeight(info.Index.Level));
            var crsFormat = !string.IsNullOrEmpty(_version) && string.CompareOrdinal(_version, "1.3.0") >= 0 ? "&CRS={0}" : "&SRS={0}";
            url.AppendFormat(crsFormat, _schema.Srs);
            url.AppendFormat("&LAYERS={0}", ToCommaSeparatedValues(_layers));
            if (_styles != null && _styles.Count > 0) url.AppendFormat("&STYLES={0}", ToCommaSeparatedValues(_styles));
            AppendCustomParameters(url);
            return new Uri(url.ToString());
        }

        private static string ToCommaSeparatedValues(IEnumerable<string> items)
        {
            var result = new StringBuilder();
            foreach (string str in items)
            {
                result.AppendFormat(CultureInfo.InvariantCulture, ",{0}", str);
            }
            if (result.Length > 0) result.Remove(0, 1);
            return result.ToString();
        }

        private void AppendCustomParameters(StringBuilder url)
        {
            if (_customParameters != null && _customParameters.Count > 0)
            {
                foreach (string name in _customParameters.Keys)
                {
                    url.AppendFormat("&{0}={1}", name, _customParameters[name]);
                }
            }
        }
    }
}
