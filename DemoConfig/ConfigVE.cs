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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;

namespace DemoConfig
{
  public class ConfigVE : IConfig
  {
    string format = "jpg";
    string name = "VirtualEarth";
    string url = "http://t1.staging.tiles.virtualearth.net/tiles/";
    string token;

    private static double[] ScalesVE = new double[] { 
      78271.516950000, 39135.758475000, 19567.879237500, 
      9783.939618750, 4891.969809375, 2445.984904688, 1222.992452344, 
      611.496226172, 305.748113086, 152.874056543, 76.437028271, 
      38.218514136, 19.109257068, 9.554628534, 4.777314267, 
      2.388657133, 1.194328567, 0.597164283, 0.298582142};

    #region IConfig Members

    public BruTile.IRequestBuilder RequestBuilder
    {
      get
      {
        //retrieve your token through your own VE account, see
        //http://msdn.microsoft.com/en-us/library/cc980844.aspx
        token = "";
        return new RequestVE(url, token);
      }
    }

    public BruTile.ITileSchema TileSchema
    {
      get
      {
        TileSchema schema = new TileSchema();
        foreach (double resolution in ScalesVE) schema.Resolutions.Add(resolution);
        schema.Height = 256;
        schema.Width = 256;
        schema.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
        schema.OriginX = -20037508.342789;
        schema.OriginY = 20037508.342789;
        schema.Name = name;
        schema.Format = format;
        schema.Axis = AxisDirection.InvertedY;
        return schema;
      }
    }

    #endregion


  }
}
