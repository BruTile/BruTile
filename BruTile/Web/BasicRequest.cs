// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace BruTile.Web
{
    /// <summary>
    /// A flexible request builder that can be used for a number of simple cases.
    /// </summary>
    public class BasicRequest : IRequest
    {
        readonly string _urlFormatter;

        public BasicRequest(string urlFormatter)
        {
            _urlFormatter = urlFormatter;
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            string result = String.Format(
              CultureInfo.InvariantCulture, _urlFormatter,
              info.Index.Level, info.Index.Col, info.Index.Row);

            return new Uri(result);
        }

    }
}
