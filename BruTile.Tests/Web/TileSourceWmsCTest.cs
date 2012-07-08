using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    internal class TileSourceWmsCTest
    {
        [Ignore("There is a bug here that is fixed in the PLC version")]
        [Test]
        public void ParseCapabilitiesWmsC()
        {
            // arrange
            const int expectedNumberOfTileSources = 54;
            using (var fs = new StreamReader(File.OpenRead(Path.Combine("Resources", @"CapabilitiesWmsC.xml"))))
            {
                var document = XDocument.Load(fs);

                // act
                var tileSources = WmscTileSource.TileSourceBuilder(document);

                // assert
                Assert.AreEqual(tileSources.Count, expectedNumberOfTileSources);
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
}