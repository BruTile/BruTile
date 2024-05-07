// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BruTile.Tms;

public class TmsRootParser
{
    public static IEnumerable<TileMapServiceItem> CreateFromResource(Stream result)
    {
        var xml = XDocument.Parse(new StreamReader(result).ReadToEnd());

        var services = new TmsRootParser();
        if (xml.Root is null)
            throw new InvalidDataException("The root of the tms xml was null");

        return xml.Root.Descendants("TileMapService")
            .Select(tileMapService => new TileMapServiceItem(
                tileMapService.Attribute("href")?.Value ?? string.Empty,
                tileMapService.Attribute("title")?.Value ?? string.Empty,
                tileMapService.Attribute("version")?.Value ?? string.Empty));
    }
}

public record class TileMapServiceItem(string Href, string Title, string Version);

