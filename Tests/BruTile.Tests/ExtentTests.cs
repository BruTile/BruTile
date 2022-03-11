using System;
using NUnit.Framework;

namespace BruTile.Tests
{
    [TestFixture]
    public class ExtentTests
    {
        [Test]
        public void TestEquals()
        {
            // Arrange
            var extent = new Extent(-180, -90, 180, 90);
            var extentSame = new Extent(-180, -90, 180, 90);
            var extentDiffMinX = new Extent(-181, -90, 180, 90);
            var extentDiffMinY = new Extent(-180, -91, 180, 90);
            var extentDiffMaxX = new Extent(-180, -90, 181, 90);
            var extentDiffMaxy = new Extent(-180, -90, 180, 91);
            
            // Act + Assert
            Assert.True(extent == extentSame);
            Assert.False(extent != extentSame);
            Assert.False(extent == extentDiffMinX);
            Assert.False(extent == extentDiffMinY);
            Assert.False(extent == extentDiffMaxX);
            Assert.False(extent == extentDiffMaxy);
        }

        [Test]
        public void TestMinSmallerThanMax()
        {
            // Arrange + act + assert
            Assert.Throws<ArgumentException> (() =>
            {
                var extent = new Extent(-180, 90, 180, -90);
                Assert.IsNotNull(extent);
            });
        }
    }
}
