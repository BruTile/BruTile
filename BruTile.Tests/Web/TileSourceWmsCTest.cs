using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    internal class TileSourceWmsCTest
    {
        [Test]
        public void ParseCapabilitiesWmsC()
        {
            // arrange
            const int expectedNumberOfTileSources = 54;
            using (var stream = File.OpenRead(Path.Combine("Resources", @"WmsCCapabilities_1_1_1.xml")))
            {
                // act
                var tileSources = WmscTileSource.CreateFromWmscCapabilties(XDocument.Load(stream));

                // assert
                Assert.AreEqual(tileSources.Count(), expectedNumberOfTileSources);
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

        [Test]
        public void TestUrls()
        {
            TestUrl("http://demo.opengeo.org/geoserver/gwc/service/wms?request=getcapabilities&tiled=true");
            //TestUrl("http://www.osmgb.org.uk/ogc/wmsc?Version=1.1.1&Service=WMS&Request=GetCapabilities");
        }

        private static void TestUrl(string url)
        {
            IEnumerable<ITileSource> sources = null;
            Assert.DoesNotThrow(() => sources = WmscTileSource.CreateFromWmscCapabilties(new Uri(url)), string.Format("Failed for '{0}'", url));
            foreach (var tileSource in sources)
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