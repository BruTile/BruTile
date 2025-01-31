// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Predefined;
using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace BruTile.Tests;

[TestFixture]
public class UtilityTests
{
    [Test]
    public void TestGetNearestLevel()
    {
        // Arrange
        var schema = new GlobalSphericalMercator();

        // Act
        var level = BruTile.Utilities.GetNearestLevel(schema.Resolutions, 300.0);
        // Assert
        Assert.True(level == 9);
    }
}
