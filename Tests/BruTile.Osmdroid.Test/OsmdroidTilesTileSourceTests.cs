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

    [SetUp]
    public void TestSetUp()
    {
        SQLitePCL.Batteries.Init();
    }

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
        Assert.IsNotNull(data);
        Assert.True(data?.Length > 0);
        Assert.AreEqual("attribution", tileSource.Attribution.Text);
    }
}
