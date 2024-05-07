// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Samples.Common;

public class Tile<T>(byte[] Data, TileInfo Info)
{
    public byte[] Data { get; } = Data;
    public TileInfo Info { get; } = Info;
    public T? Image { get; set; }
    public long StartAnimation { get; set; }
}
