#region License

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
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

#endregion License

using System;
using System.Globalization;
using System.IO;
using System.Net;

#if SILVERLIGHT
using System.Threading;
#endif

namespace BruTile.Web
{
    public static partial class RequestHelper
    {
        private static volatile int _timeout = 5000;

#if !SILVERLIGHT

        public sealed class EmptyWebProxy : IWebProxy
        {
            public ICredentials Credentials { get; set; }

            public Uri GetProxy(Uri uri)
            {
                return uri;
            }

            public bool IsBypassed(Uri uri)
            {
                return true;
            }
        }

        static RequestHelper()
        {
            WebProxy = new EmptyWebProxy();
        }

        public static IWebProxy WebProxy { get; set; }

#endif

        public static int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        public static byte[] FetchImage(Uri uri)
        {
            return FetchImage(uri, String.Empty, String.Empty, true);
        }

#if SILVERLIGHT
#else

        public static byte[] FetchImage(Uri uri, string userAgent, string referer, bool keepAlive)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);

            if (WebProxy != null)
            {
                webRequest.Proxy = WebProxy;
#if !PocketPC
                //IWebProxy proxy = WebRequest.DefaultWebProxy;
                //proxy.Credentials = CredentialCache.DefaultCredentials;
                webRequest.PreAuthenticate = true;
#endif
            }

            webRequest.KeepAlive = keepAlive;
            webRequest.AllowAutoRedirect = true;
            webRequest.Timeout = Timeout;

            webRequest.UserAgent = (string.IsNullOrEmpty(userAgent)) ? Utilities.DefaultUserAgent : userAgent;
            webRequest.Referer = (string.IsNullOrEmpty(referer)) ? Utilities.DefaultReferer : referer;

            WebResponse webResponse = webRequest.GetResponse();
            if (webResponse.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    return Utilities.ReadFully(responseStream);
                }
            }
            string message = CreateErrorMessage(webResponse, uri.AbsoluteUri);
            throw (new WebResponseFormatException(message, null));
        }

#endif

        private static string CreateErrorMessage(WebResponse webResponse, string uri)
        {
            string message = String.Format(
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