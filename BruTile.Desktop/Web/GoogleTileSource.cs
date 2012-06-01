#region License

// This code is based on information provided by http://greatmaps.codeplex.com
// Adapted for BruTile by Felix Obermaier. 2010
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

#endregion

using BruTile.PreDefined;
using BruTile.Cache;

namespace BruTile.Web
{
    public class GoogleTileSource : ITileSource
    {
        private readonly SphericalMercatorInvertedWorldSchema _tileSchema;
        private readonly GoogleTileProvider _tileProvider;
        public const string UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.1.7) Gecko/20091221 Firefox/3.5.7";
        public const string Referer = "http://maps.google.com/";

        public GoogleTileSource(GoogleMapType mapType)
            : this(new GoogleRequest(mapType), new NullCache())
        {
        }
        public GoogleTileSource(GoogleRequest request)
            : this(request, new NullCache())
        {
        }
        
        public GoogleTileSource(GoogleRequest request, ITileCache<byte[]> fileCache)
        {
            _tileSchema = new SphericalMercatorInvertedWorldSchema();
            _tileProvider = new GoogleTileProvider(request, fileCache);
        }

        #region Implementation of ITileSource

        public ITileProvider Provider
        {
            get { return _tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return _tileSchema; }
        }

        #endregion
    }
}
