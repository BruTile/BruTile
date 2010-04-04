// Copyright 2009 - Paul den Dulk (Geodan)
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
using System.Globalization;

namespace BruTile.Web
{
    /// <summary>
    /// A flexible request builder that can be used for a number of simple cases.
    /// </summary>
    public class BasicRequest : IRequest
    {
        string urlFormatter;

        public BasicRequest(string urlFormatter)
        {
            this.urlFormatter = urlFormatter;
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="tile">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            string result = String.Format(
              CultureInfo.InvariantCulture, urlFormatter,
              info.Index.Level, info.Index.Col, info.Index.Row);

            return new Uri(result);
        }

    }
}
