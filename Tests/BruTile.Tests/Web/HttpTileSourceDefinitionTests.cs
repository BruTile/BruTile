// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Net.Http;
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
        var basicUrlBuilder = new BasicUrlBuilder("http://localhost/{z}/{x}/{y}.png");
        var name = "name";
        var attribution = new Attribution("attribution");
        var configureHttpRequestMessage = new Action<HttpRequestMessage>(m => m.Headers.UserAgent.ParseAdd("userAgentOverride"));

        // Act
        var httpTileSourceDefinitionActual = new HttpTileSourceDefinition(tileSchema, basicUrlBuilder, name, attribution, configureHttpRequestMessage);

        // Assert
        Assert.AreEqual(tileSchema, httpTileSourceDefinitionActual.TileSchema);
        Assert.AreEqual(basicUrlBuilder, httpTileSourceDefinitionActual.UrlBuilder);
        Assert.AreEqual(name, httpTileSourceDefinitionActual.Name);
        Assert.AreEqual(attribution, httpTileSourceDefinitionActual.Attribution);
        Assert.AreEqual(configureHttpRequestMessage, httpTileSourceDefinitionActual.ConfigureHttpRequestMessage);
    }

    [Test]
    public void DeconstructTest()
    {
        // Arrange
        var tileSchema = new GlobalSphericalMercator();
        var basicUrlBuilder = new BasicUrlBuilder("http://localhost/{z}/{x}/{y}.png");
        var name = "name";
        var attribution = new Attribution("attribution");
        var userAgentOverride = new Action<HttpRequestMessage>(m => m.Headers.UserAgent.ParseAdd("userAgentOverride"));
        var httpTileSourceDefinition = new HttpTileSourceDefinition(tileSchema, basicUrlBuilder, name, attribution, userAgentOverride);

        // Act
        var (actualTileSchema, actualUrlBuilder, actualName, actualAttribution, configureHttpRequestMessage) = httpTileSourceDefinition;

        // Assert
        Assert.AreEqual(tileSchema, actualTileSchema);
        Assert.AreEqual(basicUrlBuilder, actualUrlBuilder);
        Assert.AreEqual(name, actualName);
        Assert.AreEqual(attribution, actualAttribution);
        Assert.AreEqual(userAgentOverride, configureHttpRequestMessage);
    }
}
