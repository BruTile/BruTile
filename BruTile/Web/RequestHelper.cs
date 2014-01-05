// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Net;
using BruTile.Extensions;

namespace BruTile.Web
{
    public static class RequestHelper
    {
        static RequestHelper()
        {
            Timeout = 10000;
        }

        public static int Timeout { get; set; }

        public static byte[] FetchImage(HttpWebRequest webRequest)
        {
            WebResponse webResponse = webRequest.GetSyncResponse(Timeout);
            if (webResponse == null) throw (new WebException("An error occurred while fetching tile", null));
            if (webResponse.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    return Utilities.ReadFully(responseStream);
                }
            }
            string message = ComposeErrorMessage(webResponse, webRequest.RequestUri.AbsoluteUri);
            throw (new WebResponseFormatException(message, null));
        }

        public static byte[] FetchImage(Uri uri)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            return FetchImage(webRequest);
        }

        private static string ComposeErrorMessage(WebResponse webResponse, string uri)
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