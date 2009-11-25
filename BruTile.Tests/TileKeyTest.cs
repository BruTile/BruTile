using BruTile;
using NUnit.Framework;

namespace BruTileTests
{
    /// <summary>
    ///This is a test class for TileKeyTest and is intended
    ///to contain all TileKeyTest Unit Tests
    ///</summary>
    [TestFixture]
    public class TileKeyTest
    {
        /// <summary>
        ///A test for CompareTo
        ///</summary>
        [Test]
        public void CompareToTest()
        {
            TileKey target = new TileKey(2, 4, 2);
            TileKey key = new TileKey(2, 5, 2);
            int expected = -1;
            int actual;
            actual = target.CompareTo(key);
            Assert.AreEqual(expected, actual);
        }
    }
}
