﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;

[assembly: CLSCompliant(true)]

namespace BruTile
{
    /// <summary>
    /// Enumeration of possible axis directions
    /// </summary>
    /// <remarks>
    /// Direction is relative to the coordinate system in which the map is presented.
    /// <para/>
    /// InvertedX and InvertedXY do not exist yet, and may never.
    /// </remarks>
    public enum AxisDirection
    {
        /// <summary>
        /// The axis direction of the tiles match that of the map. This is used by TMS.
        /// </summary>
        Normal,

        /// <summary>
        /// The y-axis direction is inverted compared to that of the map. This is used by OpenStreetMap
        /// </summary>
        InvertedY
    }

    public class TileSchema : ITileSchema
    {
        private readonly IDictionary<string, Resolution> _resolutions;

        public TileSchema()
        {
            _resolutions = new Dictionary<string, Resolution>();
            Axis = AxisDirection.Normal;
            OriginY = Double.NaN;
            OriginX = Double.NaN;
        }

        public string Name { get; set; }
        public string Srs { get; set; }
        public string Format { get; set; }
        public Extent Extent { get; set; }
        public AxisDirection Axis { get; set; }
        public IDictionary<string, Resolution> Resolutions
        {
            get { return _resolutions; }

        }

        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

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
            return Width;
        }

        public int GetTileHeight(string levelId)
        {
            return Height;
        }

        public int GetMatrixWidth(string levelId)
        {
            return (int)((Extent.Width / Resolutions[levelId].UnitsPerPixel) / GetTileWidth(levelId));
        }

        public int GetMatrixHeight(string levelId)
        {
            return (int)((Extent.Height / Resolutions[levelId].UnitsPerPixel) / GetTileHeight(levelId));
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
            return GetTilesInView(this, extent, levelId);
        }

        internal static IEnumerable<TileInfo> GetTilesInView(ITileSchema schema, Extent extent, string levelId)
        {
            var range = TileTransform.WorldToTile(extent, levelId, schema);

            for (var x = range.FirstCol; x < range.FirstCol + range.ColCount; x++)
            {
                for (var y = range.FirstRow; y < range.FirstRow + range.RowCount; y++)
                {
                    if (x < 0 || x >= schema.GetMatrixWidth(levelId)) continue;
                    if (y < 0 || y >= schema.GetMatrixHeight(levelId)) continue;
                    
                     var info = new TileInfo
                    {
                        Extent = TileTransform.TileToWorld(new TileRange(x, y), levelId, schema),
                        Index = new TileIndex(x, y, levelId)
                    };

                    yield return info;
                }
            }
        }

        public Extent GetExtentOfTilesInView(Extent extent, string levelId)
        {
            return GetExtentOfTilesInView(this, extent, levelId);
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
                                                            "TileSchema {0} OriginX was 'not a number', perhaps it was not initialized.",
                                                            Name));
            }
            if (Double.IsNaN(OriginY))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                                                            "TileSchema {0} OriginY was 'not a number', perhaps it was not initialized.",
                                                            Name));
            }
            if (Resolutions.Count == 0)
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                                                            "No Resolutions were added for TileSchema '{0}'", Name));
            }
            if (Width == 0)
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                                                            "The Width was not set for TileSchema '{0}'", Name));
            }
            if (Height == 0)
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                                                            "The Height was not set for TileSchema '{0}'", Name));
            }
            if (String.IsNullOrEmpty(Format))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                                                            "The Format was not set for TileSchema '{0}'", Name));
            }

            // TODO: BoundingBox should contain a SRS, and we should check if BoundingBox.Srs is the same
            // as TileSchema Srs because we do not project one to the other. 
        }

        public static bool WithinSchemaExtent(Extent schemaExtent, Extent tileExtent)
        {
            // Always return false when the tile is outsize of the schema
            if (!tileExtent.Intersects(schemaExtent)) return false;

            // Do not always accept when the tile is partially inside the schema. 
            // Reject tiles that have less than 0.1% percent overlap.
            // In practice they turn out to be mostly false positives due to rounding errors.
            // They are not present on the server and the failed requests slow the application down.
            return ((tileExtent.Intersect(schemaExtent).Area/tileExtent.Area) > 0.001);
        }
    }
}