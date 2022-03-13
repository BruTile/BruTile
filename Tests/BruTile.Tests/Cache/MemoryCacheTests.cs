using System;
using BruTile.Cache;
using NUnit.Framework;

namespace BruTile.Tests.Cache
{
    [TestFixture]
    public class MemoryCacheTests
    {
        [Test]
        public void WhenContentIsAddedItShouldBeRetrieved()
        {
            // Arrange
            var memoryCache = new MemoryCache<byte[]>();
            var tileIndex = new TileIndex(1, 2, 3);
            var tileBytes = new byte[] { 7, 7, 7 };

            // Act
            memoryCache.Add(tileIndex, tileBytes);

            // Assert
            Assert.AreEqual(tileBytes, memoryCache.Find(tileIndex));
        }

        private class DisposableTile : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        [Test]
        public void WhenContentIsDisposableItShouldBeDisposed()
        {
            // Arrange
            var memoryCache = new MemoryCache<DisposableTile>();
            var tileIndex = new TileIndex(1, 2, 3);
            var disposableTile = new DisposableTile();
            memoryCache.Add(tileIndex, disposableTile);

            // Act
            memoryCache.Remove(tileIndex);

            // Assert
            Assert.True(disposableTile.Disposed);
        }

        [Test]
        public void WhenMaxTilesIsExceededTileCountShouldGoToMinTiles()
        {
            // Arrange
            var memoryCache = new MemoryCache<DisposableTile>(2, 3);
            memoryCache.Add(new TileIndex(1, 0, 0), new DisposableTile());
            memoryCache.Add(new TileIndex(2, 0, 0), new DisposableTile());
            memoryCache.Add(new TileIndex(3, 0, 0), new DisposableTile());
            var tileCountBeforeExceedingMax = memoryCache.TileCount;
            
            // Act
            memoryCache.Add(new TileIndex(4, 0, 0), new DisposableTile());
            var tileCountAfterExceedingMax = memoryCache.TileCount;
            
            // Assert
            Assert.True(tileCountBeforeExceedingMax == 3);
            Assert.True(tileCountAfterExceedingMax == 2);
        }

        [Test]
        public void WhenKeepInMemoryIsUsedItShouldPreserveTilesThatMeetTheCondition()
        {
            // Arrange
            const int maxTiles = 2;
            const int minTiles = 1;
            var memoryCache = new MemoryCache<byte[]>(minTiles, maxTiles, index => index.Row == 2);
            var tileBytes = new byte[] { 0, 0, 0, 0 };
            var tileOne = new TileIndex(0, 2, 0);
            var tileTwo = new TileIndex(2, 2, 2);
            var tileThree = new TileIndex(0, 8, 0);
            
            // Act
            memoryCache.Add(tileOne, tileBytes);
            memoryCache.Add(tileTwo, tileBytes);
            memoryCache.Add(tileThree, tileBytes); 
            // Third tile causes CleanUp inside Add because max is 2
            // normally only the last one would remain.
            // With this keepTileMemory method the first two remain.

            // Assert
            Assert.True(memoryCache.Find(tileOne) != null);
            Assert.True(memoryCache.Find(tileTwo) != null);
            Assert.True(memoryCache.Find(tileThree) == null);
        }
    }
}
