// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BruTile.Web;

public class ArcGisTileRequest(Uri baseUrl, string format, Dictionary<string, string> customParameters) : IUrlBuilder
{
    private readonly Uri _baseUrl = baseUrl;
    private readonly Dictionary<string, string> _customParameters = customParameters;
    private readonly string _format = format;

    public ArcGisTileRequest(Uri baseUrl, string format) : this(baseUrl, format, []) { }

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
            var first = true;
            foreach (var name in _customParameters.Keys)
            {
                var value = _customParameters[name];
                url.AppendFormat("{0}{1}={2}", first ? "?" : "&", name, value);
                first = false;
            }
        }
    }

    private static string LevelToHex(int level)
    {
        string zoomUrl;

        if (level < 10)
            zoomUrl = "L0" + level;
        else
            zoomUrl = "L" + level;

        return zoomUrl;
    }

    private static string ColumnToHex(int x)
    {
        return "C" + $"{x:x8}"; // Column (xTile to hex with min/max 8 and justify with 0)
    }

    private static string RowToHex(int y)
    {
        return "R" + $"{y:x8}"; // Row (yTile to hex with min/max 8 and justify with 0)
    }

}
