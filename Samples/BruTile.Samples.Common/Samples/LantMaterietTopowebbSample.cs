// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BruTile.Wmts;

namespace BruTile.Samples.Common.Samples
{
    public static class LantMaterietTopowebbSample
    {
        public static async Task<ITileSource> CreateAsync()
        {
            var url = "https://api.lantmateriet.se/open/topowebb-ccby/v1/wmts/token/7cdfdab81eba86d0bc4fbb328165e9a/?request=GetCapabilities&version=1.1.1&service=wmts";
            var httpClienthandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClienthandler);
            await using var stream = await httpClient.GetStreamAsync(new Uri(url)); 
            var tileSources = WmtsParser.Parse(stream);
            return tileSources.First(s => s.Name.ToLower() == "topowebb" && s.Schema.Srs.Contains("3857"));
        }
    }
}
