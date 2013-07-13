// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Linq;
using BruTile.Cache;

namespace BruTile.Web
{
    public class ArcGisTileSource : TileSource
    {
        public string BaseUrl { get; private set; }

        public ArcGisTileSource(
                string baseUrl, 
                ITileSchema schema, 
                IPersistentCache<byte[]> persistentCache = null,
                Func<Uri, byte[]> fetchTile = null)
            : base(
                new WebTileProvider(CreateArcGISRequest(baseUrl), persistentCache, fetchTile), 
                schema)
        {
            BaseUrl = baseUrl;
        }

        private static IRequest CreateArcGISRequest(string baseUrl)
        {
            return new BasicRequest(string.Format("{0}/tile/{1}", baseUrl, "{0}/{2}/{1}"));
        }
    }
}
