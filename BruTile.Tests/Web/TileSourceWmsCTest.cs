using System;
using System.IO;
using System.Reflection;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    internal class TileSourceWmsCTest
    {
        [Test]
        public void ParseCapabiltiesWmsC()
        {
            // arrange
            const string url = @"\Resources\CapabiltiesWmsC.xml";
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // act
            var tileSources = WmscTileSource.TileSourceBuilder(new Uri("file://" + directory + "\\" + url), null);

            // assert
            const int numberOfTileSources = 54;
            Assert.AreEqual(tileSources.Count, numberOfTileSources);
            foreach (var tileSource in tileSources)
            {
                Assert.NotNull(tileSource.Provider);
                Assert.NotNull(tileSource.Schema);
                Assert.NotNull(tileSource.Schema.Resolutions);
                Assert.NotNull(tileSource.Schema.Axis);
                Assert.NotNull(tileSource.Schema.Extent);
                Assert.NotNull(tileSource.Schema.Srs);
            }
        }
    }
}