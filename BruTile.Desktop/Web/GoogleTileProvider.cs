// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using BruTile.Cache;

namespace BruTile.Web
{
    [Serializable]
    class GoogleTileProvider : ITileProvider
    {
        readonly ITileCache<byte[]> _tileCache;
        readonly IRequest _request;
        
        public static int Timeout { get; set; }

        public GoogleTileProvider(IRequest request, ITileCache<byte[]> fileCache = null)
        {
            _tileCache = fileCache ?? new NullCache();
            _request = request;
            Timeout = 5000;
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = _tileCache.Find(tileInfo.Index);

            if (bytes == null)
            {
                bytes = FetchImage(_request.GetUri(tileInfo), GoogleTileSource.UserAgent, GoogleTileSource.Referer, true);
                if (bytes != null)
                    _tileCache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }

        private static byte[] FetchImage(Uri uri, string userAgent, string referer, bool keepAlive)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Timeout = Timeout;
            webRequest.UserAgent = userAgent;
            webRequest.Referer = referer;
            webRequest.KeepAlive = keepAlive;
            using (var webResponse = webRequest.GetResponse())
            {
                if (webResponse.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        return Utilities.ReadFully(responseStream);
                    }
                }
                var message = ComposeErrorMessage(webResponse, uri.AbsoluteUri);
                throw (new Exception(message, null));
            }
        }

        private static string ComposeErrorMessage(WebResponse webResponse, string uri)
        {
            var message = String.Format(
                CultureInfo.InvariantCulture,
                "Failed to retrieve tile from this uri:\n{0}\n.An image was expected but the received type was '{1}'.",
                uri,
                webResponse.ContentType
            );

            if (webResponse.ContentType.StartsWith("text", StringComparison.OrdinalIgnoreCase))
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    message += String.Format(CultureInfo.InvariantCulture,
                      "\nThis was returned:\n{0}", ReadAllText(stream));
                }
            }
            return message;
        }

        private static string ReadAllText(Stream responseStream)
        {
            using (var streamReader = new StreamReader(responseStream, true))
            {
                using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    stringWriter.Write(streamReader.ReadToEnd());
                    return stringWriter.ToString();
                }
            }
        }
    }
}
