// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Xml.Linq;
using BruTile.Tests.Utilities;
using BruTile.Wms;
using NUnit.Framework;

namespace BruTile.Tests.Wms;

[TestFixture]
internal class WmsCapabilitiesTests
{
    [Test]
    public void WmsCapabilitiesWhenParsedShouldSetCorrectGetMapUrl()
    {
        // Arrange
        var fileName = "BgrGroundwaterWhyMapCapabilities_1_1_1.xml";
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName));
        const string expectedUrl = "http://www.bgr.de/Service/groundwater/whymap/?";

        // Act
        var capabilities = new WmsCapabilities(stream);

        // Assert
        Assert.True(expectedUrl == capabilities.Capability.Request.GetMap.DCPType[0].Http.Get.OnlineResource.Href);
    }

    [Test]
    public void WmsCapabilitiesWhenInitializedWithWmsCShouldInitializeAllTileLayers()
    {
        // Arrange
        var fileName = "WmsCCapabilities_1_1_1.xml";
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wmsc", fileName));
        // Act
        var capabilities = new WmsCapabilities(stream);

        // Assert
        Assert.NotNull(capabilities.Version);
        Assert.AreEqual(54, capabilities.Capability.Layer.ChildLayers.Count);
    }

    [Test]
    public void WmsCapabilitiesWhenCreatedWithCapabilitiesWithMultipleRootLayersShouldInitializeCorrectly()
    {
        // Arrange
        var fileName = "MultiTopLayersCapabilities_1_3_0.xml";
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName));

        // Act
        var capabilities = new WmsCapabilities(stream);

        // Assert
        Assert.NotNull(capabilities.Version);
        Assert.AreEqual("Root Layer", capabilities.Capability.Layer.Title);
        Assert.AreEqual(4, capabilities.Capability.Layer.ChildLayers.Count);
    }

    [Test]
    public void WmsCapabilitiesWhenCreatedWithValidCapabilitiesV111DocumentShouldInitializeCorrectly()
    {
        // Arrange
        var fileName = "FrioCountyTXMapsWmsCapabilities_1_1_1.xml";
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName));

        // Act
        var capabilities = new WmsCapabilities(stream);

        // Assert
        Assert.NotNull(capabilities.Version);
        Assert.AreEqual("Frio County TX Maps", capabilities.Capability.Layer.Title);
        Assert.AreEqual(13, capabilities.Capability.Layer.ChildLayers.Count);
    }

    [Test]
    public void WmsCapabilitiesForLizardTech()
    {
        // Arrange
        var fileName = "LizardtechWmsCapabilities_1_1_1.xml";
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName));

        // Act
        var wmsCapabilities = new WmsCapabilities(XDocument.Load(stream));

        // Assert
        Assert.AreEqual(12, wmsCapabilities.Capability.Layer.ChildLayers.Count);
    }

    [Test]
    public void WmsCapabilitiesWithXmlnsAttribute()
    {
        // Arrange
        var fileName = "WmsCapabilities_1_3_0_withXmlns.xml";
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName));

        // Act
        var capabilities = new WmsCapabilities(stream);

        // Assert
        Assert.NotNull(capabilities);
        Assert.NotNull(capabilities.Version);
        Assert.AreEqual("1 Million Scale WMS Layers from the National Atlas of the United States", capabilities.Capability.Layer.Title);
        Assert.AreEqual(19, capabilities.Capability.Layer.ChildLayers.Count);
    }

    [Test]
    public void WmsCapabilitiesForNrcsSoilWms()
    {
        // Arrange
        var fileName = "NrcsSoilWmsCapabilities_1_1_1.xml";
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName));

        // Act
        var wmsCapabilities = new WmsCapabilities(XDocument.Load(stream));

        // Assert
        Assert.True(wmsCapabilities.Service.Name == ServiceName.WMS);
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
        // Arrange
        var valid = !expected;
        var uri = new Uri(url);

        // Act
        if (exception != null)
        {
            Assert.Throws<ArgumentException>(() => valid = WmsCapabilities.ValidateGetCapabilitiesRequest(uri.Query));
            return;
        }
        Assert.DoesNotThrow(() => valid = WmsCapabilities.ValidateGetCapabilitiesRequest(uri.Query));

        //assert
        Assert.That(valid, Is.EqualTo(expected));
    }

    [Test]
    public void WmsGetCapabilitiesUrl()
    {
        // Note: Not sure if this test makes much sense anymore now I 
        // changed it to use local resources. Perhaps this one should
        // just be removed since the above test covers the url 
        // generating logic.

        // Arrange
        var fileName = "wms-capabilities-gdimv_dtk.xml";
        var version = "1.3.0";
        var serviceTitle = "GDI MV DTK WMS";
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName));
        WmsCapabilities cap = null;

        // Act
        Assert.DoesNotThrow(() => cap = new WmsCapabilities(XDocument.Load(stream)));

        // Assert
        Assert.That(cap, Is.Not.Null);

        Console.WriteLine($"{cap.Service.Title} (WMS {cap.Version.VersionString})");
        if (!string.IsNullOrWhiteSpace(serviceTitle))
            Assert.That(cap.Service.Title, Is.EqualTo(serviceTitle));

        if (!string.IsNullOrWhiteSpace(version))
            Assert.That(cap.Version.VersionString, Is.EqualTo(version));
    }

    [Test]
    public void WmsCapabilitiesChildInheritsCrsFromParentLayer()
    {
        // Arrange
        var fileName = "wms_topplus_web_open.xml";
        using var stream = File.OpenRead(Path.Combine(Paths.AssemblyDirectory, "Resources", "Wms", fileName));

        // Act
        var wmsCapabilities = new WmsCapabilities(XDocument.Load(stream));

        // Assert
        Assert.AreEqual(16, wmsCapabilities.Capability.Layer.ChildLayers.Count);
        foreach (var layerChildLayer in wmsCapabilities.Capability.Layer.ChildLayers)
        {
            Assert.True(layerChildLayer.CRS.Count > 0);
        }
    }
}
