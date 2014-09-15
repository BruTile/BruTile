using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BruTile.Web.Wms;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    internal class WmsCapabilitiesTests
    {
        [Test]
        public void WmsCapabilitiesWhenParsedShouldSetCorrectGetMapUrl()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wms", "BgrGroundwaterWhyMapCapabilities_1_1_1.xml")))
            {
                const string expectedUrl = "http://www.bgr.de/Service/groundwater/whymap/?";

                // act
                var capabilities = new WmsCapabilities(stream);

                // assert
                Assert.True(expectedUrl == capabilities.Capability.Request.GetMap.DCPType[0].Http.Get.OnlineResource.Href);
            }
        }

        [Test]
        public void WmsCapabilitiesWhenInitializedWithWmsCShouldInitializeAllTileLayers()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wmsc", "WmsCCapabilities_1_1_1.xml")))
            {
                // act
                var capabilities = new WmsCapabilities(stream);

                // assert
                Assert.NotNull(capabilities.Version);
                Assert.AreEqual(54, capabilities.Capability.Layer.ChildLayers.Count);
            }
        }
        
        [Test]
        public void WmsCapabilitiesWhenCreatedWithCapabilitiesWithMultipleRootLayersShouldInitializeCorrectly()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wms", "MultiTopLayersCapabilities_1_3_0.xml")))
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
        public void WmsCapabilitiesWhenCreatedWithValidCapabilitiesV111DocumentShouldInitializeCorrectly()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wms", "FrioCountyTXMapsWmsCapabilities_1_1_1.xml")))
            {
                // act
                var capabilities = new WmsCapabilities(stream); 

                // assert
                Assert.NotNull(capabilities.Version);
                Assert.AreEqual("Frio County TX Maps", capabilities.Capability.Layer.Title);
                Assert.AreEqual(13, capabilities.Capability.Layer.ChildLayers.Count);
            }
        }

        [Test]
        public void WmsCapabilitiesForLizardTech()
        {
            // arrange
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wms", "wms-lizardtech.xml")))
            {
                // act
                var wmsCapabilities = new WmsCapabilities(XDocument.Load(stream));
                
                // assert
                Assert.AreEqual(12, wmsCapabilities.Capability.Layer.ChildLayers.Count);
            }
        }

        [Test]
        public void WmsCapabilitiesWithXmlnsAttribute()
        {
            using (var stream = File.OpenRead(Path.Combine("Resources", "Wms", "WmsCapabilities_1_3_0_withXmlns.xml")))
            {
                // act
                var capabilities = new WmsCapabilities(stream);

                // assert
                Assert.NotNull(capabilities);
                Assert.NotNull(capabilities.Version);
                Assert.AreEqual("1 Million Scale WMS Layers from the National Atlas of the United States", capabilities.Capability.Layer.Title);
                Assert.AreEqual(19, capabilities.Capability.Layer.ChildLayers.Count);
            }   
        }
    }
}