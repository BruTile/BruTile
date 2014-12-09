using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BruTile.Web;
using BruTile.Wmsc;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    internal class TileSourceWmsCTests
    {
        [Test]
        public void ParseCapabilitiesWmsC()
        {
            // arrange
            const int expectedNumberOfTileSources = 54;
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wmsc", "WmsCCapabilities_1_1_1.xml")))
            {
                // act
                var tileSources = WmscTileSource.CreateFromWmscCapabilties(XDocument.Load(stream));

                // assert
                Assert.AreEqual(tileSources.Count(), expectedNumberOfTileSources);
                foreach (var tileSource in tileSources)
                {
                    Assert.NotNull(tileSource.Schema);
                    Assert.NotNull(tileSource.Schema.Resolutions);
                    Assert.NotNull(tileSource.Schema.YAxis);
                    Assert.NotNull(tileSource.Schema.Extent);
                    Assert.NotNull(tileSource.Schema.Srs);
                }
            }
        }
    }
}