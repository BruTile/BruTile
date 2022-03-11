using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // Arrange
            var request = new BasicRequest("http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", new[] {"a", "b", "c"});
            var tileInfo = new TileInfo {Index = new TileIndex(3, 4, 5)};
        
            // Act
            var url = request.GetUri(tileInfo);

            // Assert
            Assert.True(url.ToString() == "http://a.tile.openstreetmap.org/5/3/4.png");
        }

        [Test]
        public void GetUriInParallelTest()
        {
            // Arrange
            var request = new BasicRequest("http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", new[] {"a", "b", "c"});
            var tileInfo = new TileInfo {Index = new TileIndex(3, 4, 5)};
            var urls = new ConcurrentBag<Uri>(); // List is not thread save

            // Act
            var requests = new List<Func<Uri>>();
            for (var i = 0; i < 100; i++) requests.Add(() => request.GetUri(tileInfo));
            Parallel.ForEach(requests, r => urls.Add(r()));

            // Assert
            Assert.True(urls.FirstOrDefault(u => u.ToString() == "http://b.tile.openstreetmap.org/5/3/4.png") != null);
        }
    }
}



