// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Predefined;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web;
internal class HttpTileSourceDefinitionTests
{
    [Test]
    public void ConstructorTest()
    {
        // Arrange
        var tileSchema = new GlobalSphericalMercator();
        var urlBuilder = new BasicRequest("http://localhost/{z}/{x}/{y}.png");
        var name = "name";
        var attribution = new Attribution("attribution");
        var userAgentOverride = "userAgentOverride";

        // Act
        var httpTileSourceDefinitionActual = new HttpTileSourceDefinition(tileSchema, urlBuilder, name, attribution, userAgentOverride);

        // Assert
        Assert.AreEqual(tileSchema, httpTileSourceDefinitionActual.TileSchema);
        Assert.AreEqual(urlBuilder, httpTileSourceDefinitionActual.UrlBuilder);
        Assert.AreEqual(name, httpTileSourceDefinitionActual.Name);
        Assert.AreEqual(attribution, httpTileSourceDefinitionActual.Attribution);
        Assert.AreEqual(userAgentOverride, httpTileSourceDefinitionActual.UserAgentOverride);
    }

    [Test]
    public void DeconstructTest()
    {
        // Arrange
        var tileSchema = new GlobalSphericalMercator();
        var urlBuilder = new BasicRequest("http://localhost/{z}/{x}/{y}.png");
        var name = "name";
        var attribution = new Attribution("attribution");
        var userAgentOverride = "userAgentOverride";
        var httpTileSourceDefinition = new HttpTileSourceDefinition(tileSchema, urlBuilder, name, attribution, userAgentOverride);

        // Act
        var (actualTileSchema, actualUrlBuilder, actualName, actualAttribution, actualUserAgentOverride) = httpTileSourceDefinition;

        // Assert
        Assert.AreEqual(tileSchema, actualTileSchema);
        Assert.AreEqual(urlBuilder, actualUrlBuilder);
        Assert.AreEqual(name, actualName);
        Assert.AreEqual(attribution, actualAttribution);
        Assert.AreEqual(userAgentOverride, actualUserAgentOverride);
    }
}
