using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using BruTile.Web;
using BruTile.Web.Wms;
using NUnit.Framework;
using Exception = System.Exception;

namespace BruTile.Tests.Web
{
    [TestFixture]
    internal class WmsCapabilitiesTest
    {
        [Test]
        public void WmsCapabilities_WhenParsed_ShouldSetCorrectGetMapUrl()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", @"BgrGroundwaterWhyMapCapabilities_1_1_1.xml")))
            {
                const string expectedUrl = "http://www.bgr.de/Service/groundwater/whymap/?";

                // act
                var capabilities = new WmsCapabilities(stream);

                // assert
                Assert.True(expectedUrl == capabilities.Capability.Request.GetMap.DCPType[0].Http.Get.OnlineResource.Href);
            }
        }

        [Test]
        public void WmsCapabilities_WhenInitializedWithWmsC_ShouldInitializeAllTileLayers()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", @"WmsCCapabilities_1_1_1.xml")))
            {
                // act
                var capabilities = new WmsCapabilities(stream);

                // assert
                Assert.NotNull(capabilities.Version);
                Assert.AreEqual(54, capabilities.Capability.Layer.ChildLayers.Count);
            }
        }
        
        //[Ignore("Not working properly")]
        [Test]
        public void WmsCapabilities_WhenCreatedWithCapabilitiesWithMultipleRootLayers_ShouldInitializeCorrectly()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", @"MultiTopLayersCapabilities_1_3_0.xml")))
            {
                // act
                var capabilities = new WmsCapabilities(stream);

                // assert
                Assert.NotNull(capabilities.Version);
                Assert.AreEqual("Root Layer", capabilities.Capability.Layer.Title);
                Assert.AreEqual(4, capabilities.Capability.Layer.ChildLayers.Count);
            }
        }

        [Test]
        public void WmsCapabilities_WhenCreatedWithValidCapabilitiesV111Document_ShouldInitializeCorrectly()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", @"FrioCountyTXMapsWmsCapabilities_1_1_1.xml")))
            {
                // act
                var capabilities = new WmsCapabilities(stream);

                // assert
                Assert.NotNull(capabilities.Version);
                Assert.AreEqual("Frio County TX Maps", capabilities.Capability.Layer.Title);
                Assert.AreEqual(13, capabilities.Capability.Layer.ChildLayers.Count);
            }
        }
    }
}