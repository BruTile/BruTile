// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Web;

public interface IUrlBuilder
{
    /// <summary>
    /// Generates a URI at which to get the data for a tile.
    /// </summary>
    /// <param name="info">Information about a tile.</param>
    /// <returns>The URI at which to get the data for the specified tile.</returns>
    Uri GetUrl(TileInfo info);
}

[Obsolete("Use IUrlBuilder instead.")]
public interface IRequest : IUrlBuilder
{ }
