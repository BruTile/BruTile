// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using BruTile.Tms;
using NUnit.Framework;

namespace BruTile.Tests.Tms
{
    [TestFixture]
    internal class RootTests
    {
        private const string RootResource =
            "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
            "<Services>" +
            "<TileMapService title=\"Example Tile Map Service\" version=\"1.0.0\" href=\"http://tms.osgeo.org/1.0.0/\" />" +
            "<TileMapService title=\"New Example Tile Map Service\" version=\"1.1.0\" href=\"http://tms.osgeo.org/1.1.0/\" />" +
            "<FancyFeatureService title=\"Features!\" version=\"0.9\" href=\"http://ffs.osgeo.org/0.9/\" />" +
            "</Services>";

        [Test]
        public void CreateFromResource()
        {
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(RootResource));
            var root = Root.CreateFromResource(stream);
            Assert.True(root.TileMapServices.Count() == 2);
            Assert.True(root.TileMapServices.First(tms => tms.Title == "Example Tile Map Service").Version == "1.0.0");
        }
    }
}
