using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BruTile.Web
{
    public static class HttpClientBuilder
    {
        public static HttpClientHandler HttpClientHandler { get; set; }


        public static HttpClient Build()
        {
            if (HttpClientHandler is null)
                return new HttpClient(HttpClientHandler, false);

            return new HttpClient();
        }
    }
}
