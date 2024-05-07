// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using BruTile.Tms;
using NUnit.Framework;

namespace BruTile.Tests.Tms;

[TestFixture]
public class TmsRequestTests
{
    [Test]
    public void WhenInitializedShouldReturnCorrectUri()
    {
        // Arrange
        var tmsUrlBuilder = new TmsUrlBuilder("http://tileserver.com", "png");
        var tileInfo = new TileInfo { Index = new TileIndex(1, 2, 3) };

        // Act
        var uri = tmsUrlBuilder.GetUrl(tileInfo);

        // Assert
        Assert.AreEqual(uri.ToString(), "http://tileserver.com/3/1/2.png");
    }

    [Test]
    public void WhenInitializedWithServerNodesShouldReturnCorrectUrl()
    {
        // Arrange
        var tmsUrlBuilder = new TmsUrlBuilder("http://{S}.tileserver.com", "png", ["a", "b"]);
        var tileInfo = new TileInfo { Index = new TileIndex(1, 2, 3) };
        string[] possibleExpectedUrls = ["http://a.tileserver.com/3/1/2.png", "http://b.tileserver.com/3/1/2.png"]; // Node a or b

        // Act
        var url = tmsUrlBuilder.GetUrl(tileInfo);

        // Assert
        Assert.True(possibleExpectedUrls.Contains(url.ToString()));
    }
}
