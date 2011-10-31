using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    class WmsCapabilitiesTest
    {
        [Test]
        public void WmsCapabilities_WhenSet_ShouldNotBeNull()
        {
            // arrange
            const string url = @"\Resources\CapabiltiesWmsC.xml";
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // act
            var capabilities = new WmsCapabilities(new Uri("file://" + directory + "\\" + url), null);

            // assert
            Assert.NotNull(capabilities.WmsVersion);
            Assert.AreEqual(54, capabilities.Layer.ChildLayers.Length);
        }

        [Test]
        public void WmsCapabilities_SyntheticRoot()
        {
            // arrange
            const string url = @"\Resources\CapabilitiesWmsMultiTopLayers.xml";
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // act
            var capabilities = new WmsCapabilities(new Uri("file://" + directory + "\\" + url), null);

            // assert
            Assert.NotNull(capabilities.WmsVersion);
            Assert.AreEqual("Root Layer", capabilities.Layer.Title);
            Assert.AreEqual(4, capabilities.Layer.ChildLayers.Length);
        }
    }
}
