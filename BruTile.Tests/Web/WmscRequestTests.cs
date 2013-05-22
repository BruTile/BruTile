using System;
using System.Collections.Generic;
using System.Linq;
using BruTile.Predefined;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Web
{
    [TestFixture]
    class WmscRequestTest
    {
        [Test]
        public void WmscRequest_NoVersion()
        {
            var request = new WmscRequest(new Uri("http://testserver.com"), new SphericalMercatorWorldSchema(), new List<string>(new string[] { "Layer One" }), null, null);
            var ti = new TileInfo();
            var uri = request.GetUri(ti);
            StringAssert.DoesNotContain("VERSION=", uri.ToString());
            StringAssert.Contains("SRS=", uri.ToString());
        }

        [Test]
        public void WmscRequest_Version111()
        {
            var request = new WmscRequest(new Uri("http://testserver.com"), new SphericalMercatorWorldSchema(), new List<string>(new string[] { "Layer One" }), null, null, "1.1.1");
            var ti = new TileInfo();
            var uri = request.GetUri(ti);
            StringAssert.Contains("VERSION=1.1.1", uri.ToString());
            StringAssert.Contains("SRS=", uri.ToString());
        }

        [Test]
        public void WmscRequest_Version130()
        {
            var request = new WmscRequest(new Uri("http://testserver.com"), new SphericalMercatorWorldSchema(), new List<string>(new string[] { "Layer One" }), null, null, "1.3.0");
            var ti = new TileInfo();
            var uri = request.GetUri(ti);
            StringAssert.Contains("VERSION=1.3.0", uri.ToString());
            StringAssert.Contains("CRS=", uri.ToString());
        }
    }
}
