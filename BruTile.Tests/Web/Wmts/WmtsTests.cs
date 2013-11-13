using System.IO;
using System.Linq;
using BruTile.Web.Wmts;
using NUnit.Framework;

namespace BruTile.Tests.Web.Wmts
{
    [TestFixture]
    public class WmtsTests
    {
        [Test]
        public void TestParsingWmtsCapabilities()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wmts", "wmts-capabilties-copied-from-openlayers-sample.xml")))
            {
                // act
                var tileSource = WmtsParser.Parse(stream);

                // assert
                Assert.NotNull(tileSource);
            }
        }
    }
}
