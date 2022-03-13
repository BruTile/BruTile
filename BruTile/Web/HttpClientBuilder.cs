// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Net.Http;

namespace BruTile.Web
{
    public static class HttpClientBuilder
    {
        public static HttpClientHandler HttpClientHandler { get; set; }


        public static HttpClient Build()
        {
            if (HttpClientHandler != null)
                return new HttpClient(HttpClientHandler, false);

            return new HttpClient();
        }
    }
}
