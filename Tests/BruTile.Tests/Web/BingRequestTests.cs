using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    public class BingRequestTests
    {
        [Test]
        public void GetUriTest()
        {
            // arrange
            var request = new BingRequest(
                "http://t{s}.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g={apiversion}&token={userkey}", 
                "pindakaas", "555", new [] { "000", "111"});
            var tileInfo = new TileInfo { Index = new TileIndex(3, 4, "5") };

            // act
            request.GetUri(tileInfo); // to test the internal server node counter
            var url2 = request.GetUri(tileInfo);

            // assert
            Assert.True(url2.ToString() == "http://t111.tiles.virtualearth.net/tiles/r00211.jpeg?g=555&token=pindakaas");
        }

        [Test]
        public void GetUriMultiThreadedTest()
        {
            // arrange
            var request = new BingRequest(
                "http://t{s}.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g={apiversion}&token={userkey}",
                "pindakaas", "555", new[] { "000", "111" });
            var tileInfo = new TileInfo { Index = new TileIndex(3, 4, "5") };

            // act
            var requests = new List<Func<Uri>>();
            for (var i = 0 ; i < 100; i++) requests.Add(() => request.GetUri(tileInfo));
            Parallel.ForEach(requests, r =>  r());

            // assert
            //Assert.True(url2.ToString() == "http://t111.tiles.virtualearth.net/tiles/r00211.jpeg?g=555&token=pindakaas");
        }
    }
}
