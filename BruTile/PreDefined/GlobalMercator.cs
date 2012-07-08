#region License

// Copyright 2011 - Felix Obermaier (www.ivv-aachen.de)
//
// This file is part of BruTile.
// BruTile is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// BruTile is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

using System.Collections.Generic;

namespace BruTile.PreDefined
{
    /// <summary>
    /// Tile schema for Global Mercator 
    /// </summary>
    /// <seealso href="http://wiki.osgeo.org/wiki/Tile_Map_Service_Specification#global-mercator"/>
    public class GlobalMercator : TileSchema
    {
        private const double ScaleFactor = 78271.516d;

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="format">The image format of the tiles</param>
        public GlobalMercator(string format)
            :this(format, 20)
        {
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="format">The image format of the tiles</param>
        /// <param name="numResolution">The number of resolutions to create</param>
        public GlobalMercator(string format, int numResolution)
            :this(format, ToResolutions(0, numResolution-1))
        {
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="format">The image format of the tiles</param>
        /// <param name="minZoomLevel">The minimum zoom level</param>
        /// <param name="maxZoomLevel">The maximum zoom level</param>
        public GlobalMercator(string format, int minZoomLevel, int maxZoomLevel)
            :this(format, ToResolutions(minZoomLevel, maxZoomLevel))
        {
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="format">The image format of the tiles</param>
        /// <param name="declaredZoomLevels">The declared zoomlevels</param>
        public GlobalMercator(string format, IList<int> declaredZoomLevels)
            :this(format, ToResolutions(declaredZoomLevels))
        {
        }

        internal GlobalMercator(string format, IEnumerable<Resolution> resolutions)
        {
            Name = "GlobalMercator";

            Format = format;
            Axis = AxisDirection.Normal;
            Srs = "OSGEO:41001";
            Height = 256;
            Width = 256;

            foreach (var resolution in resolutions)
                Resolutions.Add(resolution);

            OriginX = -ScaleFactor*Width;
            OriginY = -ScaleFactor*Height;

            Extent = new Extent(OriginX, OriginY, -OriginX, -OriginY);
        }

        private static IEnumerable<Resolution> ToResolutions(int min, int max)
        {
            var results = new List<Resolution>(max-min+1);
            for (var i = min; i <= max; i++ )
                results.Add(new Resolution
                                {
                                    Id = i.ToString(),
                                    //2 * ScaleFactor: this is a hack, since first level is made up of 4 tiles
                                    UnitsPerPixel = 2 * ScaleFactor / (1 << i)
                                });
            return results.ToArray();
        }

        private static IEnumerable<Resolution> ToResolutions(IList<int> levels)
        {
            var results = new Resolution[levels.Count];
            for (var i = 0; i < levels.Count; i++)
                results[i] = new Resolution
                                 {
                                     Id = levels[i].ToString(),
                                     //2 * ScaleFactor: this is a hack, since first level is made up of 4 tiles
                                     UnitsPerPixel = 2 * ScaleFactor / (1 << levels[i])
                                 };
            return results;
        }

        /// <summary>
        /// Gets the Well-Known-Text representation of the spatial reference system
        /// </summary>
        public string SrsWkt 
        { 
            get { return 
                "PROJCS[\"WGS84 / Simple Mercator\", "+
                    "GEOGCS[\"WGS 84\", "+
                        "DATUM[\"WGS_1984\", SPHEROID[\"WGS_1984\",6378137,298.257223563]], " +
                        "PRIMEM[\"Greenwich\",0], " +
                        "UNIT[\"Decimal_Degree\", 0.0174532925199433]], " +
                    "PROJECTION[\"Mercator_1SP\"], " +
                    "PARAMETER[\"central_meridian\",0], " +
                    "PARAMETER[\"false_easting\",0]," +
                    "PARAMETER[\"false_northing\",0]," +
                    "UNIT[\"Meter\",1]]"; }
        }
    }
}