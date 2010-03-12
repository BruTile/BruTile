using System;
using System.IO;
using System.Reflection;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    class TileSourceWmsCTest
    {
        [Test]
        public void ParseCapabiltiesWmsC()
        {
            //todo: configure the test data in the proper way.
            string url = @"\Resources\CapabiltiesWmsC.xml";
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tileSources = TileSourceWmsC.TileSourceBuilder(new Uri("file://" + directory + "\\" + url), null);
            int count = 54;
            Assert.AreEqual(tileSources.Count, count);
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
