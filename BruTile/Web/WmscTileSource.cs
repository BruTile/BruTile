#region License

// Copyright 2009 - Paul den Dulk (Geodan)
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
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

#endregion License

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BruTile.Web
{
    /// <summary>
    /// This class has not been tested.
    /// </summary>
    public class WmscTileSource : ITileSource
    {
        #region Fields

        readonly ITileSchema _tileSchema;
        readonly ITileProvider _tileProvider;

        #endregion Fields

        private WmscTileSource(ITileSchema tileSchema, ITileProvider tileProvider)
        {
            _tileSchema = tileSchema;
            _tileProvider = tileProvider;
        }

#if !SILVERLIGHT

        //public static List<ITileSource> TileSourceBuilder(Uri uri, WebProxy proxy)
        //{
        //    //var s = new DataContractSerializer(typeof (WmsCapabilities));
        //    //var wms = (WmsCapabilities)s.ReadObject()
        //    //WmsCapabilities =System.Runtime.Serialization.DataContractSerializer
        //    //var wmsCapabilities = new WmsCapabilities(uri, proxy);

        //    //return ParseVendorSpecificCapabilitiesNode(wmsCapabilities.VendorSpecificCapabilities, wmsCapabilities.Capability.Request.[0].OnlineResource);
        //}

#else
        //public static List<ITileSource> TileSourceBuilder(Uri uri)
        //{
        //    var wmsCapabilities = new WmsCapabilities(uri);
        //    return ParseVendorSpecificCapabilitiesNode(wmsCapabilities.VendorSpecificCapabilities, wmsCapabilities.GetMapRequests[0].OnlineResource);
        //}
#endif

        /// <summary>
        /// Parses the TileSets from the VendorSpecificCapabilities node of the WMS Capabilties
        /// and adds them to the TileSets member
        /// </summary>
        /// <param name="xnlVendorSpecificCapabilities">The VendorSpecificCapabilities node of the Capabilties</param>
        /// <param name="onlineResource"></param>
        ///
        private static List<ITileSource> ParseVendorSpecificCapabilitiesNode(
            XElement xnlVendorSpecificCapabilities, string onlineResource)
        {
            var tileSets = new List<ITileSource>();

            var xnlTileSets = xnlVendorSpecificCapabilities.Elements(XName.Get("TileSet"));

            if (xnlTileSets != null)
            {
                foreach (var xnlTileSet in xnlTileSets)
                {
                    ITileSource tileSource = ParseTileSetNode(xnlTileSet, onlineResource);
                    tileSets.Add(tileSource);
                }
            }
            return tileSets;
        }

        private static ITileSource ParseTileSetNode(XElement xnlTileSet, string onlineResource)
        {
            var schema = new TileSchema();

            var xnStyles = xnlTileSet.Elements("Styles");
            var styles = xnStyles.Select(xnStyle => xnStyle.Value).ToList();

            var xnLayers = xnlTileSet.Elements("Layers");
            var layers = xnLayers.Select(xnLayer => xnLayer.Value).ToList();

            schema.Name = CreateDefaultName(layers);

            var xnSrs = xnlTileSet.Element("SRS");
            if (xnSrs != null)
                schema.Srs = xnSrs.Value;

            var xnWidth = xnlTileSet.Element("Width");
            if (xnWidth != null)
            {
                int width;
                if (!Int32.TryParse(xnWidth.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out width))
                    throw new ArgumentException("Invalid width on tileset '" + schema.Name + "'");
                schema.Width = width;
            }

            var xnHeight = xnlTileSet.Element("Height");
            if (xnHeight != null)
            {
                int height;
                if (!Int32.TryParse(xnHeight.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out height))
                    throw new ArgumentException("Invalid width on tileset '" + schema.Name + "'");
                schema.Height = height;
            }

            var xnFormat = xnlTileSet.Element("Format");
            if (xnFormat != null)
                schema.Format = xnFormat.Value;

            var xnBoundingBox = xnlTileSet.Element("BoundingBox");
            if (xnBoundingBox != null)
            {
                double minx, miny, maxx, maxy;
                if (!double.TryParse(xnBoundingBox.Attribute("minx").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out minx) &
                    !double.TryParse(xnBoundingBox.Attribute("miny").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out miny) &
                    !double.TryParse(xnBoundingBox.Attribute("maxx").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out maxx) &
                    !double.TryParse(xnBoundingBox.Attribute("maxy").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out maxy))
                {
                    throw new ArgumentException("Invalid LatLonBoundingBox on tileset '" + schema.Name + "'");
                }

                schema.Extent = new Extent(minx, miny, maxx, maxy);

                //In WMS-C the origin is defined as the lower left corner of the boundingbox
                schema.OriginX = minx;
                schema.OriginY = miny;
            }

            var xnResolutions = xnlTileSet.Element("Resolutions");
            if (xnResolutions != null)
            {
                var count = 0;
                string[] resolutions = xnResolutions.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var resolutionStr in resolutions)
                {
                    double resolution;
                    if (!Double.TryParse(resolutionStr, NumberStyles.Any, CultureInfo.InvariantCulture, out resolution))
                        throw new ArgumentException("Invalid resolution on tileset '" + schema.Name + "'");
                    schema.Resolutions.Add(new Resolution { Id = count.ToString(), UnitsPerPixel = resolution });
                    count++;
                }
            }

            return new WmscTileSource(schema, new WebTileProvider(new WmscRequest(new Uri(onlineResource), schema, layers, styles, new Dictionary<string, string>())));
        }

        private static string CreateDefaultName(IEnumerable<string> layers)
        {
            var stringBuilder = new StringBuilder();
            foreach (string layer in layers)
            {
                stringBuilder.Append(layer + ",");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }

        #region ITileSource Members

        public ITileProvider Provider
        {
            get { return _tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return _tileSchema; }
        }

        #endregion ITileSource Members
    }
}