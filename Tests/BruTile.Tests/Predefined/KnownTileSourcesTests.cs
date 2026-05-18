// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Cache;
using BruTile.Predefined;
using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace BruTile.Tests.Predefined;

[TestFixture]
public class KnownTileSourcesTests
{
    [TestCase(KnownTileSource.BingAerial)]
    [TestCase(KnownTileSource.BingHybrid)]
    [TestCase(KnownTileSource.BingRoads)]
    [TestCase(KnownTileSource.BingAerialStaging)]
    [TestCase(KnownTileSource.BingHybridStaging)]
    [TestCase(KnownTileSource.BingRoadsStaging)]
    public void CreateShouldPreservePersistentCacheForBingSources(KnownTileSource source)
    {
        // Arrange
        var persistentCache = new TestPersistentCache();

        // Act
        var tileSource = KnownTileSources.Create(source, persistentCache: persistentCache);

        // Assert
        Assert.AreSame(persistentCache, tileSource.PersistentCache);
    }

    private class TestPersistentCache : IPersistentCache<byte[]>
    {
        public void Add(TileIndex index, byte[] tile)
        {
        }

        public void Remove(TileIndex index)
        {
        }

        public byte[]? Find(TileIndex index)
        {
            return null;
        }
    }
}
