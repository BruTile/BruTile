// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class BingTileSource : TileSource
    {
        public BingTileSource(String url, string token, BingMapType mapType)
            :this(new BingRequest(url, token, mapType))
        {
        }

        public BingTileSource(IRequest bingRequest)
            : base(new WebTileProvider(bingRequest), new BingSchema())
        {
        }
    }
}
