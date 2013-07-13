// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using BruTile.Web.Wms;

namespace BruTile.Web
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

            return ParseVendorSpecificCapabilitiesNode(
                (XElement)wmsCapabilities.Capability.ExtendedCapabilities[XName.Get("VendorSpecificCapabilities")],
                wmsCapabilities.Capability.Request.GetCapabilities.DCPType[0].Http.Get.OnlineResource);
        }

        /// <remarks> WMS-C uses the VendorSpecificCapabilities to specify the tile schema.</remarks>
        private static IEnumerable<ITileSource> ParseVendorSpecificCapabilitiesNode(
            XElement xnlVendorSpecificCapabilities, OnlineResource onlineResource)
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

        private static ITileSource ParseTileSetNode(XElement xnlTileSet, OnlineResource onlineResource)
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
                    schema.Resolutions[count] = new Resolution { Id = count.ToString(), UnitsPerPixel = resolution };
                    count++;
                }
            }

            return new WmscTileSource(schema, new WebTileProvider(new WmscRequest(new Uri(onlineResource.Href), schema, layers, styles, new Dictionary<string, string>())));
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