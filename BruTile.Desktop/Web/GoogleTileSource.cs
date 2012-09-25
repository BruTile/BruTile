// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2010.

using System;
using BruTile.PreDefined;
using BruTile.Cache;

namespace BruTile.Web
{
    [Serializable]
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
