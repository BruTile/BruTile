using System;
using System.Linq;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    public class BasisRequestTests
    {
        [Test]
        public void GetUriTest()
        {
            // arrange
            var request = new BasicRequest("http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", new[] {"a", "b", "c"});
            var tileInfo = new TileInfo {Index = new TileIndex(3, 4, "5")};
        
            // act
            var url = request.GetUri(tileInfo);

            // assert
            Assert.True(url.ToString() == "http://a.tile.openstreetmap.org/5/3/4.png");
        }
    }
}
