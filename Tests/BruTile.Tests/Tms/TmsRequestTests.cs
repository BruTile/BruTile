using BruTile.Tms;
using NUnit.Framework;
using System.Linq;

namespace BruTile.Tests.Tms
{
    [TestFixture]
    public class TmsRequestTests
    {
        [Test]
        public void WhenInitializedShouldReturnCorrectUri()
        {
            // arrange
            var request = new TmsRequest("http://tileserver.com", "png");
            var tileInfo = new TileInfo {Index = new TileIndex(1, 2, 3)};
            
            // Act
            var uri = request.GetUri(tileInfo);

            // assert
            Assert.AreEqual(uri.ToString(), "http://tileserver.com/3/1/2.png"); 
        }


        [Test]
        public void WhenInitializedWithServerNodesShouldReturnCorrectUri()
        {
            // arrange
            var request = new TmsRequest("http://{S}.tileserver.com", "png", new[] { "a", "b"});
            var tileInfo = new TileInfo { Index = new TileIndex(1, 2, 3) };
            
            // Act
            var uri = request.GetUri(tileInfo);

            // assert
            Assert.True(new [] { "http://a.tileserver.com/3/1/2.png", "http://b.tileserver.com/3/1/2.png" }.Contains(uri.ToString()));
        }
    }
}
