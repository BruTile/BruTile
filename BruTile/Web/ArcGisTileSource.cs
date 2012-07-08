// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Linq;

namespace BruTile.Web
{
    public class ArcGisTileSource : ITileSource
    {
        public ITileProvider Provider { get; private set; }
        public ITileSchema Schema { get; private set; }
        public static string BaseUrl { get; private set; }

        public ArcGisTileSource(string baseUrl, ITileSchema schema)
        {
            BaseUrl = baseUrl;
            Provider = CreateProvider();
            Schema = schema;
        }

        private static ITileProvider CreateProvider()
        {
            return new WebTileProvider(RequestBuilder);
        }

        private static IRequest RequestBuilder
        {
            get
            {
                var requestUrl = string.Format("{0}/tile/{1}", BaseUrl, "{0}/{2}/{1}");                
                return new BasicRequest(requestUrl);
            }
        }
    }
}
