using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BruTile.Web.Tms
{
    public class TileMapService
    {
        public IEnumerable<TileMap> TileMaps { get; set; }
        public string Version { get; set; }

        public static TileMapService CreateFromXmlStream(Stream result)
        {
            var tileMapService = new TileMapService();
            
            XDocument xml = XDocument.Parse(new StreamReader(result).ReadToEnd());

            tileMapService.TileMaps =
                from tileMap in xml.Root.Descendants("TileMap")
                select new TileMap
                {
                    Href = tileMap.Attribute("href").Value,
                    Srs = tileMap.Attribute("srs").Value,
                    Title = tileMap.Attribute("title").Value,
                    Profile = tileMap.Attribute("profile").Value
                };

            return tileMapService;
        }
    }

    public class TileMap
    {
        public string Href { get; set; }
        public string Srs { get; set; }
        public string Title { get; set; }
        public string Profile { get; set; }
    }
}
