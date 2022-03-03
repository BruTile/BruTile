// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Cache;
using BruTile.Web;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BruTile.Tms
{
    public class TmsTileSource : TileSource
    {
        [Obsolete("Use HttpTileSource")]
        public TmsTileSource(string serviceUrl, ITileSchema tileSchema)
             : this (new Uri(serviceUrl), tileSchema)
        {
        }

        [Obsolete("Use HttpTileSource")]
        public TmsTileSource(Uri serviceUri, ITileSchema tileSchema, IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, Task<byte[]>> fetchTile = null) :
            base(new HttpTileProvider(new TmsRequest(serviceUri, tileSchema.Format), persistentCache,
                fetchTile), tileSchema)
        {
        }

        public static ITileSource CreateFromTileMapResource(Stream tileMapResource)
        {
            // This method should be moved somewhere else
            return TileMapParser.CreateTileSource(tileMapResource);
        }
    }
}