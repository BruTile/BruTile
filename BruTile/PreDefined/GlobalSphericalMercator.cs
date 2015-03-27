// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace BruTile.Predefined
{
    public class GlobalSphericalMercator : TileSchema
    {
        private const double ScaleFactor = 78271.51696401953125;
        private const string DefaultFormat = "png"; 
        private const int DefaultMinZoomLevel = 0; 
        private const int DefaultMaxZoomLevel = 19;

        // The default for YAxis is YAxis.OSM for all constructors
        
        public GlobalSphericalMercator(string format = DefaultFormat, YAxis yAxis = YAxis.OSM, int minZoomLevel = DefaultMinZoomLevel, int maxZoomLevel = DefaultMaxZoomLevel, string name = null) :
            this(ToResolutions(minZoomLevel, maxZoomLevel), format, yAxis, name)
        {
        }

        public GlobalSphericalMercator(YAxis yAxis = YAxis.OSM, int minZoomLevel = DefaultMinZoomLevel, int maxZoomLevel = DefaultMaxZoomLevel, string name = null) :
            this(ToResolutions(minZoomLevel, maxZoomLevel), DefaultFormat, yAxis, name)
        {
        }

        public GlobalSphericalMercator(int minZoomLevel = DefaultMinZoomLevel, int maxZoomLevel = DefaultMaxZoomLevel, string name = null) :
            this(ToResolutions(minZoomLevel, maxZoomLevel), DefaultFormat, YAxis.OSM, name)
        {
        }

        public GlobalSphericalMercator() :
            this(ToResolutions(DefaultMinZoomLevel, DefaultMaxZoomLevel))
        {
        }

        public GlobalSphericalMercator(string format = DefaultFormat, YAxis yAxis = YAxis.OSM, IEnumerable<int> zoomLevels = null, string name = null) :
            this(ToResolutions(zoomLevels), format, yAxis, name)
        {
        }

        internal GlobalSphericalMercator(IEnumerable<KeyValuePair<string, Resolution>> resolutions, string format = DefaultFormat,
                                         YAxis yAxis = YAxis.OSM, string name = null)
        {
            Name = name ?? "GlobalSphericalMercator";
            Format = format;
            YAxis = yAxis;
            Srs = "EPSG:3857";
            Height = 256;
            Width = 256;

            foreach (var resolution in resolutions)
            {
                Resolutions[resolution.Value.Id] = resolution.Value;
            }

            OriginX = -ScaleFactor * Width;
            OriginY = -ScaleFactor * Height;

            Extent = new Extent(OriginX, OriginY, -OriginX, -OriginY);

            if (yAxis == YAxis.OSM) OriginY = -OriginY; // OSM has an inverted Y-axis
        }

        private static IEnumerable<KeyValuePair<string, Resolution>> ToResolutions(int min, int max)
        {
            var list = new List<int>();
            for (var i = min; i <= max; i++) list.Add(i);
            return ToResolutions(list);
        }

        private static IEnumerable<KeyValuePair<string, Resolution>> ToResolutions(IEnumerable<int> levels)
        {
            var dictionary = new Dictionary<string, Resolution>();
            foreach (var level in levels)
            {
                dictionary[level.ToString()] = new Resolution
                    {
                        Id = level.ToString(), 
                        UnitsPerPixel = 2 * ScaleFactor / (1 << level)
                    };
            }
            return dictionary;
        }
    }
}
