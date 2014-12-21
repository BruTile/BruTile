// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile
{
    /// <summary>
    /// Interface for a tile source.
    /// </summary>
    /// <remarks>A tile provider is tuple of <see cref="ITileProvider"/> and <see cref="ITileSchema"/>.</remarks>
    public interface ITileSource : ITileProvider
    {
        /// <summary>
        /// Gets a value indicating the tile schema
        /// </summary>
        ITileSchema Schema { get; }

        /// <summary>
        /// Gets a value indicating the title of the tile source
        /// </summary>
        string Name { get; }
    }
}