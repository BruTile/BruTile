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
    public class RequestTms : IRequestBuilder
    {
        IList<Uri> baseUrl;
        Dictionary<string, string> customParameters;
        string format;
        bool isSingleUrl = false; //If single url is added the request uses the same url for every resolution

        public RequestTms(Uri baseUrl, string format)
            : this(new List<Uri>() { baseUrl }, format)
        {
            isSingleUrl = true;
        }

        public RequestTms(Uri baseUrl, string format, Dictionary<string, string> dictionary)
            : this(new List<Uri>() { baseUrl }, format, dictionary)
        {
            isSingleUrl = true;
        }

        public RequestTms(IList<Uri> baseUrl, string format)
            : this(baseUrl, format, new Dictionary<string, string>())
        {
        }

        public RequestTms(IList<Uri> baseUrl, string format, Dictionary<string, string> customParameters)
        {
            this.baseUrl = baseUrl;
            this.format = format;
            this.customParameters = customParameters;
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="tile">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo tile)
        {
            System.Text.StringBuilder url = new StringBuilder();

            if (isSingleUrl)
            {
                url.AppendFormat(CultureInfo.InvariantCulture,
                      "{0}/{1}/{2}/{3}.{4}",
                      baseUrl[0], tile.Key.Level, tile.Key.Col, tile.Key.Row, format);
            }
            else
            {
                url.AppendFormat(CultureInfo.InvariantCulture,
                  "{0}/{1}/{2}.{3}",
                  baseUrl[tile.Key.Level], tile.Key.Col, tile.Key.Row, format);
            }


            AppendCustomParameters(url);
            return new Uri(url.ToString());
        }

        private void AppendCustomParameters(System.Text.StringBuilder url)
        {
            if (customParameters != null && customParameters.Count > 0)
            {
                bool first = true;
                foreach (string name in customParameters.Keys)
                {
                    string value = customParameters[name];
                    url.AppendFormat("{0}{1}={2}", first ? "?" : "&", name, value);
                    first = false;
                }
            }
        }

    }
}
