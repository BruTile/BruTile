// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Web
{
    [Obsolete("Replaced with HttpTileSource", true)]
    public class OsmTileSource : TileSource
    {
        public OsmTileSource(ITileProvider tileProvider, ITileSchema tileSchema) : base(tileProvider, tileSchema)
        {
        }
    }
}