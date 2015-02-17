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
            Resolutions = new Dictionary<string, Resolution>();
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

            var res = new WmtsTileSchema();
            res.YAxis = YAxis;
            res.Extent = new Extent(Extent.MinX, Extent.MinY, Extent.MaxX, Extent.MaxY);
            res.Title = title;
            res.Layer = layer;
            res.Abstract = @abstract;
            res.TileMatrixSet = tileMatrixSet;
            res.Style = style;
            res.Format = format;
            res.Name = Name;
            foreach (var resolution in Resolutions)
                res.Resolutions.Add(resolution);
            res.Srs = Srs;
            res.SupportedSRS = SupportedSRS;
            return res;
        }

        /// <summary>
        /// Gets or sets a value indicating the content of the layer
        /// </summary>
        public string Abstract { get; private set; }

        public string Name
        {
            get;
            set;
        }

        public string Srs { get; set; }

        public Extent Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }

        public string Format { get; set; }
        
        public YAxis YAxis { get; set; }
        
        public IDictionary<string, Resolution> Resolutions { get; set; }

        public int GetTileWidth(string levelId)
        {
            return Resolutions[levelId].TileWidth;
        }

        public int GetTileHeight(string levelId)
        {
            return Resolutions[levelId].TileHeight;
        }

        public double GetOriginX(string levelId)
        {
            return Resolutions[levelId].Left;
        }

        public double GetOriginY(string levelId)
        {
            return Resolutions[levelId].Top;
        }

        public int GetMatrixWidth(string levelId)
        {
            return Resolutions[levelId].MatrixWidth;
        }

        public int GetMatrixHeight(string levelId)
        {
            return Resolutions[levelId].MatrixHeight;
        }
        
        /// <summary>
        /// Returns a List of TileInfos that cover the provided extent. 
        /// </summary>
        public IEnumerable<TileInfo> GetTileInfos(Extent extent, double resolution)
        {
            var level = Utilities.GetNearestLevel(Resolutions, resolution);
            return GetTileInfos(extent, level);
        }

        public IEnumerable<TileInfo> GetTileInfos(Extent extent, string levelId)
        {
            return TileSchema.GetTileInfos(this, extent, levelId);
        }

        public Extent GetExtentOfTilesInView(Extent extent, string levelId)
        {
            return TileSchema.GetExtentOfTilesInView(this, extent, levelId);
        }

        public int GetMatrixFirstCol(string levelId)
        {
            return 0; // always zero because WMTS can not have a discrepancy between schema origin and bbox origin
        }

        public int GetMatrixFirstRow(string levelId)
        {
            return 0; // always zero because WMTS can not have a discrepancy between schema origin and bbox origin
        }
    }
}
