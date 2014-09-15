// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BruTile.Tms
{
    public class Root
    {
        public IEnumerable<TileMapServiceItem> TileMapServices { get; set; }

        public static Root CreateFromResource(Stream result)
        {
            XDocument xml = XDocument.Parse(new StreamReader(result).ReadToEnd());

            var services = new Root();
            if (xml.Root != null)
            {
                services.TileMapServices =
                    from tileMapService in xml.Root.Descendants("TileMapService")
                    select new TileMapServiceItem
                    {
                        Href =
                            (tileMapService.Attribute("href") != null)
                                ? tileMapService.Attribute("href").Value
                                : String.Empty,
                        Title =
                            (tileMapService.Attribute("title") != null)
                                ? tileMapService.Attribute("title").Value
                                : String.Empty,
                        Version =
                            (tileMapService.Attribute("version") != null)
                                ? tileMapService.Attribute("version").Value
                                : String.Empty,
                    };

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
}