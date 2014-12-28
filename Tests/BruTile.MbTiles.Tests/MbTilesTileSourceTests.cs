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
        [Ignore("This test does not run properly from Resharper but does van NUnit Gui. This is probably related to x64 vs x86")]
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
