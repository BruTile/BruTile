// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile
{
    using System.Collections.Generic;

    using BruTile.Wmts.Generated;

    public class TileInfo
    {
        public TileIndex Index { get; set; }
        public Extent Extent { get; set; }
        public Dictionary<string, string> DimensionSettings { get; set; }
}
}
