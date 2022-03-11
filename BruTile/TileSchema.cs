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
        private readonly IDictionary<int, Resolution> _resolutions;

        public TileSchema()
        {
            ProportionIgnored = 0.0001;
            _resolutions = new Dictionary<int, Resolution>();
            YAxis = YAxis.TMS;
            OriginY = double.NaN;
            OriginX = double.NaN;
        }

        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public string Name { get; set; }
        public string Srs { get; set; }
        public Extent Wgs84BoundingBox { get; set; }
        public string Format { get; set; }
        public Extent Extent { get; set; }
        public YAxis YAxis { get; set; }

        public IDictionary<int, Resolution> Resolutions
        {
            get { return _resolutions; }
        }

        public double GetOriginX(int level)
        {
            return OriginX;
        }

        public double GetOriginY(int level)
        {
            return OriginY;
        }

        public int GetTileWidth(int level)
        {
            return Resolutions[level].TileWidth;
        }

        public int GetTileHeight(int level)
        {
            return Resolutions[level].TileHeight;
        }

        public long GetMatrixWidth(int level)
        {
            return GetMatrixLastCol(level) - GetMatrixFirstCol(level) + 1;
        }

        public long GetMatrixHeight(int level)
        {
            return GetMatrixLastRow(level) - GetMatrixFirstRow(level) + 1;
        }

        public int GetMatrixFirstCol(int level)
        {
            return (int)Math.Floor(((GetFirstXRelativeToOrigin(Extent, OriginX) / Resolutions[level].UnitsPerPixel) / GetTileWidth(level)));
        }

        public int GetMatrixFirstRow(int level)
        {
            return (int)Math.Floor((GetFirstYRelativeToOrigin(YAxis, Extent, OriginY) / Resolutions[level].UnitsPerPixel) / GetTileHeight(level));
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
            return GetTileInfos(this, extent, level);
        }

        public Extent GetExtentOfTilesInView(Extent extent, int level)
        {
            return GetExtentOfTilesInView(this, extent, level);
        }

        private int GetMatrixLastCol(int level)
        {
            return (int)Math.Floor(((GetLastXRelativeToOrigin(Extent, OriginX)) / Resolutions[level].UnitsPerPixel) / GetTileWidth(level) - ProportionIgnored);
        }

        private int GetMatrixLastRow(int level)
        {
            return (int)Math.Floor((GetLastYRelativeToOrigin(YAxis, Extent, OriginY) / Resolutions[level].UnitsPerPixel) / GetTileHeight(level) - ProportionIgnored);
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

        internal static IEnumerable<TileInfo> GetTileInfos(ITileSchema schema, Extent extent, int level)
        {
            // Todo: Move this method elsewhere.
            var range = TileTransform.WorldToTile(extent, level, schema);

            // Todo: Use a method to get TileRange for full schema and intersect with requested TileRange.
            var startX = Math.Max(range.FirstCol, schema.GetMatrixFirstCol(level));
            var stopX = Math.Min(range.FirstCol + range.ColCount, schema.GetMatrixFirstCol(level) + schema.GetMatrixWidth(level));
            var startY = Math.Max(range.FirstRow, schema.GetMatrixFirstRow(level));
            var stopY = Math.Min(range.FirstRow + range.RowCount, schema.GetMatrixFirstRow(level) + schema.GetMatrixHeight(level));

            for (var x = startX; x < stopX; x++)
            {
                for (var y = startY; y < stopY; y++)
                {
                    yield return new TileInfo
                    {
                        Extent = TileTransform.TileToWorld(new TileRange(x, y), level, schema),
                        Index = new TileIndex(x, y, level)
                    };
                }
            }
        }

        public static Extent GetExtentOfTilesInView(ITileSchema schema, Extent extent, int level)
        {
            var range = TileTransform.WorldToTile(extent, level, schema);
            return TileTransform.TileToWorld(range, level, schema);
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