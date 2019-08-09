// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;

[assembly: CLSCompliant(true)]

namespace BruTile
{
    /// <summary>
    /// Enumeration of possible YAxis directions
    /// </summary>
    /// <remarks>
    /// Direction is relative to the coordinate system in which the map is presented.
    /// <para/>
    /// </remarks>
    public enum YAxis
    {
        /// <summary>
        /// The y-axis direction of the tiles match that of the map. This is used by TMS.
        /// </summary>
        TMS,

        /// <summary>
        /// The y-axis direction is inverted compared to that of the map. This is used by OpenStreetMap
        /// </summary>
        OSM
    }

    public class TileSchema : ITileSchema
    {
        public double ProportionIgnored;
        private readonly IDictionary<string, Resolution> _resolutions;

        public TileSchema()
        {
            ProportionIgnored = 0.0001;
            _resolutions = new Dictionary<string, Resolution>();
            YAxis = YAxis.TMS;
            OriginY = Double.NaN;
            OriginX = Double.NaN;
        }

        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public string Name { get; set; }
        public string Srs { get; set; }
        public Extent Wgs84BoundingBox { get; set; }
        public string Format { get; set; }
        public Extent Extent { get; set; }
        public YAxis YAxis { get; set; }

        public IDictionary<string, Resolution> Resolutions
        {
            get { return _resolutions; }
        }

        public double GetOriginX(string levelId)
        {
            return OriginX;
        }

        public double GetOriginY(string levelId)
        {
            return OriginY;
        }

        public int GetTileWidth(string levelId)
        {
            return Resolutions[levelId].TileWidth;
        }

        public int GetTileHeight(string levelId)
        {
            return Resolutions[levelId].TileHeight;
        }

        public long GetMatrixWidth(string levelId)
        {
            return GetMatrixLastCol(levelId) - GetMatrixFirstCol(levelId) + 1;
        }

        public long GetMatrixHeight(string levelId)
        {
            return GetMatrixLastRow(levelId) - GetMatrixFirstRow(levelId) + 1;
        }

        public int GetMatrixFirstCol(string levelId)
        {
            return (int)Math.Floor(((GetFirstXRelativeToOrigin(Extent, OriginX) / Resolutions[levelId].UnitsPerPixel) / GetTileWidth(levelId)));
        }

        public int GetMatrixFirstRow(string levelId)
        {
            return (int)Math.Floor((GetFirstYRelativeToOrigin(YAxis, Extent, OriginY) / Resolutions[levelId].UnitsPerPixel) / GetTileHeight(levelId));
        }

        /// <summary>
        /// Returns a List of TileInfos that cover the provided extent. 
        /// </summary>
        public IEnumerable<TileInfo> GetTileInfos(Extent extent, double unitsPerPixel)
        {
            var level = Utilities.GetNearestLevel(Resolutions, unitsPerPixel);
            return GetTileInfos(extent, level);
        }

        public IEnumerable<TileInfo> GetTileInfos(Extent extent, string levelId)
        {
            return GetTileInfos(this, extent, levelId);
        }

        public Extent GetExtentOfTilesInView(Extent extent, string levelId)
        {
            return GetExtentOfTilesInView(this, extent, levelId);
        }

        private int GetMatrixLastCol(string levelId)
        {
            return (int)Math.Floor(((GetLastXRelativeToOrigin(Extent, OriginX)) / Resolutions[levelId].UnitsPerPixel) / GetTileWidth(levelId) - ProportionIgnored);
        }

        private int GetMatrixLastRow(string levelId)
        {
            return (int)Math.Floor((GetLastYRelativeToOrigin(YAxis, Extent, OriginY) / Resolutions[levelId].UnitsPerPixel) / GetTileHeight(levelId) - ProportionIgnored);
        }

        private static double GetLastXRelativeToOrigin(Extent extent, double originX)
        {
            return extent.MaxX - originX;
        }

        private static double GetLastYRelativeToOrigin(YAxis yAxis, Extent extent, double originY)
        {
            return yAxis == YAxis.TMS ? extent.MaxY - originY : -extent.MinY + originY;
        }

        private static double GetFirstXRelativeToOrigin(Extent extent, double originX)
        {
            return extent.MinX - originX;
        }

        private static double GetFirstYRelativeToOrigin(YAxis yAxis, Extent extent, double originY)
        {
            return (yAxis == YAxis.TMS) ? extent.MinY - originY : -extent.MaxY + originY;
        }

        internal static IEnumerable<TileInfo> GetTileInfos(ITileSchema schema, Extent extent, string levelId)
        {
            // todo: move this method elsewhere.
            var range = TileTransform.WorldToTile(extent, levelId, schema);

            // todo: use a method to get tilerange for full schema and intersect with requested tilerange.
            var startX = Math.Max(range.FirstCol, schema.GetMatrixFirstCol(levelId));
            var stopX = Math.Min(range.FirstCol + range.ColCount, schema.GetMatrixFirstCol(levelId) + schema.GetMatrixWidth(levelId));
            var startY = Math.Max(range.FirstRow, schema.GetMatrixFirstRow(levelId));
            var stopY = Math.Min(range.FirstRow + range.RowCount, schema.GetMatrixFirstRow(levelId) + schema.GetMatrixHeight(levelId));

            for (var x = startX; x < stopX; x++)
            {
                for (var y = startY; y < stopY; y++)
                {
                    yield return new TileInfo
                    {
                        Extent = TileTransform.TileToWorld(new TileRange(x, y), levelId, schema),
                        Index = new TileIndex(x, y, levelId)
                    };
                }
            }
        }

        public static Extent GetExtentOfTilesInView(ITileSchema schema, Extent extent, string levelId)
        {
            var range = TileTransform.WorldToTile(extent, levelId, schema);
            return TileTransform.TileToWorld(range, levelId, schema);
        }

        /// <summary>
        /// Checks if the TileSchema members are properly initialized and throws an exception if not.
        /// </summary>
        public void Validate()
        {
            if (String.IsNullOrEmpty(Srs))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                    "The SRS was not set for TileSchema '{0}'", Name));
            }

            if (Extent == new Extent())
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                    "The BoundingBox was not set for TileSchema '{0}'", Name));
            }

            if (Double.IsNaN(OriginX))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                    "TileSchema {0} OriginX was 'not a number', perhaps it was not initialized.", Name));
            }

            if (Double.IsNaN(OriginY))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                    "TileSchema {0} OriginY was 'not a number', perhaps it was not initialized.",Name));
            }

            if (Resolutions.Count == 0)
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                    "No Resolutions were added for TileSchema '{0}'", Name));
            }

            if (String.IsNullOrEmpty(Format))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                    "The Format was not set for TileSchema '{0}'", Name));
            }
        }
    }
}