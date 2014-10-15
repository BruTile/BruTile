using System.Linq;
using BruTile.Web;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var url = request.GetUri(tileInfo);

            // assert
            Assert.True(url.ToString() == "http://t111.tiles.virtualearth.net/tiles/r00211.jpeg?g=555&token=pindakaas");
        }

        [Test]
        public void GetUriMultiThreadedTest()
        {
            // arrange
            var request = new BingRequest(
                "http://t{s}.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g={apiversion}&token={userkey}",
                "pindakaas", "555", new[] { "000", "111" });
            var tileInfo = new TileInfo { Index = new TileIndex(3, 4, "5") };
            var urls = new List<Uri>();

            // act
            var requests = new List<Func<Uri>>();
            for (var i = 0 ; i < 100; i++) requests.Add(() => request.GetUri(tileInfo));
            Parallel.ForEach(requests, r => urls.Add(r()));

            // assert
            Assert.True(urls.FirstOrDefault(u => u.ToString() == "http://t111.tiles.virtualearth.net/tiles/r00211.jpeg?g=555&token=pindakaas") != null);
        }
    }
}
