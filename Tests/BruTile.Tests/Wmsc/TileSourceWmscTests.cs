// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using System.Xml.Linq;
using BruTile.Tests.Utilities;
using BruTile.Wmsc;
using NUnit.Framework;

namespace BruTile.Tests.Wmsc;

[TestFixture]
internal class TileSourceWmscTests
{
    [Test]
    public void ParseCapabilitiesWmsc()
    {
        // Arrange
        const int expectedNumberOfTileSources = 54;
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmsc", "WmscCapabilities_1_1_1.xml"));

        // Act
        var tileSources = WmscCapabilitiesParser.Parse(XDocument.Load(stream));

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
}
