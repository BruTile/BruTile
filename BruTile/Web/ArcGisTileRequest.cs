// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace BruTile.Web
{
    public class ArcGisTileRequest : IRequest
    {
        readonly Uri _baseUrl;
        readonly Dictionary<string, string> _customParameters;
        readonly string _format;

        public ArcGisTileRequest(Uri baseUrl, string format)
            : this(baseUrl, format, new Dictionary<string, string>())
        {
        }

        public ArcGisTileRequest(Uri baseUrl, string format, Dictionary<string, string> customParameters)
        {
            _baseUrl = baseUrl;
            _format = format;
            _customParameters = customParameters;
        }

        public Uri GetUri(TileInfo info)
        {
            var url = new StringBuilder();

            url.AppendFormat(CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}.{4}",
                _baseUrl, LevelToHex(info.Index.Level), RowToHex(info.Index.Row), ColumnToHex(info.Index.Col), _format);
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

        private static string LevelToHex(string zoomLevelId)
        {
            var zoom = int.Parse(zoomLevelId);

            string zoomUrl;

            if (zoom < 10)
                zoomUrl = "L0" + zoom;
            else
                zoomUrl = "L" + zoom;

            return zoomUrl;
        }

        static string ColumnToHex(int x)
        {
            return "C" + String.Format("{0:x8}", x); //Column (xTile naar hex met min/max 8 getallen aanvullen met 0)
        }

        static string RowToHex(int y)
        {
            return "R" + String.Format("{0:x8}", y); //Row (yTile naar hex met min/max 8 getallen aanvullen met 0)
        }

    }
}
