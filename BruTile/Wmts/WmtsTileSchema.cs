using System.Collections.Generic;

namespace BruTile.Wmts
{
    class WmtsTileSchema : ITileSchema
    {
        private Extent _extent;
        private Extent _wgs84Extent;

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        public WmtsTileSchema()
        {
            Resolutions = new Dictionary<string, Resolution>();
            Axis = AxisDirection.InvertedY;
        }

        /// <summary>
        /// Gets or set the identifier of the corresponding TileMatrixSet
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the supported spatial reference system
        /// </summary>
        public CrsIdentifier SupportedSRS { get; set; }


        public string Name
        {
            get { return Identifier; }
            set { Identifier = value; }
        }

        public string Srs { get; set; }

        public Extent Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }

        public string Format { get; set; }
        public AxisDirection Axis { get; set; }
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
        public IEnumerable<TileInfo> GetTilesInView(Extent extent, double resolution)
        {
            var level = Utilities.GetNearestLevel(Resolutions, resolution);
            return GetTilesInView(extent, level);
        }

        public IEnumerable<TileInfo> GetTilesInView(Extent extent, string levelId)
        {
            return TileSchema.GetTilesInView(this, extent, levelId);
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
