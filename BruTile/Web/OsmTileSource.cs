// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using BruTile.Cache;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class OsmTileSource : ITileSource
    {
        public ITileSchema Schema { get; private set; }
        public ITileProvider Provider { get; private set; }

        public OsmTileSource()
            :this(new OsmRequest(KnownOsmTileServers.Mapnic))
        {}

        public OsmTileSource(OsmRequest osmRequest)
            :this(osmRequest, new MemoryCache<byte[]>(50, 100))
        {
        }

        public OsmTileSource(OsmRequest osmRequest, ITileCache<byte[]> cache)
        {
            Schema = new SphericalMercatorInvertedWorldSchema();
            
            var resolutionsToDelete = new List<int>();
            var resolutions = Schema.Resolutions;
            for(var i = 0; i < resolutions.Count; i++)
            {
                var id = int.Parse(resolutions[i].Id);
                if (id < osmRequest.OsmConfig.MinResolution || id > osmRequest.OsmConfig.MaxResolution)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("must remove resolution at index {0}", i));
                    resolutionsToDelete.Add(i);
                }
            }

            int numDeleted = 0;
            foreach (var i in resolutionsToDelete)
            {
                resolutions.RemoveAt(i - numDeleted++);
            }

            Provider = new WebTileProvider(osmRequest, cache);
        }

        public Extent Extent { get { return Schema.Extent; } }
    }
}
