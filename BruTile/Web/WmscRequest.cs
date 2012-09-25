// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BruTile.Web
{
    public class WmscRequest : IRequest
    {
        //readonly ITileSchema _schema;
        readonly Uri _baseUrl;
        readonly IDictionary<string, string> _customParameters;
        readonly IList<string> _layers;
        readonly IList<string> _styles;
        
        private readonly string _format;
        private readonly int _width, _height;
        private readonly string _srs;

        public WmscRequest(Uri baseUrl, ITileSchema schema, IList<string> layers, IList<string> styles, IDictionary<string, string> customParameters)
        {
            _baseUrl = baseUrl;
            _format = schema.Format;
            _width = schema.Width;
            _height = schema.Height;
            _srs = schema.Srs;
            //_schema = schema;
            _customParameters = customParameters;
            _layers = layers;
            _styles = styles;
        }

        public WmscRequest(Uri baseUrl, ITileSchema schema, IList<string> layers, IList<string> styles, IDictionary<string, string> customParameters, string version)
            : this(baseUrl, schema, layers, styles, customParameters)
        {
            Version = version;
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            var url = new StringBuilder(_baseUrl.AbsoluteUri);

            //TODO: look at .net's UriBuilder for improvement
            //http://msdn.microsoft.com/en-us/library/system.uribuilder.aspx
            //note that the first & assumes a preceiding argument, this is not
            //always the case.

            url.Append("&SERVICE=WMS");
            if (!string.IsNullOrEmpty(Version))
                url.AppendFormat("&VERSION={0}", Version);
            url.Append("&REQUEST=GetMap");
            url.AppendFormat("&BBOX={0}", info.Extent);
            url.AppendFormat("&FORMAT={0}", _format/*_schema.Format*/);
            url.AppendFormat("&WIDTH={0}", _width/*_schema.Width*/);
            url.AppendFormat("&HEIGHT={0}", _height/*_schema.Height*/);
            var crsFormat = !string.IsNullOrEmpty(Version) && String.Compare(Version, "1.3.0") >= 0 ? "&CRS={0}" : "&SRS={0}";
            url.AppendFormat(crsFormat, _srs/*_schema.Srs*/);
            url.AppendFormat("&LAYERS={0}", ToCommaSeparatedValues(_layers));
            if (_styles != null && _styles.Count > 0) url.AppendFormat("&STYLES={0}", ToCommaSeparatedValues(_styles));

            AppendCustomParameters(url);

            return new Uri(url.ToString());
        }

        /// <summary>
        /// Sets or gets the WMS Version to use in GetMap requests
        /// </summary>
        public string Version
        {
            get;
            set;
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
