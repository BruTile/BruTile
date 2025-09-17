using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BruTile.MbTiles.Tests.Utilities;
using BruTile.Predefined;
using NUnit.Framework;
using SQLite;

namespace BruTile.Osmdroid.Test;

[TestFixture]
public class OsmdroidTilesTileSourceTests
{
    private readonly string? _encryptionKey = null;

    [Test]
    public async Task FetchTiles()
    {
        // Arrange
        var path = Path.Combine(Paths.AssemblyDirectory, "Resources", "test.sqlite");
        var tileSource = new OsmdroidTilesTileSource(new SQLiteConnectionString(path, false, _encryptionKey));
        var extent = tileSource.Schema.Extent;
        var tileInfos = tileSource.Schema.GetTileInfos(extent, 1).ToList();
        tileSource.Attribution = new Attribution("attribution", "url");

        // Act
        var data = await tileSource.GetTileAsync(tileInfos.First()).ConfigureAwait(false);

        // Assert
        Assert.That(data != null, "Should not be null");
        Assert.That(data?.Length > 0, "Length should be greater than 0");
        Assert.That(string.Equals("attribution", tileSource.Attribution.Text), "Should be equal");
    }

    // These are just random indexes from a map database,
    // they are not meant to be the same point in any way.
    [TestCase(0, 0)]
    [TestCase(5, 1)]
    [TestCase(36, 2)]
    [TestCase(203, 3)]
    [TestCase(1033, 4)]
    [TestCase(5134, 5)]
    [TestCase(24586, 6)]
    [TestCase(115346, 7)]
    [TestCase(541820, 8)]
    [TestCase(2621280, 9)]
    public void TestDecodeIndex(long index, int zoom)
    {
        var level = OsmdroidTilesTileSource.GetZoomLevel(index);

        Assert.That(level, Is.Not.Negative);
        Assert.That(level, Is.EqualTo(zoom));
    }
}
