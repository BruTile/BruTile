// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BruTile.Predefined;
using NUnit.Framework;

namespace BruTile.Tests
{
    [TestFixture]
    public class TileTransformTests
    {
        private const double Epsilon = 0.000001;

        [Test]
        public void TileToWorldShouldReturnCorrectExtent()
        {
            // Arrange
            var range = new TileRange(1, 2);
            var schema = new GlobalSphericalMercator(YAxis.TMS);
            var expectedExtent = new Extent(-15028131.257989, -10018754.173189, -10018754.173189, -5009377.088389);
            const double toleratedDelta = 0.01;

            // Act
            var extent = TileTransform.TileToWorld(range, 3, schema);


            // Assert
            Assert.AreEqual(extent.MinX, expectedExtent.MinX, toleratedDelta);
            Assert.AreEqual(extent.MinY, expectedExtent.MinY, toleratedDelta);
            Assert.AreEqual(extent.MaxX, expectedExtent.MaxX, toleratedDelta);
            Assert.AreEqual(extent.MaxY, expectedExtent.MaxY, toleratedDelta);
        }

        [Test]
        public void WorldToTileShouldReturnCorrectTileRange()
        {
            // Arrange
            var expectedRange = new TileRange(1, 2);
            var schema = new GlobalSphericalMercator(YAxis.TMS);
            var extent = new Extent(-15028130, -10018753, -10018755, -5009378);

            // Act
            var range = TileTransform.WorldToTile(extent, 3, schema);

            // Assert
            Assert.AreEqual(range, expectedRange);
        }

        [Test]
        public void GetTilesInViewWithBiggerExtentThanTileSchemaExtentReturnsCorrectNumberOfTiles()
        {
            // Arrange
            var schema = new GlobalSphericalMercator();
            var requestExtent = GrowExtent(schema.Extent, schema.Extent.Width);

            var counter = 0;
            foreach (var resolution in schema.Resolutions.OrderByDescending(r => r.Value.UnitsPerPixel))
            {
                // Act
                var tileInfos = schema.GetTileInfos(requestExtent, resolution.Value.Level).ToList();

                // Assert
                Assert.True(tileInfos.Count == (int)Math.Round(Math.Pow(4, counter++)));
                if (counter >= 6) break;
            }
        }

        private static Extent GrowExtent(Extent extent, double amount)
        {
            return new Extent(
                extent.MinX - amount,
                extent.MinY - amount,
                extent.MaxX + amount,
                extent.MaxY + amount);
        }

        [Test]
        public void TileSchemaWithExtentThatDoesOriginateInOriginShouldReturnCorrectNumberOfTiles()
        {
            // Arrange
            var schemaExtent = new Extent(187009, 331184, 187189, 331290);
            var schema = new WkstNederlandSchema { Extent = schemaExtent, OriginY = -100000 };
            var requestExtent = GrowExtent(schemaExtent, schemaExtent.Width);

            // Act
            var tileInfos = schema.GetTileInfos(requestExtent, 14).ToList();

            // Assert
            Assert.True(TilesWithinEnvelope(tileInfos, schemaExtent));
            Assert.True(Math.Abs(TileAreaWithinEnvelope(tileInfos, schemaExtent) - schemaExtent.Area) < Epsilon);
        }

        [Test]
        public void TileSchemaWithExtentThatDoesOriginateInOriginAndWithInverteYShouldReturnCorrectNumberOfTiles()
        {
            // Arrange
            var schemaExtent = new Extent(187009, 331184, 187189, 331290);
            var schema = new WkstNederlandSchema { Extent = schemaExtent, OriginY = -22598.080, YAxis = YAxis.OSM };
            var requestExtent = GrowExtent(schemaExtent, schemaExtent.Width);

            // Act
            var tileInfos = schema.GetTileInfos(requestExtent, 14);

            // Assert
            Assert.True(TilesWithinEnvelope(tileInfos, schemaExtent));
            Assert.True(Math.Abs(TileAreaWithinEnvelope(tileInfos, schemaExtent) - schemaExtent.Area) < Epsilon);
        }

        private static bool TilesWithinEnvelope(IEnumerable<TileInfo> tileInfos, Extent evenlope)
        {
            return tileInfos.All(tileInfo => evenlope.Intersects(tileInfo.Extent));
        }

        private static double TileAreaWithinEnvelope(IEnumerable<TileInfo> tileInfos, Extent envelope)
        {
            return tileInfos.Sum(tileInfo => envelope.Intersect(tileInfo.Extent).Area);
        }

        [Test]
        public void TileSchemaWithExtentThatDoesNotStartInOriginShouldReturnNoTiles()
        {
            // Arrange
            var schema = new WkstNederlandSchema { Extent = new Extent(187036, 331205, 187202, 331291) };
            var mapExtent = new Extent(187256.999043765, 331197.712996388, 187437.576002535, 331303.350517269);

            // Act
            var tileInfos = schema.GetTileInfos(mapExtent, 14);

            // Assert
            Assert.AreEqual(tileInfos.Count(), 0);
        }

        [Test]
        public void TileTransformWithDifferentTileWidthAndHeight()
        {
            // Arrange
            var tileWidth = 10;
            var tileHeight = 5; // Note, tile tileHeight is half the tileWidth

            var expectedColCount = 10;
            var expectedRowCount = 20; // Because tileHeight is half the tileHeight there is a double number of rows

            var schema = new TileSchema { Extent = new Extent(0, 0, 100, 100), OriginX = 0, OriginY = 0 };
            schema.Resolutions.Add(0, new Resolution(0, 1, tileWidth, tileHeight, 0, 0, 10, 10, 1));
            var requestedExtent = new Extent(0, 0, 100, 100);

            // Act
            var range = TileTransform.WorldToTile(requestedExtent, 0, schema);

            // Assert
            Assert.AreEqual(expectedColCount, range.ColCount, "ColCount");
            Assert.AreEqual(expectedRowCount, range.RowCount, "RowCount");
        }
    }
}
