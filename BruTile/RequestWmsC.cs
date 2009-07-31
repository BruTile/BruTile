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
using System.Text;

namespace Tiling
{
  public class RequestWmsC : IRequestBuilder
  {
    private ITileSchema schema;
    private Uri baseUrl;
    Dictionary<string, string> customParameters;

    public RequestWmsC(Uri baseUrl, ITileSchema schema, Dictionary<string, string> customParameters)
    {
      this.baseUrl = baseUrl;
      this.schema = schema;
      this.customParameters = customParameters;
    }

    public Uri GetUrl(TileInfo tile)
    {
      StringBuilder url = new StringBuilder(baseUrl.AbsoluteUri);

      //warning
      //!!! hack, replaced with line below: url.Append("&SERVICE=WMS");
      
      url.Append("&SERVICE=WMS");
      url.Append("&REQUEST=GetMap");
      url.AppendFormat("&BBOX={0}", tile.Extent.ToString());
      url.AppendFormat("&FORMAT={0}", schema.Format);
      url.AppendFormat("&WIDTH={0}", schema.Width);
      url.AppendFormat("&HEIGHT={0}", schema.Height);
      url.AppendFormat("&SRS={0}", schema.Srs);

      url.Append(schema.Additions);
            
      AppendCustomParameters(url);

      return new Uri(url.ToString());
    }

    private void AppendCustomParameters(System.Text.StringBuilder url)
    {
      if (customParameters != null && customParameters.Count > 0)
      {
        foreach (string name in customParameters.Keys)
        {
          url.AppendFormat("&{0}={1}", name, customParameters[name]);
        }
      }
    }
  }
}
