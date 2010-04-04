// Copyright 2009 - Tim Ebben (Geodan)
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
using System.Text;
using System.Globalization;

namespace BruTile.Web
{
    public class ArcgisRequest : IRequest
    {
        Uri baseUrl;
        Dictionary<string, string> customParameters;
        string format;

        public ArcgisRequest(Uri baseUrl, string format)
            : this(baseUrl, format, new Dictionary<string, string>())
        {
        }

        public ArcgisRequest(Uri baseUrl, string format, Dictionary<string, string> customParameters)
        {
            this.baseUrl = baseUrl;
            this.format = format;
            this.customParameters = customParameters;
        }

        public Uri GetUri(TileInfo tile)
        {
            System.Text.StringBuilder url = new StringBuilder();

            url.AppendFormat(CultureInfo.InvariantCulture,
              "{0}/{1}/{2}/{3}.{4}",
              baseUrl, levelToHex(tile.Key.Level), rowToHex(tile.Key.Row), columnToHex(tile.Key.Col), format);
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

        string levelToHex(int zoom)
        {
            String zoomUrl;

            if (zoom < 10)
                zoomUrl = "L0" + (String)zoom.ToString();
            else
                zoomUrl = "L" + (String)zoom.ToString();

            return zoomUrl;
        }

        string columnToHex(int x)
        {
            String hexX;
            return hexX = "C" + String.Format("{0:x8}", x); //Column (xTile naar hex met min/max 8 getallen aanvullen met 0)
        }

        string rowToHex(int y)
        {
            String hexY;
            return hexY = "R" + String.Format("{0:x8}", y); //Row (yTile naar hex met min/max 8 getallen aanvullen met 0)
        }

    }
}
