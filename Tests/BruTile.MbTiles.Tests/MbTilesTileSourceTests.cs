using System.Linq;
using NUnit.Framework;
using SQLite.Net;
using SQLite.Net.Platform.Win32;

namespace BruTile.MbTiles.Tests
{
    [TestFixture]
    public class MbTilesTileSourceTests
    {
        [Test]
        public void FetchTiles()
        {
            // arrange
            MbTilesTileSource.SetPlatform(new SQLitePlatformWin32());
            const string path = ".\\Resources\\test.mbtiles";
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false));
            var extent = tileSource.Extent;
            var tileInfos = tileSource.Schema.GetTileInfos(extent, "1").ToList();
            
            // act
            var data = tileSource.GetTile(tileInfos.First());

            // assert
            Assert.True(data.Length > 0);
        }
    }
}
