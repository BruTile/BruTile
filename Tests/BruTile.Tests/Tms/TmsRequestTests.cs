using System.Linq;
using BruTile.Tms;
using NUnit.Framework;

namespace BruTile.Tests.Tms
{
    [TestFixture]
    public class TmsRequestTests
    {
        [Test]
        public void WhenInitializedShouldReturnCorrectUri()
        {
            // Arrange
            var request = new TmsRequest("http://tileserver.com", "png");
            var tileInfo = new TileInfo { Index = new TileIndex(1, 2, 3) };

            // Act
            var uri = request.GetUri(tileInfo);

            // Assert
            Assert.AreEqual(uri.ToString(), "http://tileserver.com/3/1/2.png");
        }


        [Test]
        public void WhenInitializedWithServerNodesShouldReturnCorrectUri()
        {
            // Arrange
            var request = new TmsRequest("http://{S}.tileserver.com", "png", new[] { "a", "b" });
            var tileInfo = new TileInfo { Index = new TileIndex(1, 2, 3) };

            // Act
            var uri = request.GetUri(tileInfo);

            // Assert
            Assert.True(new[] { "http://a.tileserver.com/3/1/2.png", "http://b.tileserver.com/3/1/2.png" }.Contains(uri.ToString()));
        }
    }
}
