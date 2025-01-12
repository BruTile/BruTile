// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BruTile.MbTiles.Tests.Utilities;
using BruTile.Predefined;
using NUnit.Framework;
using SQLite;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace BruTile.MbTiles.Tests;

[TestFixture]
public class MbTilesTileSourceTests
{
    private readonly string? _encryptionKey = null;

    [SetUp]
    public void TestSetUp()
    {
        SQLitePCL.Batteries.Init();
    }

    [Test]
    public async Task FetchTiles()
    {
        // Arrange
        var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "test.mbtiles");
        var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey));
        var extent = tileSource.Schema.Extent;
        var tileInfos = tileSource.Schema.GetTileInfos(extent, 1).ToList();
        tileSource.Attribution = new Attribution("attribution", "url");

        // Act
        var data = await tileSource.GetTileAsync(tileInfos.First()).ConfigureAwait(false);

        // Assert
        Assert.IsNotNull(data);
        Assert.True(data?.Length > 0);
        Assert.AreEqual(MbTilesType.BaseLayer, tileSource.Type);
        Assert.AreEqual("attribution", tileSource.Attribution.Text);
    }

    [Test]
    public void SchemaGeneratedFromMbTiles()
    {
        // Arrange
        var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "test.mbtiles");

        // Act
        var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey));

        // Assert
        var extent = new Extent(-20037508.3427892, -20037471.205137, 20037508.3427892, 20037471.205137);
        Assert.IsTrue(extent.Area / tileSource.Schema.Extent.Area > 0.0000001);
        Assert.AreEqual(3, tileSource.Schema.Resolutions.Count);
    }

    [Test]
    public void SchemaGeneratedFromMbTilesContainingSmallArea()
    {
        // Arrange
        var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "el-molar.mbtiles");

        // Act
        var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey), determineZoomLevelsFromTilesTable: true);

        // Assert
        Assert.AreEqual(95490133.792558521d, tileSource.Schema.Extent.Area, 0.0001d);
        Assert.AreEqual(17, tileSource.Schema.Resolutions.Count);
    }

    [Test]
    public void SchemaGeneratedFromMbTilesContainingSmallAreaWithFewLevels()
    {
        // Arrange
        var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "torrejon-de-ardoz.mbtiles");

        // Act
        var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey), determineZoomLevelsFromTilesTable: true);

        // Assert
        Assert.AreEqual(692609746.90386355, tileSource.Schema.Extent.Area);
        Assert.AreEqual(5, tileSource.Schema.Resolutions.Count);
    }

    [Test]
    public async Task SchemaGeneratedFromMbTilesWithSchemaInConstructor()
    {
        // Arrange
        SQLitePCL.Batteries.Init();
        var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "torrejon-de-ardoz.mbtiles");

        // Act
        var tileSource = new MbTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey), new GlobalSphericalMercator("png", YAxis.TMS, null));

        // Assert
        var tile = await tileSource.GetTileAsync(new TileInfo { Index = new TileIndex(2006, 2552, 12) }).ConfigureAwait(false);
        Assert.NotNull(tile);
    }
}
