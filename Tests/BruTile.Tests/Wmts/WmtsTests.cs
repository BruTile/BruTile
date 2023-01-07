// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BruTile.Cache;
using BruTile.Tests.Utilities;
using BruTile.Web;
using BruTile.Wmts;
using BruTile.Wmts.Generated;
using NUnit.Framework;

namespace BruTile.Tests.Wmts
{
    [TestFixture]
    public class WmtsTests
    {
        [TestCase("wmts_capabilities_missing_crs.xml")]
        [TestCase("wmts-capabilities-dlr.xml")]
        [TestCase("wmts_capabilities_where_upperbound_and_lowerbound_lack_ows_prefix.xml")]
        public void TestParsingWmtsCapabilities(string xml)
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", xml));

            // Act
            IEnumerable<ITileSource> tileSources = null;
            Assert.DoesNotThrow(() => tileSources = WmtsParser.Parse(stream));

            // Assert
            Assert.NotNull(tileSources);
            Assert.Greater(tileSources.Count(), 0);
        }

        [Test]
        public void TestParsingWmtsCapabilitiesResourceUrls()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilties-restful-wien-resourceUrls.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            Assert.NotNull(tileSources);
        }

        [Test]
        public void TestParsingWmtsCapabilities()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilties-restful-wien.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            Assert.NotNull(tileSources);
        }

        [Test]
        public void TestParsingWmtsCapabilitiesKvp()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-pdok.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            Assert.NotNull(tileSources);
        }

        [Test]
        public void TestParsingWmtsGlobalCRS84Scale()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-pdok.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            var tileSource = tileSources.First(s => s.Name == "non-existing-GlobalCRS84Scale-layer");
            Assert.True(Math.Abs(tileSource.Schema.Extent.Area - new Extent(-180, -90, 180, 90).Area) < 1.0);
        }

        [Test]
        public void TestParsingWmtsCapabilitiesKvpAndRestful()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-arcgis-server-doggersbank.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            var tileSource = tileSources.First(s => s.Name.ToLower() == "public_doggersbank");
            var tileSchema = tileSource.Schema as WmtsTileSchema;
            Assert.AreEqual(15, tileSource.Schema.Resolutions.Count);
            Assert.NotNull(tileSchema);
            Assert.AreEqual("public_doggersbank", tileSchema.Title);
            Assert.AreEqual("public_doggersbank", tileSchema.Layer);
            Assert.AreEqual("default028mm", tileSchema.TileMatrixSet);
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
            // Arrange
            var resourceUrls = CreateResourceUrls();
            var levelToIdentifier = new Dictionary<int, string> { [14] = "level-14" };
            var wmtsRequest = new WmtsRequest(resourceUrls, levelToIdentifier);

            // Act
            var url1 = wmtsRequest.GetUri(new TileInfo { Index = new TileIndex(8938, 5680, 14) });
            var url2 = wmtsRequest.GetUri(new TileInfo { Index = new TileIndex(8938, 5680, 14) });

            // Assert
            Assert.True(url1.ToString().Equals("http://maps1.wien.gv.at/wmts/lb/farbe/google3857/level-14/5680/8938.jpeg"));
            Assert.True(url2.ToString().Contains("maps2"));
        }

        [Test]
        public void TestWmtsRequestInParallel()
        {
            // Arrange
            var resourceUrls = CreateResourceUrls();
            var levelToIdentifier = new Dictionary<int, string> { [14] = "level-14" };
            var request = new WmtsRequest(resourceUrls, levelToIdentifier);
            var urls = new ConcurrentBag<Uri>(); // List is not thread save
            var tileInfo = new TileInfo { Index = new TileIndex(8938, 5680, 14) };

            // Act
            var requests = new List<Func<Uri>>();
            for (var i = 0; i < 150; i++) requests.Add(() => request.GetUri(tileInfo));
            Parallel.ForEach(requests, r => urls.Add(r()));

            // Assert
            var count = urls.Count(u => u.ToString() == "http://maps1.wien.gv.at/wmts/lb/farbe/google3857/level-14/5680/8938.jpeg");
            Assert.True(count == 50);
        }

        [Test]
        public void TestParsingWmtsWhereUpperBoundAndLowerBoundLackOwsPrefix()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts_capabilities_where_upperbound_and_lowerbound_lack_ows_prefix.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            var tileSource = tileSources.First(s => s.Name.ToLower() == "topowebb");
            var tileSchema = (WmtsTileSchema)tileSource.Schema;
            Assert.NotNull(tileSchema.Extent);
        }

        [Test]
        public void TestParsingWmtsCapabilitiesWithDeviatingEpsgCodes()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-cuzk-cz.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            Assert.NotNull(tileSources);
        }

        [Test]
        public void TestParsingWmtsCapabilitiesRayaBasemapServer()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-raya-basemap-server.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            Assert.AreEqual(3, tileSources.Count());
        }

        [Test]
        public void TestParsingWmtsCapabilitiesNoConstraint()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-noconstraint.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream).ToList();

            // Assert
            Assert.AreEqual(1, tileSources.Count);
            var s = tileSources[0].GetUri(new TileInfo()).ToString();
            Assert.IsTrue(s.Contains("&FORMAT=image/png", StringComparison.OrdinalIgnoreCase), "Assures is kvp mapping");
        }

        [Test]
        public void TestParsingWmtsCapabilitiesMarsWithDoubleValues()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-mars.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            Assert.AreEqual(1, tileSources.Count());
        }

        [Test]
        public void TestNoTitlePresentInWmtsCapabilitiesLayer()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-opencache-statkart-no.xml"));

            // Act
            var tileSources = WmtsParser.Parse(stream);

            // Assert
            Assert.AreEqual(319, tileSources.Count());
        }

        [Test]
        public void TestPersistentCacheCanBeSet()
        {
            // Arrange
            using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmts", "wmts-capabilities-dlr.xml"));
            IEnumerable<HttpTileSource> tileSources = null;
            Assert.DoesNotThrow(() => tileSources = WmtsParser.Parse(stream));
            var tileSource = tileSources.First();

            // Act
            tileSource.PersistentCache = new NullCache();

            // Assert
            Assert.NotNull(tileSource.PersistentCache);
        }
    }
}
