#region License

// Copyright 2008 - Paul den Dulk (Geodan)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BruTile.Web.TmsService
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