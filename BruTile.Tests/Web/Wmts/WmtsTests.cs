using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BruTile.Web;
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
                var ser = new XmlSerializer(typeof(Capabilities));

                Capabilities capabilties;

                using (var reader = new StreamReader(stream))
                {
                     capabilties = (Capabilities)ser.Deserialize(reader);
                }
                // assert
                Assert.NotNull(capabilties);
            }
        }
    }
}
