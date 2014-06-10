using System;
using BruTile.Wmts;
using BruTile.Wmts.Generated;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BruTile.Tests.Wmts
{
    [TestFixture]
    public class WmtsTests
    {
        [Test]
        public void TestParsingWmtsCapabilitiesResourceUrls()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wmts", "wmts-capabilties-restful-wien-resourceUrls.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                Assert.NotNull(tileSources);
            }
        }

        [Test]
        public void TestParsingWmtsCapabilities()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wmts", "wmts-capabilties-restful-wien.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                Assert.NotNull(tileSources);
            }
        }

        [Test]
        public void TestParsingWmtsCapabilitiesKvp()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wmts", "wmts-capabilities-pdok.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                Assert.NotNull(tileSources);
            }
        }

        [Test]
        public void TestParsingWmtsGlobalCRS84Scale()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wmts", "wmts-capabilities-pdok.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                var tileSource = tileSources.First(s => s.Title == "non-existing-GlobalCRS84Scale-layer");
                Assert.True(Math.Abs(tileSource.Schema.Extent.Area - new Extent(-180, -90, 180, 90).Area) < 1.0);
            }
        }

        [Test]
        public void TestParsingWmtsCapabilitiesKvpAndRestful()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wmts", "wmts-capabilities-arcgis-server-doggersbank.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                var tileSource = tileSources.First(s => s.Title.ToLower() == "public_doggersbank");
                Assert.NotNull(tileSource.Provider);
            }
        }

        [Test]
        public void TestWmtsRequest()
        {
            // arrange
            var resourceUrls = new List<ResourceUrl>
            {
                new ResourceUrl { Format = "image/jpeg", Template="http://maps1.wien.gv.at/wmts/lb/farbe/google3857/{TileMatrix}/{TileRow}/{TileCol}.jpeg", 
                    ResourceType = URLTemplateTypeResourceType.tile },
                new ResourceUrl { Format = "image/jpeg", Template="http://maps2.wien.gv.at/wmts/lb/farbe/google3857/{TileMatrix}/{TileRow}/{TileCol}.jpeg", 
                    ResourceType = URLTemplateTypeResourceType.tile },
			    new ResourceUrl { Format = "image/jpeg", Template="http://maps3.wien.gv.at/wmts/lb/farbe/google3857/{TileMatrix}/{TileRow}/{TileCol}.jpeg", 
                    ResourceType = URLTemplateTypeResourceType.tile }
            };

            var wmtsRequest = new WmtsRequest(resourceUrls);

            // act
            var url1 = wmtsRequest.GetUri(new TileInfo { Index = new TileIndex(8938, 5680, "14") });
            var url2 = wmtsRequest.GetUri(new TileInfo { Index = new TileIndex(8938, 5680, "14") });

            // assert
            Assert.True(url1.ToString().Equals("http://maps1.wien.gv.at/wmts/lb/farbe/google3857/14/5680/8938.jpeg"));
            Assert.True(url2.ToString().Contains("maps2"));
        }
    }
}
