// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using BruTile.Tests.Utilities;
using BruTile.Wmsc;
using NUnit.Framework;

namespace BruTile.Tests.Wmsc;

[TestFixture]
internal class TileSourceWmsCTests
{
    [Test]
    public void ParseCapabilitiesWmsC()
    {
        // Arrange
        const int expectedNumberOfTileSources = 54;
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmsc", "WmsCCapabilities_1_1_1.xml"));
        // Act
        var tileSources = WmscTileSource.CreateFromWmscCapabilities(XDocument.Load(stream));

        // Assert
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

    [TestCase("http://resource.sgu.se/service/wms/130/brunnar?SERVICE=WMS&VERSION=1.3&REQUEST=getcapabilities&TILED=true")]
    [Ignore("The above url does not support WMSC anymore. Perhaps we should delete all our WMSC code.")]
    public async Task TestParseUrlAsync(string url)
    {
        // Arrange
        var myWmsc = new Uri(url);

        // Act
        var httpTileSources = await WmscTileSource.CreateFromWmscCapabilitiesAsync(myWmsc);

        // Assert        
        Assert.IsNotNull(httpTileSources);
        Assert.That(httpTileSources.Count, Is.GreaterThan(0));
    }
}
