// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using BruTile.Cache;
using BruTile.Predefined;

namespace BruTile.Web
{
    [Obsolete("Replaced with HttpTileSource", true)]
    public class OsmTileSource : TileSource
    {
        public OsmTileSource(OsmRequest osmRequest = null,
            IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, byte[]> fetchTile = null)
            : base(new HttpTileProvider(
                        osmRequest ?? new OsmRequest(KnownTileSource.OpenStreetMap), 
                        persistentCache,
                        fetchTile), 
                new SphericalMercatorInvertedWorldSchema())
        {
            if (osmRequest == null) osmRequest = new OsmRequest(KnownTileSource.OpenStreetMap);
            var resolutionsToDelete = new List<int>();
            var resolutions = Schema.Resolutions;
            for (var i = 0; i < resolutions.Count; i++)
            {
                var id = int.Parse(resolutions[i.ToString(CultureInfo.InvariantCulture)].Id);
                if (id < osmRequest.OsmConfig.MinResolution || id > osmRequest.OsmConfig.MaxResolution)
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("must remove resolution at index {0}", i));
                    resolutionsToDelete.Add(i);
                }
            }

            int numDeleted = 0;
            foreach (var i in resolutionsToDelete)
            {
                resolutions.Remove((i - numDeleted++).ToString(CultureInfo.InvariantCulture));
            }
        }

        public Extent Extent { get { return Schema.Extent; } }
    }
}
