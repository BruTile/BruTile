// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Net;
using BruTile.Cache;
using BruTile.Predefined;

namespace BruTile.Web
{
    public class BingTileSource : TileSource
    {
        public BingTileSource(String url, string token, BingMapType mapType)
            : this(new BingRequest(url, token, mapType))
        {
        }

        public BingTileSource(
                        BingRequest bingRequest, 
                        IPersistentCache<byte[]> persistentCache = null)
            : base(new WebTileProvider(bingRequest, persistentCache), new GlobalSphericalMercator("jpg", true, 1, 19, "Bing"))
        {
        }
    }
}
