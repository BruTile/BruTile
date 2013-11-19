using BruTile.Web.Wmts;
using BruTile.Web.Wmts.Generated;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace BruTile.Tests.Web.Wmts
{
    [TestFixture]
    public class WmtsTests
    {
        [Test]
        public void TestParsingWmtsCapabilities()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wmts", "wmts-capabilties-copied-from-openlayers-sample.xml")))
            {
                // act
                var tileSource = WmtsParser.Parse(stream);

                // assert
                Assert.NotNull(tileSource);
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
