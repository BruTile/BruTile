#region License

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
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

using System;

namespace BruTile.Web
{
    /// <remarks>
    /// I don't like this class in its current form. It adds nothing except a more convenient constructor
    /// </remarks>
    public class TmsTileSource : ITileSource
    {
        public ITileProvider Provider { get; private set; }
        public ITileSchema Schema { get; private set; }
        
        public TmsTileSource(string serviceUrl, ITileSchema tileSchema)
             : this (new Uri(serviceUrl), tileSchema)
        {
        }

        public TmsTileSource(Uri serviceUri, ITileSchema tileSchema)
        {
            Provider = new WebTileProvider(new TmsRequest(serviceUri, tileSchema.Format));
            Schema = tileSchema;
        }
    }
}