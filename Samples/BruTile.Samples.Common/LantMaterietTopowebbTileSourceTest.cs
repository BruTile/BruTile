using System;
using System.Linq;
using System.Net;
using BruTile.Wmts;

namespace BruTile.Samples.Common
{
    public static class LantMaterietTopowebbTileSourceTest
    {
        public static ITileSource Create()
        {
            Uri uri =
                new Uri(
                    "https://api.lantmateriet.se/open/topowebb-ccby/v1/wmts/token/7cdfdab81eba86d0bc4fbb328165e9a/?request=GetCapabilities&version=1.1.1&service=wmts");
            var req = WebRequest.Create(uri);
            var resp = req.GetResponseAsync();
            ITileSource tileSource;
            using (var stream = resp.Result.GetResponseStream())
            {
                var tileSources = WmtsParser.Parse(stream);
                tileSource = tileSources.First(s => s.Name.ToLower() == "topowebb");
            }
            return tileSource;
        }
    }
}