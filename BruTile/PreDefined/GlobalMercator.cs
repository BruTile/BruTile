// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System.Collections.Generic;

namespace BruTile.Predefined
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
        public GlobalMercator()
        {}

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

            var count = 0;
            foreach (var resolution in resolutions)
            {
                Resolutions[count.ToString()] = resolution;
                count++;
            }

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