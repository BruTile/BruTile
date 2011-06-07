#region License

// Copyright 2008 - Paul den Dulk (Geodan)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BruTile.Web
{
    public class WmscRequest : IRequest
    {
        readonly ITileSchema _schema;
        readonly Uri _baseUrl;
        readonly IDictionary<string, string> _customParameters;
        readonly IList<string> _layers;
        readonly IList<string> _styles;

        public WmscRequest(Uri baseUrl, ITileSchema schema, IList<string> layers, IList<string> styles, IDictionary<string, string> customParameters)
        {
            _baseUrl = baseUrl;
            _schema = schema;
            _customParameters = customParameters;
            _layers = layers;
            _styles = styles;
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
            url.Append("&REQUEST=GetMap");
            url.AppendFormat("&BBOX={0}", info.Extent);
            url.AppendFormat("&FORMAT={0}", _schema.Format);
            url.AppendFormat("&WIDTH={0}", _schema.Width);
            url.AppendFormat("&HEIGHT={0}", _schema.Height);
            url.AppendFormat("&SRS={0}", _schema.Srs);
            url.AppendFormat("&LAYERS={0}", ToCommaSeparatedValues(_layers));
            //uri.AppendFormat("&STYLES={0}", ToCommaSeparatedValues(_styles));

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
