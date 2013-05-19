using System;
using System.Linq;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    public class BingRequestTest
    {
        [Test]
        public void GetUriTest()
        {
            // arrange
            var request = new BingRequest(
                "http://t{S}.tiles.virtualearth.net/tiles/r{QuadKey}.jpeg?g={ApiVersion}&token={UserKey}", 
                "pindakaas", "555", new [] { "000", "111"});
            var tileInfo = new TileInfo { Index = new TileIndex(3, 4, 5) };

            // act
            request.GetUri(tileInfo); // to test the internal server node counter
            var url2 = request.GetUri(tileInfo);

            // assert
            Assert.True(url2.ToString() == "http://t111.tiles.virtualearth.net/tiles/r00211.jpeg?g=555&token=pindakaas");
        }
    }
}
