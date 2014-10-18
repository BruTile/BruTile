using System.Collections.Generic;

namespace BruTile.Predefined
{
    public class GlobalSphericalMercator : TileSchema
    {
        private const double ScaleFactor = 78271.51696401953125;
        private const string DefaultFormat = "png"; 
        private const int DefaultMinZoomLevel = 0; 
        private const int DefaultMaxZoomLevel = 19;

        // The default for AxisDirection is AxisDirection.OSM for all constructors
        
        public GlobalSphericalMercator(string format = DefaultFormat, AxisDirection axis = AxisDirection.OSM, int minZoomLevel = DefaultMinZoomLevel, int maxZoomLevel = DefaultMaxZoomLevel, string name = null) :
            this(ToResolutions(minZoomLevel, maxZoomLevel), format, axis, name)
        {
        }

        public GlobalSphericalMercator(AxisDirection axis = AxisDirection.OSM, int minZoomLevel = DefaultMinZoomLevel, int maxZoomLevel = DefaultMaxZoomLevel, string name = null) :
            this(ToResolutions(minZoomLevel, maxZoomLevel), DefaultFormat, axis, name)
        {
        }

        public GlobalSphericalMercator(int minZoomLevel = DefaultMinZoomLevel, int maxZoomLevel = DefaultMaxZoomLevel, string name = null) :
            this(ToResolutions(minZoomLevel, maxZoomLevel), DefaultFormat, AxisDirection.OSM, name)
        {
        }

        public GlobalSphericalMercator() :
            this(ToResolutions(DefaultMinZoomLevel, DefaultMaxZoomLevel))
        {
        }

        public GlobalSphericalMercator(string format = DefaultFormat, AxisDirection axis = AxisDirection.OSM, IEnumerable<int> zoomLevels = null, string name = null) :
            this(ToResolutions(zoomLevels), format, axis, name)
        {
        }

        internal GlobalSphericalMercator(IEnumerable<KeyValuePair<string, Resolution>> resolutions, string format = DefaultFormat,
                                         AxisDirection axis = AxisDirection.OSM, string name = null)
        {
            Name = name ?? "GlobalSphericalMercator";
            Format = format;
            Axis = axis;
            Srs = "EPSG:3857";
            Height = 256;
            Width = 256;

            foreach (var resolution in resolutions)
            {
                Resolutions[resolution.Key] = resolution.Value;
            }

            OriginX = -ScaleFactor * Width;
            OriginY = -ScaleFactor * Height;

            Extent = new Extent(OriginX, OriginY, -OriginX, -OriginY);

            if (axis == AxisDirection.OSM) OriginY = -OriginY; // OSM has an inverted Y-axis
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
