// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile;

/// <summary>
/// Interface for a tile source.
/// </summary>
public interface ITileSource
{
    /// <summary>
    /// Gets a value indicating the tile schema
    /// </summary>
    ITileSchema Schema { get; }

    /// <summary>
    /// Gets a value indicating the title of the tile source
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the attribution text with respect the provided data
    /// </summary>
    Attribution Attribution { get; }
}
