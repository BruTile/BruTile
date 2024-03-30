﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BruTile.Predefined;
using BruTile.Web;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace BruTile.Tests.Web;

[TestFixture]
public class HttpTileSourceTests
{
    [Test]
    public static async Task TestAsyncTileFetcher()
    {
        // Arrange
        var tileSource = KnownTileSources.Create();
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://*").Respond("image/png", new MemoryStream());
        var httpClient = new HttpClient(mockHttp);
        var range = tileSource.Schema.GetTileInfos(tileSource.Schema.Extent, 3);
        var timeStart = DateTime.Now;

        // Act
        var tiles = await tileSource.GetTilesAsync(httpClient, range).ConfigureAwait(false);

        // Assert
        Console.WriteLine("Durations: {0:0} milliseconds", DateTime.Now.Subtract(timeStart).TotalMilliseconds);
        Assert.AreEqual(64, tiles.Length);
    }

    [Test]
    public void TestAddHeader()
    {
        // Arrange
        var tileSource = new HttpTileSource(new GlobalSphericalMercator(), "https://tile.openstreetmap.org/{z}/{x}/{y}.png");

        // Act
        tileSource.AddHeader("Referer", "test-referer");

        // Assert
        Assert.AreEqual("test-referer", tileSource.GetHeaders("Referer").FirstOrDefault());
    }
}
