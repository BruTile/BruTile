using System;
using System.Linq;
using BruTile.PreDefined;
using NUnit.Framework;

namespace BruTile.Tests
{
    [TestFixture]
    public class TileTransformTest
    {
        [Test]
        public void TileToWorldShouldReturnCorrectExtent()
        {
            // arrange
            var range = new TileRange(1, 2);
            var schema = new SphericalMercatorWorldSchema();
            var expectedExtent = new Extent(-15028131.257989,-10018754.173189,-10018754.173189,-5009377.088389);
              
            // act
            var extent = TileTransform.TileToWorld(range, 3, schema);

            // assert
            Assert.AreEqual(extent.MinX, expectedExtent.MinX, 0.0001);
            Assert.AreEqual(extent.MinY, expectedExtent.MinY, 0.0001);
            Assert.AreEqual(extent.MaxX, expectedExtent.MaxX, 0.0001);
            Assert.AreEqual(extent.MaxY, expectedExtent.MaxY, 0.0001);
        }

        [Test]
        public void WorldToTileShouldReturnCorrectTileRange()
        {
            // arrange
            var expectedRange = new TileRange(1, 2);
            var schema = new SphericalMercatorWorldSchema();
            var extent = new Extent(-15028130, -10018753, -10018755, -5009378);
            
            // act
            var range = TileTransform.WorldToTile(extent, 3, schema);

            // assert
            Assert.AreEqual(range, expectedRange);
        }
    }
}
