using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using BruTile.Tests.Utilities;
using BruTile.Wms;
using NUnit.Framework;

namespace BruTile.Tests.Wms
{
    [TestFixture]
    internal class WmsCapabilitiesTests
    {
        [Test]
        public void WmsCapabilitiesWhenParsedShouldSetCorrectGetMapUrl()
        {
            // arrange
            var fileName = "BgrGroundwaterWhyMapCapabilities_1_1_1.xml";
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName)))
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
            var fileName = "WmsCCapabilities_1_1_1.xml";
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmsc", fileName)))
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
            var fileName = "MultiTopLayersCapabilities_1_3_0.xml";
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName)))
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
            var fileName = "FrioCountyTXMapsWmsCapabilities_1_1_1.xml";
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName)))
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
            var fileName = "LizardtechWmsCapabilities_1_1_1.xml";
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName)))
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
            // arrange
            var fileName = "WmsCapabilities_1_3_0_withXmlns.xml";
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName)))
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

        [Test]
        public void WmsCapabilitiesForNrcsSoilWms()
        {
            // arrange
            var fileName = "NrcsSoilWmsCapabilities_1_1_1.xml";
            using (var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName)))
            {
                // act
                var wmsCapabilities = new WmsCapabilities(XDocument.Load(stream));

                // assert
                Assert.True(wmsCapabilities.Service.Name == ServiceName.WMS);
            }
        }

        [TestCase("http://abc.de?", null, false)]
        [TestCase("http://abc.de?Service=WMS&", null, false)]
        [TestCase("http://abc.de?Service=WMS&Version=1.0.7", null, false)]
        [TestCase("http://abc.de?Service=WMS&Version=1.0.0&REQUEST=GetCapabilities&", null, true)]
        [TestCase("http://abc.de?Service=WMS&Version=1.0.7&REQUEST=GetCapabilities&", null, true)]
        [TestCase("http://abc.de?Service=WMS&Version=1.1.0&REQUEST=GetCapabilities&", null, true)]
        [TestCase("http://abc.de?Service=WMS&Version=1.1.1&REQUEST=GetCapabilities&", null, true)]
        [TestCase("http://abc.de?Service=WMS&Version=1.3.0&REQUEST=GetCapabilities&", null, true)]
        [TestCase("http://abc.de?Service=NG&Version=1.0.7&REQUEST=GetCapabilities&", typeof(ArgumentException), false)]
        [TestCase("http://abc.de?Service=WMS&Version=1.0.6&REQUEST=GetCapabilities&", typeof(ArgumentException), true)]
        [TestCase("http://abc.de?Service=WMS&Version=1.0.7&REQUEST=GetBlaBla&", typeof(ArgumentException), false)]
        public void WmsValidateGetCapabilitiesRequest(string url, Type exception, bool expected)
        {
            // arrange
            var valid = !expected;
            var uri = new Uri(url);

            // act
            if (exception != null)
            {
                Assert.Throws<ArgumentException>(() => valid = WmsCapabilities.ValidateGetCapabilitiesRequest(uri.Query));
                return;
            }
            Assert.DoesNotThrow(() => valid = WmsCapabilities.ValidateGetCapabilitiesRequest(uri.Query));

            //assert
            Assert.That(valid, Is.EqualTo(expected));
        }

        [TestCase("http://www.geodaten-mv.de/dienste/gdimv_dtk", "WMS MV DTK", "1.3.0")]
        [TestCase("http://www.geodaten-mv.de/dienste/gdimv_dtk?VERSION=1.1.1", "WMS MV DTK", "1.1.1")]
        [TestCase("http://www.geodaten-mv.de/dienste/gdimv_dtk?VERSION=1.1.0", "WMS MV DTK", "1.1.0")]
        public void WmsGetCapabilitiesUrl(string url, string serviceTitle = null, string version = null)
        {
            // arrange, act
            WmsCapabilities cap = null;
            Assert.DoesNotThrow(() => cap = new WmsCapabilities(url, null));

            // assert
            Assert.That(cap, Is.Not.Null);

            Console.WriteLine($"{cap.Service.Title} (WMS {cap.Version.VersionString})");
            if (!string.IsNullOrWhiteSpace(serviceTitle))
                Assert.That(cap.Service.Title, Is.EqualTo(serviceTitle));

            if (!string.IsNullOrWhiteSpace(version))
                Assert.That(cap.Version.VersionString, Is.EqualTo(version));
        }
    }
}