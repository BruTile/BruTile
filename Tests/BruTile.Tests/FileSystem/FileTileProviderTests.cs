using System;
using System.Threading.Tasks;
using BruTile.Cache;
using NUnit.Framework;
using BruTile.FileSystem;

namespace BruTile.Tests.FileSystem
{
    [TestFixture]
    public class FileTileProviderTests
    {
        [Test]
        public async Task GetTile_WhenTilePresent_ShouldReturnTile()
        {
            // arrange
            var tileCache = new FileCache(".\\FileCacheTest", "png", new TimeSpan(long.MaxValue));
            tileCache.Add(new TileIndex(4, 5, 8),new byte[243]);
            var fileTileProvider = new FileTileProvider(".\\FileCacheTest", "png", new TimeSpan(long.MaxValue));

            // Act
            var tile = await fileTileProvider.GetTileAsync(new TileInfo { Index = new TileIndex(4, 5, 8) }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tile.Length, 243);
        }
    }
}
