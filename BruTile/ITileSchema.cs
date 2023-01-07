// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace BruTile
{
    /// <summary>
    /// Interface definition of a tile schema
    /// </summary>
    public interface ITileSchema
    {
        /// <summary>
        /// Gets a value indicating the name of the tile schema
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating the spatial reference system (srs) of the tile schema
        /// </summary>
        string Srs { get; }

        /// <summary>
        /// Gets a value indicating the extent covered by this tile schema
        /// </summary>
        Extent Extent { get; }

        /// <summary>
        /// Function to get a tile's width for a given zoom level.
        /// </summary>
        /// <param name="level">The zoom level</param>
        /// <returns>The width of the tile</returns>
        int GetTileWidth(int level);

        /// <summary>
        /// Function to get a tile's height for a given zoom level.
        /// </summary>
        /// <param name="level">The zoom level</param>
        /// <returns>The height of the tile</returns>
        int GetTileHeight(int level);

        /// <summary>
        /// Function to get the x vertex of the schemas origin for a given zoom level.
        /// </summary>
        /// <param name="level">The zoom level</param>
        /// <returns>The x vertex of the origin</returns>
        double GetOriginX(int level);

        /// <summary>
        /// Function to get the y vertex of the schemas origin for a given zoom level.
        /// </summary>
        /// <param name="level">The zoom level</param>
        /// <returns>The y vertex of the origin</returns>
        double GetOriginY(int level);

        /// <summary>
        /// Function to get the matrix width (aka number of columns) of the schema for a given zoom level.
        /// </summary>
        /// <param name="level">The zoom level</param>
        /// <returns>The matrix width</returns>
        long GetMatrixWidth(int level);

        /// <summary>
        /// Function to get the matrix height (aka number of rows) of the schema for a given zoom level.
        /// </summary>
        /// <param name="level">The zoom level</param>
        /// <returns>The matrix height</returns>
        long GetMatrixHeight(int level);

        /// <summary>
        /// Gets a value indicating the resolutions defined in this schema
        /// </summary>
        IDictionary<int, Resolution> Resolutions { get; }

        /// <summary>
        /// Gets a value indicating the file format of the tiles
        /// </summary>
        string Format { get; }

        /// <summary>
        /// Gets a value indicating the orientation of the y-axis
        /// </summary>
        YAxis YAxis { get; }

        /// <summary>
        /// Function to get the <see cref="TileInfo"/>s for a given extent and zoom level.
        /// </summary>
        /// <param name="extent">The extent for which to get the tiles</param>
        /// <param name="level">The zoom level</param>
        /// <returns>A number of <see cref="TileInfo"/>s</returns>
        IEnumerable<TileInfo> GetTileInfos(Extent extent, int level);

        /// <summary>
        /// Function to get the <see cref="TileInfo"/>s for a given extent and unitsPerPixel.
        /// </summary>
        /// <param name="extent">The extent for which to get the tiles</param>
        /// <param name="unitsPerPixel">The unitsPerPixel</param>
        /// <returns>A number of <see cref="TileInfo"/>s</returns>
        IEnumerable<TileInfo> GetTileInfos(Extent extent, double unitsPerPixel);

        /// <summary>
        /// Function to get the intersection of requested <paramref name="extent"/> 
        /// and this schemas <see cref="Extent"/> for a given zoom level.
        /// </summary>
        /// <param name="extent">The extent for which to get tiles</param>
        /// <param name="level">The zoom level</param>
        /// <returns>The intersection of requested extent and this schemas extent</returns>
        Extent GetExtentOfTilesInView(Extent extent, int level);

        /// <summary>
        /// Function to get the first matrix column served by this schema for a given zoom level. 
        /// </summary>
        /// <param name="level">The zoom level</param>
        int GetMatrixFirstCol(int level);

        /// <summary>
        /// Function to get the first matrix row served by this schema for a given zoom level. 
        /// </summary>
        /// <param name="level">The zoom level</param>
        int GetMatrixFirstRow(int level);
    }
}
