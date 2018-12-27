using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BruTile.Predefined;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace BruTile.Tests.Web
{
    [TestFixture]
    public class HttpTileSourceTests
    {
        [Test]
        public static async Task TestAsyncTileFetcher()
        {
            // arrange
            var tileSource = KnownTileSources.Create();
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://*").Respond("image/png", new MemoryStream());
            var httpClient = new HttpClient(mockHttp);
            var range = tileSource.Schema.GetTileInfos(tileSource.Schema.Extent, "3");
            var timeStart = DateTime.Now;

            // act
            var tiles = await tileSource.GetTilesAsync(httpClient, range).ConfigureAwait(false);

            // assert
            Console.WriteLine("Durations: {0:0} milliseconds", DateTime.Now.Subtract(timeStart).TotalMilliseconds);
            Assert.AreEqual(64, tiles.Count());
        }
    }
}
