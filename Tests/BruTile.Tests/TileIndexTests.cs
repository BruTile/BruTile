using NUnit.Framework;

namespace BruTile.Tests
{
    [TestFixture]
    public class TileIndexTests
    {
        [Test]
        public void CompareToTest()
        {
            // arrange
            var target = new TileIndex(2, 4, "2");
            var index = new TileIndex(2, 5, "2");
            const int expected = -1;

            // act
            int actual = target.CompareTo(index);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
