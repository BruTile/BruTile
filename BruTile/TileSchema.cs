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

namespace BruTile
{
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
        private AxisDirection axis = AxisDirection.Normal;

        #endregion

        #region Propertiesb

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
            get { return axis; }
            set { axis = value; }
        }

        public virtual string Additions
        {
            get { return ""; }
        }

        #endregion

        #region Methods

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

        #endregion
    }


}
