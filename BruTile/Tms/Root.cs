// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BruTile.Tms;

public class Root
{
    public IEnumerable<TileMapServiceItem> TileMapServices { get; set; }

    public static Root CreateFromResource(Stream result)
    {
        var xml = XDocument.Parse(new StreamReader(result).ReadToEnd());

        var services = new Root();
        if (xml.Root != null)
        {
            services.TileMapServices = xml.Root.Descendants("TileMapService")
                .Select(tileMapService => new TileMapServiceItem
                {
                    Href = tileMapService.Attribute("href")?.Value ?? string.Empty,
                    Title = tileMapService.Attribute("title")?.Value ?? string.Empty,
                    Version = tileMapService.Attribute("version")?.Value ?? string.Empty,
                });
        }
        return services;
    }
}

public class TileMapServiceItem
{
    public string Href { get; set; }
    public string Version { get; set; }
    public string Title { get; set; }
}
