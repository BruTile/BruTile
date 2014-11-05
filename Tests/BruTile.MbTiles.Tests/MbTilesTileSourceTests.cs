using System;
using System.Linq;
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
            // arrange
            MbTilesTileSource.SetPlatform(new SQLite.Net.Platform.Win32.SQLitePlatformWin32());
            const string path = ".\\Resources\\test.mbtiles";
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false));
            var extent = tileSource.Extent;
            var tileInfos = tileSource.Schema.GetTilesInView(extent, "1").ToList();
            
            // act
            var data = tileSource.Provider.GetTile(tileInfos.First());

            //assert
            Assert.True(data.Length > 0);
        }
    }
}
