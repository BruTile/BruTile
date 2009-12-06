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
    public class ConfigGoogle : IConfig
    {
        public ITileSource CreateTileSource()
        {
            return new TileSource(TileProvider, TileSchema);
        }

        private ITileProvider TileProvider
        {
            get
            {
                //thanks to pascal buirey
                //http://www.codeproject.com/KB/scrapbook/googlemap.aspx
                //perhaps google needs webRequest.KeepAlive = false; ?
                //todo: look at 'server numbering and secure word'
                //nt servernum = (x + 2 * y) % 4;
                //string sec1 = ""; // after &x=...
                //string sec2 = ""&s="; // after &zoom=...
                //string secword = "Galileo";
                //int seclen = ((x * 3) + y) % 8;
                //sec2 += secword.Substring( 0, seclen );
                //if ( y >= 10000 && y < 100000 )
                //{
                //sec1 = "&s=";
                //} 
                string userAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14"; // or another agent
                string referer = "http://maps.google.com/maps";
                return new WebTileProvider(RequestBuilder, userAgent, referer, false);
            }
        }

        private ITileSchema TileSchema
        {
            get
            {
                string format = "png";
                string name = "GoogleMaps";

                double[] resolutions = new double[] { 
                    156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 
                    9783.939618750, 4891.969809375, 2445.984904688, 1222.992452344, 
                    611.496226172, 305.748113086, 152.874056543, 76.437028271, 
                    38.218514136, 19.109257068, 9.554628534, 4.777314267, 
                    2.388657133, 1.194328567, 0.597164283};

                TileSchema schema = new TileSchema();
                foreach (double resolution in resolutions) schema.Resolutions.Add(resolution);
                schema.Height = 256;
                schema.Width = 256;
                schema.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
                schema.OriginX = -20037508.342789;
                schema.OriginY = 20037508.342789;
                schema.Name = name;
                schema.Format = format;
                schema.Axis = AxisDirection.InvertedY;
                schema.Srs = "EPSG:3785";
                return schema;
            }
        }

        private IRequestBuilder RequestBuilder
        {
            get
            {
                return new RequestBasic("http://mt1.google.com/vt/lyrs=m@113&hl=nl&x={1}&y={2}&z={0}&s=");
            }
        }
    }
}
