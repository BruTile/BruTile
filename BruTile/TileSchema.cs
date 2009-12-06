// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Globalization;

[assembly: CLSCompliant(true)]

namespace BruTile
{
    public enum AxisDirection
    {
        //Direction is relative to the coordinate system in which the map is presented.
        Normal,
        InvertedY
        //InvertedX and InvertedXY do not exist yet, and may never.
    }

    public class TileSchema : ITileSchema
    {
        #region Fields

        private string name;
        private string srs;
        private Extent extent;
        private double originX = Double.NaN;
        private double originY = Double.NaN;
        private List<double> resolutions = new List<double>();
        private int width;
        private int height;
        private string format;
        private AxisDirection axisDirection = AxisDirection.Normal;
        IAxis axis = new AxisNormal();

        #endregion

        #region Properties

        //Todo: see if we can replace all setters with constructor arguments. Do this after automatic parser is implemented

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Srs
        {
            get { return srs; }
            set { srs = value; }
        }

        public Extent Extent
        {
            get { return extent; }
            set { extent = value; }
        }

        public double OriginX
        {
            get { return originX; }
            set { originX = value; }
        }

        public double OriginY
        {
            get { return originY; }
            set { originY = value; }
        }

        public IList<double> Resolutions
        {
            get { return resolutions; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public AxisDirection Axis
        {
            get { return axisDirection; }
            set 
            { 
                axisDirection = value;
                axis = GetAxis(value);
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the TileSchema members are properly initialized and throws an exception if not.
        /// </summary>
        public virtual void Validate()
        {
            if (String.IsNullOrEmpty(srs))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                  "The SRS was not set for TileSchema '{0}'", this.Name));
            }
            if (extent == new Extent())
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                  "The BoundingBox was not set for TileSchema '{0}'", this.Name));
            }
            if (Double.IsNaN(originX))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                  "TileSchema {0} OriginX was 'not a number', perhaps it was not initialized.", this.Name));
            }
            if (Double.IsNaN(originY))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                  "TileSchema {0} OriginY was 'not a number', perhaps it was not initialized.", this.Name));
            }
            if (resolutions.Count == 0)
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                  "No Resolutions were added for TileSchema '{0}'", this.Name));
            }
            if (width == 0)
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                  "The Width was not set for TileSchema '{0}'", this.Name));
            }
            if (height == 0)
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                  "The Height was not set for TileSchema '{0}'", this.Name));
            }
            if (String.IsNullOrEmpty(format))
            {
                throw new ValidationException(String.Format(CultureInfo.InvariantCulture,
                  "The Format was not set for TileSchema '{0}'", this.Name));
            }

            //TODO: BoundingBox should contain a SRS, and we should check if BoundingBox.Srs is the same
            //as TileSchema Srs because we do not project one to the other. 
        }

        /// <summary>
        /// Returns a List of TileInfos that cover the provided extent. 
        /// </summary>
        public IList<TileInfo> GetTilesInView(Extent extent, double resolution)
        {
            int level = Utilities.GetNearestLevel(Resolutions, resolution);
            return GetTilesInView(extent, level);
        }

        public IList<TileInfo> GetTilesInView(Extent extent, int level)
        {
            IList<TileInfo> tiles = new List<TileInfo>();
            TileRange range = axis.WorldToTile(extent, level, this);
            tiles.Clear();

            for (int x = range.FirstCol; x < range.LastCol; x++)
            {
                for (int y = range.FirstRow; y < range.LastRow; y++)
                {
                    TileInfo tile = new TileInfo();
                    tile.Extent = axis.TileToWorld(new TileRange(x, y), level, this);
                    tile.Key = new TileKey(x, y, level);

                    if (WithinSchemaExtent(Extent, tile.Extent))
                    {
                        tiles.Add(tile);
                    }
                }
            }
            return tiles;
        }

        public Extent GetExtentOfTilesInView(Extent extent, int level)
        {
            TileRange range = axis.WorldToTile(extent, level, this);
            return axis.TileToWorld(range, level, this);
        }

        #endregion

        #region Private Methods

        private static bool WithinSchemaExtent(Extent schemaExtent, Extent tileExtent)
        {
            if (!tileExtent.Intersects(schemaExtent)) return false;
            //We do not accept all tiles that intersect. We reject tiles that have five
            //percent or less overlap with the schema Extent. It turns out that in practice
            //that many tiles with a small overlap with the schema extent are not on the server.
            return ((tileExtent.Intersect(schemaExtent).Area / tileExtent.Area) > 0.05);
        }

        private static IAxis GetAxis(AxisDirection axis)
        {
            switch (axis)
            {
                case AxisDirection.Normal:
                    return new AxisNormal();
                case AxisDirection.InvertedY:
                    return new AxisInvertedY();
                default:
                    throw new ArgumentException("could not find axis transformer");
            }
        }

        #endregion


    }


}
