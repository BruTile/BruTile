using System;
using System.Linq;
using BruTile.Predefined;
using NUnit.Framework;

namespace BruTile.Tests
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void TestGetNearestLevel()
        {
            // arrange
            var schema = new GlobalSphericalMercator();

            // act
            var levelId = Utilities.GetNearestLevel(schema.Resolutions, 300.0);
            // assert
            Assert.True(levelId == "9");
        }
    }
}
