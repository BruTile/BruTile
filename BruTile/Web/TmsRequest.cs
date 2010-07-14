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

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BruTile.Web
{
    public class TmsRequest : IRequest
    {
        readonly IList<Uri> _baseUrl;
        readonly Dictionary<string, string> _customParameters;
        readonly string _format;
        readonly bool _isSingleUrl; //If single url is added the request uses the same url for every resolution

        public TmsRequest(Uri baseUrl, string format)
            : this(new List<Uri> { baseUrl }, format)
        {
            _isSingleUrl = true;
        }

        public TmsRequest(Uri baseUrl, string format, Dictionary<string, string> dictionary)
            : this(new List<Uri> { baseUrl }, format, dictionary)
        {
            _isSingleUrl = true;
        }

        public TmsRequest(IList<Uri> baseUrl, string format)
            : this(baseUrl, format, new Dictionary<string, string>())
        {
        }

        public TmsRequest(IList<Uri> baseUrl, string format, Dictionary<string, string> customParameters)
        {
            _baseUrl = baseUrl;
            _format = format;
            _customParameters = customParameters;
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            var url = new StringBuilder();

            if (_isSingleUrl)
            {
                url.AppendFormat(CultureInfo.InvariantCulture,
                      "{0}/{1}/{2}/{3}.{4}",
                      _baseUrl[0], info.Index.Level, info.Index.Col, info.Index.Row, _format);
            }
            else
            {
                url.AppendFormat(CultureInfo.InvariantCulture,
                  "{0}/{1}/{2}.{3}",
                  _baseUrl[info.Index.Level], info.Index.Col, info.Index.Row, _format);
            }


            AppendCustomParameters(url);
            return new Uri(url.ToString());
        }

        private void AppendCustomParameters(StringBuilder url)
        {
            if (_customParameters != null && _customParameters.Count > 0)
            {
                bool first = true;
                foreach (string name in _customParameters.Keys)
                {
                    string value = _customParameters[name];
                    url.AppendFormat("{0}{1}={2}", first ? "?" : "&", name, value);
                    first = false;
                }
            }
        }

    }
}
