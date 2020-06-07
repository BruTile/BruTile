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
            // arrange
            var memoryCache = new MemoryCache<byte[]>();
            var tileIndex = new TileIndex(1, 2, 3);
            var tileBytes = new byte[] { 7, 7, 7 };

            // act
            memoryCache.Add(tileIndex, tileBytes);

            // assert
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
            // arrange
            var memoryCache = new MemoryCache<DisposableTile>();
            var tileIndex = new TileIndex(1, 2, 3);
            var disposableTile = new DisposableTile();
            memoryCache.Add(tileIndex, disposableTile);

            // act
            memoryCache.Remove(tileIndex);

            // assert
            Assert.True(disposableTile.Disposed);
        }

        [Test]
        public void WhenMaxTilesIsExceededTileCountShouldGoToMinTiles()
        {
            // arrange
            var memoryCache = new MemoryCache<DisposableTile>(2, 3);
            memoryCache.Add(new TileIndex(1, 0, 0), new DisposableTile());
            memoryCache.Add(new TileIndex(2, 0, 0), new DisposableTile());
            memoryCache.Add(new TileIndex(3, 0, 0), new DisposableTile());
            var tileCountBeforeExceedingMax = memoryCache.TileCount;
            
            // act
            memoryCache.Add(new TileIndex(4, 0, 0), new DisposableTile());
            var tileCountAfterExceedingMax = memoryCache.TileCount;
            
            // assert
            Assert.True(tileCountBeforeExceedingMax == 3);
            Assert.True(tileCountAfterExceedingMax == 2);
        }

        [Test]
        public void WhenKeepInMemoryIsUsedItShouldPreserveTilesThatMeetTheCondition()
        {
            // arrange
            Func<TileIndex, bool> keepTileInMemory = index => index.Row == 2; // keep all where Row = 2
            const int maxTiles = 2;
            const int minTiles = 1;
            var memoryCache = new MemoryCache<byte[]>(minTiles, maxTiles, keepTileInMemory);
            var tileBytes = new byte[] { 0, 0, 0, 0 };
            var tileOne = new TileIndex(0, 2, 0);
            var tileTwo = new TileIndex(2, 2, 2);
            var tileThree = new TileIndex(0, 8, 0);
            
            // act
            memoryCache.Add(tileOne, tileBytes);
            memoryCache.Add(tileTwo, tileBytes);
            memoryCache.Add(tileThree, tileBytes); 
            // 3th tile causes CleanUp inside Add because max is 2
            // normally only the last one would remain.
            // With this keepTileMemory method the first two remain.

            // assert
            Assert.True(memoryCache.Find(tileOne) != null);
            Assert.True(memoryCache.Find(tileTwo) != null);
            Assert.True(memoryCache.Find(tileThree) == null);
        }
    }
}
