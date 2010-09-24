using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BruTile.Web.Tms
{
    public class Services
    {
        public IEnumerable<ServicesTileMapService> TileMapServices { get; set; }

        public static Services CreateFromXmlStream(Stream result)
        {
            XDocument xml = XDocument.Parse(new StreamReader(result).ReadToEnd());

            var services = new Services();
            if (xml.Root != null)
            {
                services.TileMapServices =
                    from tileMapService in xml.Root.Descendants("TileMapService")
                    select new ServicesTileMapService
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

    public class ServicesTileMapService
    {
        public string Href { get; set; }
        public string Version { get; set; }
        public string Title { get; set; }
    }
}