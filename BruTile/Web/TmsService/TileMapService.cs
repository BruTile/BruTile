// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BruTile.Web.TmsService
{
    public class TileMapService
    {
        public IEnumerable<TileMapItem> TileMaps { get; set; }
        public string Version { get; set; }

        public static TileMapService CreateFromResource(Stream result)
        {
            var tileMapService = new TileMapService();
            
            XDocument xml = XDocument.Parse(new StreamReader(result).ReadToEnd());

            tileMapService.TileMaps =
                from tileMap in xml.Root.Descendants("TileMap")
                select new TileMapItem
                {
                    Href = tileMap.Attribute("href").Value,
                    Srs = tileMap.Attribute("srs").Value,
                    Title = tileMap.Attribute("title").Value,
                    Profile = tileMap.Attribute("profile").Value
                };

            return tileMapService;
        }
    }

    public class TileMapItem 
    {
        public string Href { get; set; }
        public string Srs { get; set; }
        public string Title { get; set; }
        public string Profile { get; set; }
    }
}
