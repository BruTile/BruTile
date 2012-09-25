// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using BruTile.Cache;

namespace BruTile.Web
{
    public class WebTileProvider : ITileProvider
    {
        #region Fields

        readonly IRequest _request;
        readonly ITileCache<byte[]> _cache;
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

        /// <summary>
        /// Creates an instance of this class using <see cref="NullRequest"/> as a placeholder for the actual request builder.
        /// </summary>
        /// <remarks>This constructor is needed to make serialization possible.</remarks>
        public WebTileProvider()
            :this(new NullRequest())
        {}

        public WebTileProvider(IRequest request)
            : this(request, new NullCache())
        {
        }

        public WebTileProvider(IRequest request, ITileCache<byte[]> cache)
            : this(request, cache, String.Empty, String.Empty, true)
        {
        }

        public WebTileProvider(IRequest request, string userAgent, string referer, bool keepAlive)
            : this(request, new NullCache(), userAgent, referer, keepAlive)
        {
        }

        public WebTileProvider(IRequest request, ITileCache<byte[]> cache,
            string userAgent, string referer, bool keepAlive)
        {
            if (request == null) throw new ArgumentException("RequestBuilder can not be null");
            _request = request;

            if (cache == null) throw new ArgumentException("FileCache can not be null");
            _cache = cache;

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
            var bytes = _cache.Find(tileInfo.Index);
            if (bytes == null)
            {
                bytes = RequestHelper.FetchImage(_request.GetUri(tileInfo), _userAgent, _referer, _keepAlive);
                if (bytes != null)
                    _cache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }

        #endregion
    }
}
