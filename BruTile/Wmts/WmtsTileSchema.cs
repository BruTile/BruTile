// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace BruTile.Wmts
{
    public class WmtsTileSchema : ITileSchema
    {
        private Extent _extent;

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        internal WmtsTileSchema()
        {
            Resolutions = new Dictionary<int, Resolution>();
            YAxis = YAxis.OSM;
        }

        /// <summary>
        /// Gets an identifier for the layer and tile matrix set.
        /// </summary>
        public string Identifier { get { return Layer + "(" + TileMatrixSet + ")"; } }

        /// <summary>
        /// The layer identifier
        /// </summary>
        public string Layer { get; set; }

        /// <summary>
        /// The title of the layer
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The identifier of the tile matrix set
        /// </summary>
        public string TileMatrixSet { get; set; }

        /// <summary>
        /// Gets or sets the supported spatial reference system
        /// </summary>
        public CrsIdentifier SupportedSRS { get; internal set; }

        /// <summary>
        /// Gets a value indicating the style
        /// </summary>
        public string Style { get; private set; }

        /// <summary>
        /// Creates a copy of this schema with <see cref="Format"/> set to <paramref name="format"/>
        /// </summary>
        /// <param name="title">The layer title</param>
        /// <param name="layer">The layer identifier</param>
        /// <param name="abstract">A description for the layers content</param>
        /// <param name="tileMatrixSet">The TileMatrixSet identifier</param>
        /// <param name="style">The style identifier</param>
        /// <param name="format">The format used for this style</param>
        /// <returns>A tile schema</returns>
        internal WmtsTileSchema CreateSpecific(string title, string layer, string @abstract, string tileMatrixSet, string style, string format)
        {
            if (string.IsNullOrEmpty(layer))
                throw new ArgumentNullException("layer");
            if (string.IsNullOrEmpty(tileMatrixSet))
                throw new ArgumentNullException("tileMatrixSet");
            if (string.IsNullOrEmpty(format))
                throw new ArgumentNullException("format");

            if (@abstract == null) @abstract = string.Empty;
            if (string.IsNullOrEmpty(style)) style = "null";

            if (!format.StartsWith("image/"))
                throw new ArgumentException("Not an image mime type");

            var res = new WmtsTileSchema
            {
                YAxis = YAxis,
                Extent = new Extent(Extent.MinX, Extent.MinY, Extent.MaxX, Extent.MaxY),
                Title = title,
                Layer = layer,
                Abstract = @abstract,
                TileMatrixSet = tileMatrixSet,
                Style = style,
                Format = format,
                Name = Name,
                Srs = Srs,
                SupportedSRS = SupportedSRS
            };

            foreach (var resolution in Resolutions) res.Resolutions.Add(resolution);

            return res;
        }

        /// <summary>
        /// Gets or sets a value indicating the content of the layer
        /// </summary>
        public string Abstract { get; private set; }

        /// <summary>
        /// Gets a value indicating the name of the tile schema
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating the spatial reference system (srs) of the tile schema
        /// </summary>
        public string Srs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the extent covered by this tile schema
        /// </summary>
        public Extent Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }

        /// <summary>
        /// Gets a value indicating the file format of the tiles
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets a value indicating the orientation of the y-axis
        /// </summary>
        public YAxis YAxis { get; set; }

        public IDictionary<int, Resolution> Resolutions { get; set; }

        public int GetTileWidth(int level)
        {
            return Resolutions[level].TileWidth;
        }

        public int GetTileHeight(int level)
        {
            return Resolutions[level].TileHeight;
        }

        public double GetOriginX(int level)
        {
            return Resolutions[level].Left;
        }

        public double GetOriginY(int level)
        {
            return Resolutions[level].Top;
        }

        public long GetMatrixWidth(int level)
        {
            return Resolutions[level].MatrixWidth;
        }

        public long GetMatrixHeight(int level)
        {
            return Resolutions[level].MatrixHeight;
        }

        /// <summary>
        /// Returns a List of TileInfos that cover the provided extent. 
        /// </summary>
        public IEnumerable<TileInfo> GetTileInfos(Extent extent, double unitsPerPixel)
        {
            var level = Utilities.GetNearestLevel(Resolutions, unitsPerPixel);
            return GetTileInfos(extent, level);
        }

        public IEnumerable<TileInfo> GetTileInfos(Extent extent, int level)
        {
            return TileSchema.GetTileInfos(this, extent, level);
        }

        public Extent GetExtentOfTilesInView(Extent extent, int level)
        {
            return TileSchema.GetExtentOfTilesInView(this, extent, level);
        }

        public int GetMatrixFirstCol(int level)
        {
            return 0; // Always zero because WMTS can not have a discrepancy between schema origin and bounding box origin
        }

        public int GetMatrixFirstRow(int level)
        {
            return 0; // Always zero because WMTS can not have a discrepancy between schema origin and bounding box origin
        }

        public IDictionary<int, string> LevelToIdentifier { get; set;} = new Dictionary<int, string>();
    }
}
