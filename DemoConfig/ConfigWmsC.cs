﻿// Copyright 2008 - Paul den Dulk (Geodan)
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

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using BruTile;
using BruTile.Web;
using BruTile.PreDefined;

namespace DemoConfig
{
    public class ConfigWmsC : IConfig
    {
        public ITileSource CreateTileSource()
        {
            ITileSchema schema = new SchemaWorldSphericalMercator();
            ITileSource source = new TileSource(GetTileProvider(schema), schema);
            return source;
        }

        private static ITileProvider GetTileProvider(ITileSchema schema)
        {
            return new WebTileProvider(GetRequestBuilder(schema));
        }

        private static IRequestBuilder GetRequestBuilder(ITileSchema schema)
        {
            string url = "http://geoserver.nl/tiles/tilecache.aspx?";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("seriveparam", "ortho10");
            RequestWmsC request = new RequestWmsC(new Uri(url), schema,
              new List<string>(new string[] { "world_GM" }), new List<string>(), parameters);
            return request;
        }
    }
}
