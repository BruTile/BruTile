#region License

// Copyright 2010 - Paul den Dulk (Geodan)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

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
