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
using System.Threading;

namespace BruTile
{
    public static class ImageRequest
    {
#if SILVERLIGHT
        //This #if is ugly but it is a lot simpler compared to the dependency injection 
        //solution I had before. PDD.

        public static byte[] GetImageFromServer(Uri uri)
        {
            WebClient webClient = new WebClient();

            AsyncEventArgs asyncEventArgs = new AsyncEventArgs()
            {
                WaitHandle = new AutoResetEvent(false)
            };

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
            webClient.OpenReadAsync(uri, asyncEventArgs);

            //happy hacking:
            asyncEventArgs.WaitHandle.WaitOne();

            return asyncEventArgs.Bytes;
        }

        private static void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            AsyncEventArgs state = (AsyncEventArgs)e.UserState;
            Exception exception = null;

            if (e.Error != null || e.Cancelled)
            {
                exception = e.Error;
            }
            else
            {
                try
                {
                    state.Bytes = BruTile.Utilities.ReadFully(e.Result);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }
            state.WaitHandle.Set();
        }

        private class AsyncEventArgs
        {
            public TileInfo TileInfo { get; set; }
            public AutoResetEvent WaitHandle;
            public byte[] Bytes;

        }

#else
        public static byte[] GetImageFromServer(Uri uri)
        {
            WebRequest webRequest = WebRequest.Create(uri);
            
#if !PocketPC
            IWebProxy proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            webRequest.Proxy = proxy;
            webRequest.PreAuthenticate = true;
#endif

            WebResponse webResponse = webRequest.GetResponse();
            if (webResponse.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    return Utilities.ReadFully(responseStream);
                }
            }
            else
            {
                string message = CreateErrorMessage(webResponse, uri.AbsoluteUri);
                throw (new WebResponseFormatException(message, null));
            }
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
