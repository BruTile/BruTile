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
using System.Globalization;
using System.IO;
using System.Net;

namespace BruTile
{
    public static class ImageRequest
    {
        public static byte[] GetImageFromServer(Uri url)
        {
            WebRequest webRequest = WebRequest.Create(url);

            //This clumsy way to do a synchronous request is for compatibility with Silverlight
            IAsyncResult result = webRequest.BeginGetResponse(null, null);
            result.AsyncWaitHandle.WaitOne();
            WebResponse webResponse = webRequest.EndGetResponse(result);

            if (webResponse.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    return Utilities.ReadFully(responseStream);
                }
            }
            else
            {
                string message = CreateErrorMessage(webResponse, url.AbsoluteUri);
                throw (new WebResponseFormatException(message, null));
            }
        }

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
            using (StreamReader streamReader = new StreamReader(responseStream, true))
            {
                using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    stringWriter.Write(streamReader.ReadToEnd());
                    return stringWriter.ToString();
                }
            }
        }
    }
}
