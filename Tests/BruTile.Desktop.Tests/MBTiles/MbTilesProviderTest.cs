using BruTile.FileSystem;
using NUnit.Framework;

namespace BruTile.Tests.MBTiles
{
    [TestFixture]
    public class MbTilesProviderTest
    {
        private const string MbTilesFile = @"";

        public void SetUp()
        {
            
        }

        [Test]
        public void TestConstructorFilename()
        {
            MbTilesProvider provider;
            Assert.DoesNotThrow( () => provider = new MbTilesProvider(MbTilesFile));
        }
    }

    [TestFixture]
    public class MbTilesTileSourcTest
    {
        private const string MbTilesFile = @"";

        public void SetUp()
        {
            
        }
        [Test]
        public void TestConstructorFilename()
        {
            MbTilesTileSource source;
            Assert.DoesNotThrow( () => source = new MbTilesTileSource(MbTilesFile));
        }
    }

}