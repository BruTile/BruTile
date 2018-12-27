﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BruTile.Tests.Utilities;
using BruTile.Wmts;
using BruTile.Wmts.Generated;
using NUnit.Framework;

namespace BruTile.Tests.Wmts
{
    [TestFixture]
    public class WmtsTests
    {
        [TestCase("wmts-capabilities-dlr.xml")]
        [TestCase("wmts_capabilities_where_upperbound_and_lowerbound_lack_ows_prefix.xml")]
        public void TestParsingWmtsCapabilities(string xml)
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", xml)))
            {
                // act
                IEnumerable<ITileSource> tileSources = null;
                Assert.DoesNotThrow(() => tileSources = WmtsParser.Parse(stream));

                // assert
                Assert.NotNull(tileSources);
                Assert.Greater(tileSources.Count(), 0);
            }
        }
        
        [Test]
        public void TestParsingWmtsCapabilitiesResourceUrls()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilties-restful-wien-resourceUrls.xml")))
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
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilties-restful-wien.xml")))
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
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-pdok.xml")))
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
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-pdok.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                var tileSource = tileSources.First(s => s.Name == "non-existing-GlobalCRS84Scale-layer");
                Assert.True(Math.Abs(tileSource.Schema.Extent.Area - new Extent(-180, -90, 180, 90).Area) < 1.0);
            }
        }

        [Test]
        public void TestParsingWmtsCapabilitiesKvpAndRestful()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-arcgis-server-doggersbank.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                var tileSource = tileSources.First(s => s.Name.ToLower() == "public_doggersbank");
                var tileSchema = tileSource.Schema as WmtsTileSchema;
                Assert.AreEqual(15, tileSource.Schema.Resolutions.Count);
                Assert.NotNull(tileSchema);
                Assert.AreEqual("public_doggersbank", tileSchema.Title);
                Assert.AreEqual("public_doggersbank", tileSchema.Layer);
                Assert.AreEqual("default028mm", tileSchema.TileMatrixSet);
            }
        }

        private static List<ResourceUrl> CreateResourceUrls()
        {
            var resourceUrls = new List<ResourceUrl>
            {
                new ResourceUrl
                {
                    Format = "image/jpeg",
                    Template = "http://maps1.wien.gv.at/wmts/lb/farbe/google3857/{TileMatrix}/{TileRow}/{TileCol}.jpeg",
                    ResourceType = URLTemplateTypeResourceType.tile
                },
                new ResourceUrl
                {
                    Format = "image/jpeg",
                    Template = "http://maps2.wien.gv.at/wmts/lb/farbe/google3857/{TileMatrix}/{TileRow}/{TileCol}.jpeg",
                    ResourceType = URLTemplateTypeResourceType.tile
                },
                new ResourceUrl
                {
                    Format = "image/jpeg",
                    Template = "http://maps3.wien.gv.at/wmts/lb/farbe/google3857/{TileMatrix}/{TileRow}/{TileCol}.jpeg",
                    ResourceType = URLTemplateTypeResourceType.tile
                }
            };
            return resourceUrls;
        }

        [Test]
        public void TestWmtsRequest()
        {
            // arrange
            var resourceUrls = CreateResourceUrls();
            var wmtsRequest = new WmtsRequest(resourceUrls);

            // act
            var url1 = wmtsRequest.GetUri(new TileInfo { Index = new TileIndex(8938, 5680, "14") });
            var url2 = wmtsRequest.GetUri(new TileInfo { Index = new TileIndex(8938, 5680, "14") });

            // assert
            Assert.True(url1.ToString().Equals("http://maps1.wien.gv.at/wmts/lb/farbe/google3857/14/5680/8938.jpeg"));
            Assert.True(url2.ToString().Contains("maps2"));
        }

        [Test]
        public void TestWmtsRequestInParallel()
        {
            // arrange
            var resourceUrls = CreateResourceUrls();
            var request = new WmtsRequest(resourceUrls);
            var urls = new ConcurrentBag<Uri>(); // List is not thread save
            var tileInfo = new TileInfo {Index = new TileIndex(8938, 5680, "14")};

            // act
            var requests = new List<Func<Uri>>();
            for (var i = 0; i < 150; i++) requests.Add(() => request.GetUri(tileInfo));
            Parallel.ForEach(requests, r => urls.Add(r()));

            // assert
            var count = urls.Count(u => u.ToString() == "http://maps1.wien.gv.at/wmts/lb/farbe/google3857/14/5680/8938.jpeg");
            Assert.True(count == 50);
        }

        [Test]
        public void TestParsingWmtsWhereUpperBoundAndLowerBoundLackOwsPrefix()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts_capabilities_where_upperbound_and_lowerbound_lack_ows_prefix.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                var tileSource = tileSources.First(s => s.Name.ToLower() == "topowebb");
                var tileSchema = (WmtsTileSchema)tileSource.Schema;
                Assert.NotNull(tileSchema.Extent);
            }
        }

        [Test]
        public void TestParsingWmtsCapabilitiesWithDeviatingEpsgCodes()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-cuzk-cz.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                Assert.NotNull(tileSources);
            }
        }


        [Test]
        public void TestParsingWmtsCapabilitiesRayaBasemapServer()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-raya-basemap-server.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                Assert.AreEqual(3, tileSources.Count());
            }
        }

        [Test]
        public void TestParsingWmtsCapabilitiesMarsWithDoubleValues()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-mars.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                Assert.AreEqual(1, tileSources.Count());
            }
        }

        [Test]
        public void TestNoTitlePresentInWmtsCapabilitiesLayer()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-opencache-statkart-no.xml")))
            {
                // act
                var tileSources = WmtsParser.Parse(stream);

                // assert
                Assert.AreEqual(319, tileSources.Count());
            }
        }
    }
}
