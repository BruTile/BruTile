// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using BruTile.Tms;
using NUnit.Framework;

namespace BruTile.Tests.Tms;

[TestFixture]
internal class TileMapServiceTests
{
    private const string TileMapServiceResource =
        "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
        "<TileMapService version=\"1.0.0\" services=\"http://tms.osgeo.org\">" +
        "<Title>Example Tile Map Service</Title>" +
        "<Abstract>This is a longer description of the example tiling map service.</Abstract>" +
        "<KeywordList>example tile service</KeywordList>" +
        "<ContactInformation>" +
        "<ContactPersonPrimary>" +
        "<ContactPerson>Paul Ramsey</ContactPerson>" +
        "<ContactOrganization>Refractions Research</ContactOrganization>" +
        "</ContactPersonPrimary>" +
        "<ContactPosition>Manager</ContactPosition>" +
        "<ContactAddress>" +
        "<AddressType>postal</AddressType>" +
        "<Address>300 - 1207 Douglas Street</Address>" +
        "<City>Victoria</City>" +
        "<StateOrProvince>British Columbia</StateOrProvince>" +
        "<PostCode>V8W2E7</PostCode>" +
        "<Country>Canada</Country>" +
        "</ContactAddress>" +
        "<ContactVoiceTelephone>12503833022</ContactVoiceTelephone>" +
        "<ContactFacsimileTelephone>12503832140</ContactFacsimileTelephone>" +
        "<ContactElectronicMailAddress>pramsey@refractions.net</ContactElectronicMailAddress>" +
        "</ContactInformation>" +
        "<TileMaps>" +
        "<TileMap " +
        "title=\"VMAP0 World Map\" " +
        "srs=\"EPSG:4326\" " +
        "profile=\"global-geodetic\" " +
        "href=\"http://tms.osgeo.org/1.0.0/vmap0\" />" +
        "<TileMap " +
        "title=\"British Columbia Landsat Imagery (2000)\" " +
        "srs=\"EPSG:3005\" " +
        "profile=\"local\" " +
        "href=\"http://tms.osgeo.org/1.0.0/landsat2000\" />" +
        "</TileMaps>" +
        "</TileMapService>";

    [Test]
    public void CreateFromResource()
    {
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(TileMapServiceResource));
        var tileMapService = TileMapService.CreateFromResource(stream);
        Assert.True(tileMapService.TileMaps.Count() == 2);
        Assert.True(tileMapService.TileMaps.Count(t => t.Profile == "global-geodetic") == 1);
        Assert.True(tileMapService.TileMaps.First(t => t.Title == "VMAP0 World Map").Srs == "EPSG:4326");
    }
}
