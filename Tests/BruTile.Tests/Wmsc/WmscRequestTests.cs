using BruTile.Predefined;
using BruTile.Wmsc;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BruTile.Tests.Wmsc
{
    [TestFixture]
    class WmscRequestTests
    {
        [Test]
        public void WmscRequest_NoVersion()
        {
            var request = new WmscRequest(new Uri("http://testserver.com"), new GlobalSphericalMercator(YAxis.TMS), new List<string>(new [] { "Layer One" }), null);
            var ti = new TileInfo {Index = new TileIndex(0, 0, "0")};
            var uri = request.GetUri(ti);
            StringAssert.DoesNotContain("VERSION=", uri.ToString());
            StringAssert.Contains("SRS=", uri.ToString());
        }

        [Test]
        public void WmscRequest_Version111()
        {
            var request = new WmscRequest(new Uri("http://testserver.com"), new GlobalSphericalMercator(YAxis.TMS), new List<string>(new[] { "Layer One" }), null, null, "1.1.1");
            var ti = new TileInfo { Index = new TileIndex(0, 0, "0") };
            var uri = request.GetUri(ti);
            StringAssert.Contains("VERSION=1.1.1", uri.ToString());
            StringAssert.Contains("SRS=", uri.ToString());
        }

        [Test]
        public void WmscRequest_Version130()
        {
            var request = new WmscRequest(new Uri("http://testserver.com"), new GlobalSphericalMercator(YAxis.TMS), new List<string>(new[] { "Layer One" }), null, null, "1.3.0");
            var ti = new TileInfo { Index = new TileIndex(0, 0, "0") };
            var uri = request.GetUri(ti);
            StringAssert.Contains("VERSION=1.3.0", uri.ToString());
            StringAssert.Contains("CRS=", uri.ToString());
        }
    }
}
