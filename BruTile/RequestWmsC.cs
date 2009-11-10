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
using System.Globalization;
using System.Text;

namespace BruTile
{
	public class RequestWmsC : IRequestBuilder
	{
		private ITileSchema schema;
		private Uri baseUrl;
		Dictionary<string, string> customParameters;
		IList<string> layers;
		IList<string> styles;

		public RequestWmsC(Uri baseUrl, ITileSchema schema, IList<string> layers, IList<string> styles, Dictionary<string, string> customParameters)
		{
			this.baseUrl = baseUrl;
			this.schema = schema;
			this.customParameters = customParameters;
			this.layers = layers;
			this.styles = styles;
		}

		public Uri GetUrl(TileInfo tile)
		{
			StringBuilder url = new StringBuilder(baseUrl.AbsoluteUri);

			//TODO: look at .net's UriBuilder for improvement
			//http://msdn.microsoft.com/en-us/library/system.uribuilder.aspx
			//note that the first & assumes a preceiding argument, this is not
			//always the case.

			url.Append("&SERVICE=WMS");
			url.Append("&REQUEST=GetMap");
			url.AppendFormat("&BBOX={0}", tile.Extent.ToString());
			url.AppendFormat("&FORMAT={0}", schema.Format);
			url.AppendFormat("&WIDTH={0}", schema.Width);
			url.AppendFormat("&HEIGHT={0}", schema.Height);
			url.AppendFormat("&SRS={0}", schema.Srs);
			url.AppendFormat("&LAYERS={0}", ToCommaSeparatedValues(layers));
			///url.AppendFormat("&STYLES={0}", ToCommaSeparatedValues(styles));

			AppendCustomParameters(url);

			return new Uri(url.ToString());
		}

		private static string ToCommaSeparatedValues(IList<string> items)
		{
			StringBuilder result = new StringBuilder();
			foreach (string str in items)
			{
				result.AppendFormat(CultureInfo.InvariantCulture, ",{0}", str);
			}
			if (result.Length > 0) result.Remove(0, 1);
			return result.ToString();
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
