// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Net;
using BruTile.Cache;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class BingTileSource : TileSource
    {
        public BingTileSource(String url, string token, BingMapType mapType)
            :this(new BingRequest(url, token, mapType))
        {
        }

        public BingTileSource(BingRequest bingRequest, ITileCache<byte[]> memoryCache = null,
            Func<Uri, HttpWebRequest> webRequestFactory = null)
            : base(new WebTileProvider(bingRequest, memoryCache, webRequestFactory), new BingSchema())
        {
        }
    }
}
