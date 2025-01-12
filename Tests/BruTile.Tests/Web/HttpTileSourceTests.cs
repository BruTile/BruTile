// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Predefined;
using BruTile.Web;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace BruTile.Tests.Web;

[TestFixture]
public class HttpTileSourceTests
{
    [Test]
    public async Task TestGetTileAsync()
    {
        // Arrange
        var tileInfo = new TileInfo { Index = new TileIndex(3, 5, 7) };
        var httpTileSource = CreateHttpTileSource();
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(httpTileSource.GetUrl(tileInfo).ToString())
            .Respond("image/png", new MemoryStream([0x01, 0x02, 0x03, 0x04]));
        var httpClient = new HttpClient(mockHttp);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BruTile.Tests");

        // Act
        var response = await httpTileSource.GetTileAsync(httpClient, tileInfo, CancellationToken.None).ConfigureAwait(false);

        // Assert
        Assert.NotNull(response);
        Assert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04 }, response);
    }

    [TestCase("UserAgentOverride", "UserAgentOverride", "The 'User-Agent' header should be overridden.")]
    [TestCase(null, "DefaultUserAgent", "The 'User-Agent' header should have the default value.")]
    public async Task TestUserAgentOverride(string userAgentOverride, string expectedUserAgent, string message)
    {
        // Arrange
        var httpTileSource = CreateHttpTileSource(userAgentOverride);
        var tileInfo = new TileInfo { Index = new TileIndex(3, 5, 7) };
        var url = httpTileSource.GetUrl(tileInfo);

        var mockHttp = new MockHttpMessageHandler();
        _ = mockHttp.When(url.ToString())
            .Respond(request =>
            {
                Assert.IsTrue(request.Headers.UserAgent.ToString().Contains(expectedUserAgent), message); // Check if UserAgent header is correctly set
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var httpClient = new HttpClient(mockHttp);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DefaultUserAgent");

        // Act
        var response = await httpTileSource.GetTileAsync(httpClient, tileInfo, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
    }

    private static HttpTileSource CreateHttpTileSource(string? userAgentOverride = null)
    {
        var tileSchema = new GlobalSphericalMercator();
        var basicUrlBuilder = new BasicUrlBuilder("http://localhost/{z}/{x}/{y}.png");
        var name = "name";
        var attribution = new Attribution("attribution");
        return new HttpTileSource(tileSchema, basicUrlBuilder, name, null, attribution,
            (userAgentOverride is null) ? null : (m) => m.Headers.UserAgent.ParseAdd(userAgentOverride));
    }
}
