using System;
using System.Linq;
using BruTile.Predefined;
using NUnit.Framework;

namespace BruTile.Tests
{
    [TestFixture]
    public class TileTransformTests
    {
        [Test]
        public void TileToWorldShouldReturnCorrectExtent()
        {
            // arrange
            var range = new TileRange(1, 2);
            var schema = new SphericalMercatorWorldSchema();
            var expectedExtent = new Extent(-15028131.257989,-10018754.173189,-10018754.173189,-5009377.088389);
              
            // act
            var extent = TileTransform.TileToWorld(range, "3", schema);

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
            var range = TileTransform.WorldToTile(extent, "3", schema);

            // assert
            Assert.AreEqual(range, expectedRange);
        }

        [Test]
        public void TileSchemaWithExtentThatDoesOriginateInOriginShouldReturnCorrectNumberOfTiles()
        {
            // arrange
            var schema = new WkstNederlandSchema {Extent = new Extent(187036, 331205, 187202, 331291)};
            var mapExtent = new Extent(187009,331184,187189,331290);

            // act
            var tileInfos = schema.GetTilesInView(mapExtent, "14");

            // assert
            Assert.AreEqual(tileInfos.Count(), 12);
        }

        [Test]
        public void TileSchemaWithExtentThatDoesOriginateInOriginAndWithInverteYShouldReturnCorrectNumberOfTiles()
        {
            // arrange
            var schema = new WkstNederlandSchema { Extent = new Extent(187036, 331205, 187202, 331291), OriginY = -22598.080, Axis = AxisDirection.InvertedY };
            var mapExtent = new Extent(187009, 331184, 187189, 331290);

            // act
            var tileInfos = schema.GetTilesInView(mapExtent, "14");

            // assert
            Assert.AreEqual(tileInfos.Count(), 12);
        }

        [Test]
        public void TileSchemaWithExtentThatDoesNotStartInOriginShouldReturnNoTiles()
        {
            // arrange
            var schema = new WkstNederlandSchema {Extent = new Extent(187036, 331205, 187202, 331291)};
            var mapExtent = new Extent(187256.999043765,331197.712996388,187437.576002535,331303.350517269);

            // act
            var tileInfos = schema.GetTilesInView(mapExtent, "14");

            // assert
            Assert.AreEqual(tileInfos.Count(), 0);
        }
        
    }
}
