using System;
using BruTile.Cache;
using NUnit.Framework;

namespace BruTile.Tests.Cache
{
    [TestFixture]
    public class MemoryCacheTest
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

        [Test]
        public void WhenKeepInMemoryIsUsedItShouldPreserveTilesThatMeetTheCondition()
        {
            // arrange
            Func<TileIndex, bool> keepTileInMemory = index => index.Row == 2; // keep all where Row = 2
            var memoryCache = new MemoryCache<byte[]>(1, 2, keepTileInMemory);
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
        }
    }
}
