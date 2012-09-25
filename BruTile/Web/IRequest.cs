// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Web
{
    public interface IRequest
    {
        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        Uri GetUri(TileInfo info);
    }

    /// <summary>
    /// NullRequest class is a placeholder for request builders for tile providers. It has no other use!
    /// </summary>
    public sealed class NullRequest : IRequest
    {
        internal NullRequest()
        {}
        
        #region Implementation of IRequest

        public Uri GetUri(TileInfo info)
        {
            throw new NotSupportedException("NullRequest is a placeholder in order to instantiate tile providers.");
        }

        #endregion
    }
}
