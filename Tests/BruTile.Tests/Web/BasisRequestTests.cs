// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BruTile.Web;
using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace BruTile.Tests.Web;

[TestFixture]
public class BasisRequestTests
{
    [Test]
    public void GetUriTest()
    {
        // Arrange
        var basicUrlBuilder = new BasicUrlBuilder("http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", ["a", "b", "c"]);
        var tileInfo = new TileInfo { Index = new TileIndex(3, 4, 5) };

        // Act
        var url = basicUrlBuilder.GetUrl(tileInfo);

        // Assert
        Assert.True(url.ToString() == "http://a.tile.openstreetmap.org/5/3/4.png");
    }

    [Test]
    public void GetUriInParallelTest()
    {
        // Arrange
        var basicUrlBuilder = new BasicUrlBuilder("http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", ["a", "b", "c"]);
        var tileInfo = new TileInfo { Index = new TileIndex(3, 4, 5) };
        var urls = new ConcurrentBag<Uri>(); // List is not thread save

        // Act
        var requests = new List<Func<Uri>>();
        for (var i = 0; i < 100; i++) requests.Add(() => basicUrlBuilder.GetUrl(tileInfo));
        Parallel.ForEach(requests, r => urls.Add(r()));

        // Assert
        Assert.True(urls.FirstOrDefault(u => u.ToString() == "http://b.tile.openstreetmap.org/5/3/4.png") != null);
    }
}
