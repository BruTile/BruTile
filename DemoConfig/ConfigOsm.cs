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

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using BruTile;
using BruTile.Web;

namespace DemoConfig
{
    public class ConfigOsm : IConfig
    {
        public ITileSource CreateTileSource()
        {
            return new TileSource(Provider, Schema);
        }

        public static ITileProvider Provider
        {
            get
            {
                return new WebTileProvider(RequestBuilder);
            }
        }

        public static ITileSchema Schema
        {
            get
            {
                double[] resolutions = new double[] { 
                    156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 
                    9783.939618750, 4891.969809375, 2445.984904688, 1222.992452344, 
                    611.496226172, 305.748113086, 152.874056543, 76.437028271, 
                    38.218514136, 19.109257068, 9.554628534, 4.777314267, 
                    2.388657133, 1.194328567, 0.597164283};

                string name = "OpenStreetMap";
       
                TileSchema schema = new TileSchema();
                foreach (double resolution in resolutions) schema.Resolutions.Add(resolution);
                schema.Height = 256;
                schema.Width = 256;
                schema.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
                schema.OriginX = -20037508.342789;
                schema.OriginY = 20037508.342789;
                schema.Name = name;
                schema.Format = "png";
                schema.Axis = AxisDirection.InvertedY;
                schema.Srs = "EPSG:3785";
                return schema;
            }
        }

        private static IRequestBuilder RequestBuilder
        {
            get
            {
                string url = "http://b.tile.openstreetmap.org";

                return new RequestTms(new Uri(url), "png");
            }
        }
    }
}
