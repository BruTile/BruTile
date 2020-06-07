using BruTile.MbTiles;
using SQLite;

namespace BruTile.Samples.Common.Samples
{
    public class MbTilesSample
    {
        public static ITileSource Create()
        {
            return new MbTilesTileSource(new SQLiteConnectionString("Resources/world.mbtiles", false));
        }
    }
}
