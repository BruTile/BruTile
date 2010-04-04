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
            TileIndex target = new TileIndex(2, 4, 2);
            TileIndex key = new TileIndex(2, 5, 2);
            int expected = -1;
            int actual;
            actual = target.CompareTo(key);
            Assert.AreEqual(expected, actual);
        }
    }
}
