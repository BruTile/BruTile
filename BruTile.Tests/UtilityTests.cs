using System;
using System.Linq;
using NUnit.Framework;

namespace BruTile.Tests
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void TestVersion()
        {
            // act
            var version = Utilities.Version;
            // assert
            Assert.True(version == "0.8");
        }
    }
}
