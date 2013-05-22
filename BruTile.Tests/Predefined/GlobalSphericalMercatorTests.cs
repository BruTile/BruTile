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
            // arrange
            const string name = "BingMaps";
            const string format = "jpg";

            // act
            var schema = new GlobalSphericalMercator(format, true, 1, 19, name);
            
            // assert
            Assert.True(schema.Resolutions.Count == 19);
            Assert.True(schema.Resolutions.All(r => r.Value.Id != "0"));
            Assert.True(schema.Resolutions.Any(r => r.Value.Id == "1"));
            Assert.True(schema.Resolutions[3].Id == "3");
            Assert.True(schema.Name == name);
            Assert.True(schema.Format == format);
        }
    }
}
