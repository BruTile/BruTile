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
            // Arrange
            var schema = new GlobalSphericalMercator();

            // Act
            var level = BruTile.Utilities.GetNearestLevel(schema.Resolutions, 300.0);
            // Assert
            Assert.True(level == 9);
        }
    }
}
