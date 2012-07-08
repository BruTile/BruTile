// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using BruTile.Cache;

namespace BruTile.Web
{
    public class WebTileProvider : ITileProvider
    {
        #region Fields

        readonly IRequest _request;
        readonly ITileCache<byte[]> _fileCache;
        string _userAgent;
        string _referer;
        readonly bool _keepAlive = true;

        #endregion

        #region Properties

        public IRequest Request
        {
            get { return _request; }
        }

        protected string UserAgent
        {
            get { return _userAgent; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "UserAgent cannot be set to null!");
                _userAgent = value;
            }
        }

        protected string Referer
        {
            get { return _referer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Reverer cannot be set to null!");
                _referer = value;
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
            _request = request;

            if (fileCache == null) throw new ArgumentException("FileCache can not be null");
            _fileCache = fileCache;

            if (userAgent == null) throw new ArgumentException("UserAgent can not be null");
            _userAgent = userAgent;

            if (referer == null) throw new ArgumentException("UserAgent can not be null");
            _referer = referer;

            _keepAlive = keepAlive;
        }

        #endregion

        #region TileProvider Members

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = _fileCache.Find(tileInfo.Index);

            if (bytes == null)
            {
                bytes = RequestHelper.FetchImage(_request.GetUri(tileInfo), _userAgent, _referer, _keepAlive);
                if (bytes != null)
                    _fileCache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }

        #endregion
    }
}
