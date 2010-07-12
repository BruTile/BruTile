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
using System.Net;
using BruTile;
using System.Threading;
using BruTile.Cache;

namespace BruTile.Web
{
    public class WebTileProvider : ITileProvider
    {
        #region Fields

        readonly IRequest request;
        readonly ITileCache<byte[]> fileCache;
        string userAgent;
        string referer;
        readonly bool keepAlive = true;

        #endregion

        #region Properties

        public IRequest Request
        {
            get { return request; }
        }

        protected string UserAgent
        {
            get { return userAgent; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "UserAgent cannot be set to null!");
                userAgent = value;
            }
        }

        protected string Referer
        {
            get { return referer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Reverer cannot be set to null!");
                referer = value;
            }
        }

        #endregion

        #region Constructors

        public WebTileProvider(IRequest request)
            : this(request, new NullCache())
        {
        }

        public WebTileProvider(IRequest request, ITileCache<byte[]> fileCache)
            : this(request, fileCache, String.Empty, String.Empty, true)
        {
        }

        public WebTileProvider(IRequest request, string userAgent, string referer, bool keepAlive)
            : this(request, new NullCache(), userAgent, referer, keepAlive)
        {
        }

        public WebTileProvider(IRequest request, ITileCache<byte[]> fileCache,
            string userAgent, string referer, bool keepAlive)
        {
            if (request == null) throw new ArgumentException("RequestBuilder can not be null");
            this.request = request;

            if (fileCache == null) throw new ArgumentException("FileCache can not be null");
            this.fileCache = fileCache;

            if (userAgent == null) throw new ArgumentException("UserAgent can not be null");
            this.userAgent = userAgent;

            if (referer == null) throw new ArgumentException("UserAgent can not be null");
            this.referer = referer;

            this.keepAlive = keepAlive;
        }

        #endregion

        #region TileProvider Members

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = null;

            bytes = fileCache.Find(tileInfo.Index);
            if (bytes == null)
            {
                bytes = RequestHelper.FetchImage(request.GetUri(tileInfo), userAgent, referer, keepAlive);
                if (bytes != null)
                    fileCache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }

        #endregion
    }
}
