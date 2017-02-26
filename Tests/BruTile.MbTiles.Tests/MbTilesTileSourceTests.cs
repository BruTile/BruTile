using System.Linq;
using NUnit.Framework;
using SQLite;

namespace BruTile.MbTiles.Tests
{
    [TestFixture]
    public class MbTilesTileSourceTests
    {
        [Test]
        public void FetchTiles()
        {
            // arrange
            SQLitePCL.Batteries.Init();
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
