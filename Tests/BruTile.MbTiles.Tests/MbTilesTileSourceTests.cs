using System.IO;
using System.Linq;
using BruTile.MbTiles.Tests.Utilities;
using BruTile.Predefined;
using NUnit.Framework;
using SQLite;

namespace BruTile.MbTiles.Tests
{
    [TestFixture]
    public class MbTilesTileSourceTests
    {
        private readonly string _encryptionKey = null;

        [SetUp]
        public void TestSetUp()
        {
            SQLitePCL.Batteries.Init();
        }

        [Test]
        public void FetchTiles()
        {
            // arrange
            var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "test.mbtiles");
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey));
            var extent = tileSource.Schema.Extent;
            var tileInfos = tileSource.Schema.GetTileInfos(extent, "1").ToList();
            tileSource.Attribution = new Attribution("attribution", "url");
            
            // act
            var data = tileSource.GetTile(tileInfos.First());

            // assert
            Assert.True(data.Length > 0);
            Assert.AreEqual(MbTilesType.BaseLayer, tileSource.Type);
            Assert.AreEqual("attribution", tileSource.Attribution.Text);
        }

        [Test]
        public void SchemaGeneratedFromMbTiles()
        {
            // arrange
            var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "test.mbtiles");

            // act
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey));

            // assert
            var extent = new Extent(-20037508.3427892, -20037471.205137, 20037508.3427892, 20037471.205137);
            Assert.IsTrue(extent.Area / tileSource.Schema.Extent.Area > 0.0000001);
            Assert.AreEqual(3, tileSource.Schema.Resolutions.Count);
        }


        [Test]
        public void SchemaGeneratedFromMbTilesContainingSmallArea()
        {
            // arrange
            var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "el-molar.mbtiles");

            // act
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey));

            // assert
            Assert.AreEqual(95490133.792558521d, tileSource.Schema.Extent.Area, 0.0001d);
            Assert.AreEqual(17, tileSource.Schema.Resolutions.Count);
        }

        [Test]
        public void SchemaGeneratedFromMbTilesContainingSmallAreaWithFewLevels()
        {
            // arrange
            var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "torrejon-de-ardoz.mbtiles");

            // act
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey));

            // assert
            Assert.AreEqual(692609746.90386355, tileSource.Schema.Extent.Area);
            Assert.AreEqual(5, tileSource.Schema.Resolutions.Count);
        }


        [Test]
        public void SchemaGeneratedFromMbTilesWithSchemaInConstructor()
        {
            // arrange
            SQLitePCL.Batteries.Init();
            var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "torrejon-de-ardoz.mbtiles");

            // act
            var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey), new GlobalSphericalMercator("png", YAxis.TMS, null));

            // assert
            var tile = tileSource.GetTile(new TileInfo { Index = new TileIndex(2006, 2552, "12")});
            Assert.NotNull(tile);
        }
    }
}
