// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BruTile.Web;
using BruTile.Web.Wms;

namespace BruTile.Wmsc
{
    /// <summary>
    /// This class has not been tested.
    /// </summary>
    public class WmscTileSource : TileSource
    {
        #region Fields

        #endregion Fields

        private WmscTileSource(ITileSchema tileSchema, ITileProvider tileProvider)
            :base(tileProvider, tileSchema)
        {
        }

        public static IEnumerable<ITileSource> CreateFromWmscCapabilties(Uri uri)
        {
            var wmsCapabilities = new WmsCapabilities(uri.ToString());

            return ParseVendorSpecificCapabilitiesNode(
                (XElement)wmsCapabilities.Capability.ExtendedCapabilities[XName.Get("VendorSpecificCapabilities")],
                wmsCapabilities.Capability.Request.GetCapabilities.DCPType[0].Http.Get.OnlineResource);
        }

        public static IEnumerable<ITileSource> CreateFromWmscCapabilties(XDocument document)
        {
            var wmsCapabilities = new WmsCapabilities(document);

            if (!wmsCapabilities.Capability.ExtendedCapabilities.ContainsKey(XName.Get("VendorSpecificCapabilities")))
                throw new System.Exception("VendorSpecificCapabilities node is missing from WMS capabilities (This is probably an ordinary WMS)");

            var vendorSpecificNode = (XElement)wmsCapabilities.Capability.ExtendedCapabilities[XName.Get("VendorSpecificCapabilities")];

            return ParseVendorSpecificCapabilitiesNode(vendorSpecificNode,
                wmsCapabilities.Capability.Request.GetCapabilities.DCPType[0].Http.Get.OnlineResource);
        }

        /// <remarks> WMS-C uses the VendorSpecificCapabilities to specify the tile schema.</remarks>
        private static IEnumerable<ITileSource> ParseVendorSpecificCapabilitiesNode(
            XElement xVendorSpecificCapabilities, OnlineResource onlineResource)
        {
            var xTileSets = xVendorSpecificCapabilities.Elements(XName.Get("TileSet"));
            return xTileSets.Select(tileSet => ParseTileSetNode(tileSet, onlineResource)).ToList();
        }

        private static ITileSource ParseTileSetNode(XElement xTileSet, OnlineResource onlineResource)
        {
            var styles = xTileSet.Elements("Styles").Select(xStyle => xStyle.Value).ToList();
            var layers = xTileSet.Elements("Layers").Select(xLayer => xLayer.Value).ToList();
            var schema = ToTileSchema(xTileSet, CreateDefaultName(layers));
            var wmscRequest = new WmscRequest(new Uri(onlineResource.Href), schema, layers, styles);
            return new WmscTileSource(schema, new WebTileProvider(wmscRequest));
        }

        private static TileSchema ToTileSchema(XElement xTileSet, string name)
        {
            var schema = new TileSchema { Name = name };

            var xSrs = xTileSet.Element("SRS");
            if (xSrs != null)
                schema.Srs = xSrs.Value;

            var xWidth = xTileSet.Element("Width");
            if (xWidth != null)
            {
                int width;
                if (!Int32.TryParse(xWidth.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out width))
                    throw new ArgumentException("Invalid width on tileset '" + schema.Name + "'");
                schema.Width = width;
            }

            var xHeight = xTileSet.Element("Height");
            if (xHeight != null)
            {
                int height;
                if (!Int32.TryParse(xHeight.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out height))
                    throw new ArgumentException("Invalid width on tileset '" + schema.Name + "'");
                schema.Height = height;
            }

            var xFormat = xTileSet.Element("Format");
            if (xFormat != null)
                schema.Format = xFormat.Value;

            var xBoundingBox = xTileSet.Element("BoundingBox");
            if (xBoundingBox != null)
            {
                double minx, miny, maxx, maxy;
                if (
                    !double.TryParse(xBoundingBox.Attribute("minx").Value, NumberStyles.Any, CultureInfo.InvariantCulture,
                                     out minx) &
                    !double.TryParse(xBoundingBox.Attribute("miny").Value, NumberStyles.Any, CultureInfo.InvariantCulture,
                                     out miny) &
                    !double.TryParse(xBoundingBox.Attribute("maxx").Value, NumberStyles.Any, CultureInfo.InvariantCulture,
                                     out maxx) &
                    !double.TryParse(xBoundingBox.Attribute("maxy").Value, NumberStyles.Any, CultureInfo.InvariantCulture,
                                     out maxy))
                {
                    throw new ArgumentException("Invalid LatLonBoundingBox on tileset '" + schema.Name + "'");
                }

                schema.Extent = new Extent(minx, miny, maxx, maxy);

                //In WMS-C the origin is defined as the lower left corner of the boundingbox
                schema.OriginX = minx;
                schema.OriginY = miny;
            }

            var xResolutions = xTileSet.Element("Resolutions");
            if (xResolutions != null)
            {
                var count = 0;
                string[] resolutions = xResolutions.Value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var resolutionStr in resolutions)
                {
                    double resolution;
                    if (!Double.TryParse(resolutionStr, NumberStyles.Any, CultureInfo.InvariantCulture, out resolution))
                        throw new ArgumentException("Invalid resolution on tileset '" + schema.Name + "'");
                    var levelId = count.ToString(CultureInfo.InvariantCulture);
                    schema.Resolutions[levelId] = new Resolution {Id = levelId, UnitsPerPixel = resolution};
                    count++;
                }
            }
            return schema;
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
    }
}