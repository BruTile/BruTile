// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Predefined;
using BruTile.Web;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace BruTile.Extensions.Tests;

[TestFixture]
public class HttpClientTests
{
    [Test]
    public async Task TestGetTileAsync()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect("http://localhost/7/3/5.png")
            .Respond("image/png", new MemoryStream([0x01, 0x02, 0x03, 0x04]));
        var httpClient = new HttpClient(mockHttp);
        var tileInfo = new TileInfo { Index = new TileIndex(3, 5, 7) };
        var definition = CreateHttpTileSourceDefinition();

        // Act
        var response = await httpClient.GetTileAsync(tileInfo, definition, CancellationToken.None).ConfigureAwait(false);

        // Assert
        Assert.NotNull(response);
        Assert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04 }, response);
    }

    private static HttpTileSourceDefinition CreateHttpTileSourceDefinition()
    {
        var tileSchema = new GlobalSphericalMercator();
        var urlBuilder = new BasicRequest("http://localhost/{z}/{x}/{y}.png");
        var name = "name";
        var attribution = new Attribution("attribution");
        var userAgentOverride = "userAgentOverride";
        return new HttpTileSourceDefinition(tileSchema, urlBuilder, name, attribution, userAgentOverride);
    }
}
