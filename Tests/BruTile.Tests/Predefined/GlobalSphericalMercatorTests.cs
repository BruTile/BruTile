using System.Linq;
using BruTile.Predefined;
using NUnit.Framework;

namespace BruTile.Tests.Predefined
{
    [TestFixture]
    public class GlobalSphericalMercatorTests
    {
        [Test]
        public void InitializeAsBingSchema()
        {
            // Arrange
            const string name = "BingMaps";
            const string format = "jpg";

            // Act
            var schema = new GlobalSphericalMercator(format, YAxis.OSM, 1, 19, name);
            
            // Assert
            Assert.True(schema.Resolutions.Count == 19);
            Assert.True(schema.Resolutions.All(r => r.Value.Level != 0));
            Assert.True(schema.Resolutions.Any(r => r.Value.Level == 1));
            Assert.True(schema.Resolutions[3].Level == 3);
            Assert.True(schema.Name == name);
            Assert.True(schema.Format == format);
        }
    }
}
