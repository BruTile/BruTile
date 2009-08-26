using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace BruTile
{
    /// <summary>
    /// A flexible request builder that can be used for a number of simple cases.
    /// </summary>
    public class RequestBasic : IRequestBuilder
    {
        string urlFormatter;

        public RequestBasic(string urlFormatter)
        {
            this.urlFormatter = urlFormatter;
        }

        public Uri GetUrl(TileInfo tile)
        {
            string result = String.Format(
              CultureInfo.InvariantCulture, urlFormatter, 
              tile.Key.Level, tile.Key.Col, tile.Key.Row);

            return new Uri(result);
        }

    }
}
