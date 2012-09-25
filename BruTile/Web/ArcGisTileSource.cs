// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Linq;

namespace BruTile.Web
{
    public class ArcGisTileSource : TileSource
    {
        public string BaseUrl { get; private set; }

        public ArcGisTileSource(string baseUrl, ITileSchema schema)
            :base(CreateProvider(baseUrl), schema)
        {
            BaseUrl = baseUrl;
        }

        private static ITileProvider CreateProvider(string baseUrl)
        {
            var requestBuilder = new BasicRequest(string.Format("{0}/tile/{1}", baseUrl, "{0}/{2}/{1}"));
            return new WebTileProvider(requestBuilder);
        }
    }
}
