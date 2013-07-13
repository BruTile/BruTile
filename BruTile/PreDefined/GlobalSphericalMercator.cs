using System.Collections.Generic;
using System.Linq;

namespace BruTile.Predefined
{
    public class GlobalSphericalMercator : TileSchema
    {
        private const double ScaleFactor = 78271.516;
        private const string DefaultFormat = "png"; 
        private const bool DefaultInvertedYAxis = true;
        private const int DefaultMinZoomLevel = 0; 
        private const int DefaultMaxZoomLevel = 19;

        public GlobalSphericalMercator(string format = DefaultFormat, bool invertedYAxis = DefaultInvertedYAxis, int minZoomLevel = DefaultMinZoomLevel, int maxZoomLevel = DefaultMaxZoomLevel, string name = null) :
            this(ToResolutions(minZoomLevel, maxZoomLevel), format, invertedYAxis, name)
        {
        }

        public GlobalSphericalMercator(bool invertedYAxis = DefaultInvertedYAxis, int minZoomLevel = DefaultMinZoomLevel, int maxZoomLevel = DefaultMaxZoomLevel, string name = null) :
            this(ToResolutions(minZoomLevel, maxZoomLevel), DefaultFormat, invertedYAxis, name)
        {
        }

        public GlobalSphericalMercator(int minZoomLevel = DefaultMinZoomLevel, int maxZoomLevel = DefaultMaxZoomLevel, string name = null) :
            this(ToResolutions(minZoomLevel, maxZoomLevel), DefaultFormat, DefaultInvertedYAxis, name)
        {
        }

        public GlobalSphericalMercator():
            this(ToResolutions(DefaultMinZoomLevel, DefaultMaxZoomLevel))
        {
        }

        public GlobalSphericalMercator(string format = DefaultFormat, bool invertedYAxis = DefaultInvertedYAxis, IEnumerable<int> zoomLevels = null, string name = null) :
            this(ToResolutions(zoomLevels), format, invertedYAxis, name)
        {
        }

        internal GlobalSphericalMercator(IEnumerable<KeyValuePair<int, Resolution>> resolutions, string format = DefaultFormat,
                                         bool invertedYAxis = DefaultInvertedYAxis, string name = null)
        {
            Name = name ?? "GlobalSphericalMercator";
            Format = format;
            Axis = invertedYAxis ? AxisDirection.InvertedY : AxisDirection.Normal;
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

            if (invertedYAxis) OriginY = -OriginY;
        }

        private static IEnumerable<KeyValuePair<int, Resolution>> ToResolutions(int min, int max)
        {
            var list = new List<int>();
            for (var i = min; i <= max; i++) list.Add(i);
            return ToResolutions(list);
        }

        private static IEnumerable<KeyValuePair<int, Resolution>> ToResolutions(IEnumerable<int> levels)
        {
            var dictionary = new Dictionary<int, Resolution>();
            foreach (var level in levels)
            {
                dictionary[level] = new Resolution
                    {
                        Id = level.ToString(), 
                        UnitsPerPixel = 2 * ScaleFactor / (1 << level)
                    };
            }
            return dictionary;
        }
    }
}
