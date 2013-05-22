using System;
using System.Linq;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    public class BasisRequestTest
    {
        [Test]
        public void GetUriTest()
        {
            // arrange
            var request = new BasicRequest("http://{S}.tile.openstreetmap.org/{Z}/{X}/{Y}.png", new[] {"a", "b", "c"});
            var tileInfo = new TileInfo {Index = new TileIndex(3, 4, 5)};
        
            // act
            var url = request.GetUri(tileInfo);

            // assert
            Assert.True(url.ToString() == "http://a.tile.openstreetmap.org/5/3/4.png");
        }
    }
}
