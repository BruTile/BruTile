// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Samples.Common
{
    public class Tile<T>
    {
        public TileInfo Info { get; set; }
        public T Image { get; set; }
        public long StartAnimation { get; set; }
        public byte[] Data { get; set; }
    }
}
