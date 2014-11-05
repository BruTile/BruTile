using System;
using NUnit.Framework;
using SQLite.Net;

namespace BruTile.MbTiles.Tests
{
    [TestFixture]
    public class MbTilesTileSourceTests
    {
        [Test]
        public void FetchTiles()
        {
            //MbTilesTileSource.SetPlatform(new SQLite.Net.Platform.Win32.SQLitePlatformWin32());
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString("Resources\test.mbtiles", false));
            var extent = tileSource.Extent;
            var scale = 1;
            var tileInfos = tileSource.Schema.GetTilesInView(extent, "1");

            foreach (var tileInfo in tileInfos)
            {
                var data = tileSource.Provider.GetTile(tileInfo);
            }

        }
    }
}
