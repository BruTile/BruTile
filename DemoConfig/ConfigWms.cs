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
using Tiling;
using System.Collections.Generic;

namespace DemoConfig
{
  public class ConfigWms : IConfig
  {
    string format = "png";
    string name = "Geodan WMS";
    string url = "http://geoserver.nl/world/mapserv.cgi?map=world/world.map&VERSION=1.1.1";

    private static double[] resolutions = new double[] { 
      156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 
      9783.939618750, 4891.969809375, 2445.984904688, 1222.992452344, 
      611.496226172, 305.748113086, 152.874056543, 76.437028271, 
      38.218514136, 19.109257068, 9.554628534, 4.777314267, 
      2.388657133, 1.194328567, 0.597164283};

    #region IConfig Members

    public IRequestBuilder RequestBuilder
    {
      get
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("layers", "world");
        RequestWmsC request = new RequestWmsC(new Uri(url), this.TileSchema, dictionary);
        return request;
      }
    }

    public ITileSchema TileSchema
    {
      get 
      {
        TileSchemaWmsC schema = new TileSchemaWmsC();
        foreach (double resolution in resolutions) schema.Resolutions.Add(resolution);
        schema.Height = 256;
        schema.Width = 256;
        schema.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
        schema.OriginX = -20037508.342789;
        schema.OriginY = 20037508.342789;
        schema.Name = name;
        schema.Format = format;
        schema.Axis = AxisDirection.InvertedY;
        schema.Srs = "EPSG:900913";

        //extra for WmsC
        schema.Layers = new List<string>(new string[] { "world" });
        schema.Styles = new List<string>(new string[] { "" });
        
        return schema;
      }
    }

    #endregion
  }
}
