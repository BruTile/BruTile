// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.MbTiles;
using SQLite;

namespace BruTile.Samples.Common.Samples;

public class MbTilesSample
{
    public static ITileSource Create()
    {
        return new MbTilesTileSource(new SQLiteConnectionString("Resources/world.mbtiles", false));
    }
}
