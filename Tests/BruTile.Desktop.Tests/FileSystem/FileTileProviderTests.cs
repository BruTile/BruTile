using System;
using BruTile.Cache;
using NUnit.Framework;
using BruTile.FileSystem;

namespace BruTile.Tests.FileSystem
{
    [TestFixture]
    public class FileTileProviderTests
    {
        [Test]
        public void GetTile_WhenTilePresent_ShouldReturnTile()
        {
            // arrange
            var tileCache = new FileCache(".\\FileCacheTest", "png", new TimeSpan(long.MaxValue));
            tileCache.Add(new TileIndex(4, 5, "8"),new byte[243]);
            var fileTileProvider = new FileTileProvider(".\\FileCacheTest", "png", new TimeSpan(long.MaxValue));

            // act
            var tile = fileTileProvider.GetTile(new TileInfo { Index = new TileIndex(4, 5, "8") });

            // assert
            Assert.AreEqual(tile.Length, 243);
        }
    }
}
