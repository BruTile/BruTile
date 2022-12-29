// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net;
using BruTile.Wmts;

namespace BruTile.Samples.Common.Samples
{
    public static class LantMaterietTopowebbSample
    {
        public static ITileSource Create()
        {
            var url = "https://api.lantmateriet.se/open/topowebb-ccby/v1/wmts/token/7cdfdab81eba86d0bc4fbb328165e9a/?request=GetCapabilities&version=1.1.1&service=wmts";
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var request = WebRequest.Create(new Uri(url));
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            var response = request.GetResponseAsync();

            using var stream = response.Result.GetResponseStream();
            var tileSources = WmtsParser.Parse(stream);
            return tileSources.First(s => s.Name.ToLower() == "topowebb" && s.Schema.Srs.Contains("3857"));
        }
    }
}
