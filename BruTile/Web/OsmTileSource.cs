// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using BruTile.Cache;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class OsmTileSource : TileSource
    {
        public OsmTileSource(OsmRequest osmRequest = null, 
            ITileCache<byte[]> persistentCache = null,
            Func<Uri, HttpWebRequest> webRequestFactory = null)
            : base(new WebTileProvider(osmRequest ?? new OsmRequest(KnownOsmTileServers.Mapnik), persistentCache,
                webRequestFactory), 
                new SphericalMercatorInvertedWorldSchema())
        {
            var resolutionsToDelete = new List<int>();
            var resolutions = Schema.Resolutions;
            for (var i = 0; i < resolutions.Count; i++)
            {
                var id = int.Parse(resolutions[i].Id);
                if (id < osmRequest.OsmConfig.MinResolution || id > osmRequest.OsmConfig.MaxResolution)
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("must remove resolution at index {0}", i));
                    resolutionsToDelete.Add(i);
                }
            }

            int numDeleted = 0;
            foreach (var i in resolutionsToDelete)
            {
                resolutions.RemoveAt(i - numDeleted++);
            }
        }

        public Extent Extent { get { return Schema.Extent; } }
    }
}
