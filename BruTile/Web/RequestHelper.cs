// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
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

        public static ICredentials Credentials { get; set; }

        public static byte[] FetchImage(HttpWebRequest webRequest)
        {
            using (var webResponse = webRequest.GetSyncResponse(Timeout))
            {
                if (webResponse == null)
                {
                    throw (new WebException("An error occurred while fetching tile", null));
                }

                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    return Utilities.ReadFully(responseStream);
                }
            }
        }

        public static byte[] FetchImage(Uri uri)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            if (Credentials != null)
            {
                webRequest.Credentials = Credentials;
            }
            else
            {
                webRequest.UseDefaultCredentials = true;
            }

            return FetchImage(webRequest);
        }
    }
}