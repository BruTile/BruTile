// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2010.

using System;
using System.Net;
using BruTile.Cache;
using BruTile.Predefined;

namespace BruTile.Web
{
    [Serializable]
    public class GoogleTileSource : ITileSource
    {
        private readonly SphericalMercatorInvertedWorldSchema _tileSchema;
        private readonly WebTileProvider _tileProvider;
        public const string UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.1.7) Gecko/20091221 Firefox/3.5.7";
        public const string Referer = "http://maps.google.com/";

        public GoogleTileSource(GoogleMapType mapType)
            : this(new GoogleRequest(mapType))
        {
        }

        public GoogleTileSource(GoogleRequest request, IPersistentCache<byte[]> persistentCache = null)
        {
            _tileSchema = new SphericalMercatorInvertedWorldSchema();
            _tileProvider = new WebTileProvider(request, persistentCache, 
                // The Google requests needs to fake the UserAgent en Referer.
                uri =>
                    {
                        var httpWebRequest = (HttpWebRequest) WebRequest.Create(uri);
                        httpWebRequest.UserAgent = UserAgent;
                        httpWebRequest.Referer = Referer;
                        return RequestHelper.FetchImage(httpWebRequest);
                    });
        }

        public ITileProvider Provider
        {
            get { return _tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return _tileSchema; }
        }
    }
}
