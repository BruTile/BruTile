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

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;

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

        #endregion

        private WmscTileSource(ITileSchema tileSchema, ITileProvider tileProvider)
        {
            _tileSchema = tileSchema;
            _tileProvider = tileProvider;
        }

        public static List<ITileSource> TileSourceBuilder(Uri uri, WebProxy proxy)
        {
            var wmsCapabilities = new WmsCapabilities(uri, proxy);
            return ParseVendorSpecificCapabilitiesNode(wmsCapabilities.VendorSpecificCapabilities, wmsCapabilities.GetMapRequests[0].OnlineResource);
        }

        /// <summary>
        /// Parses the TileSets from the VendorSpecificCapabilities node of the WMS Capabilties 
        /// and adds them to the TileSets member
        /// </summary>
        /// <param name="xnlVendorSpecificCapabilities">The VendorSpecificCapabilities node of the Capabilties</param>
        /// <param name="onlineResource"></param>
        /// 
        private static List<ITileSource> ParseVendorSpecificCapabilitiesNode(
            XmlNode xnlVendorSpecificCapabilities, string onlineResource)
        {
            var tileSets = new List<ITileSource>();
            var nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("sm", "");

            XmlNodeList xnlTileSets = xnlVendorSpecificCapabilities.SelectNodes("sm:TileSet", nsmgr);

            if (xnlTileSets != null)
            {
                foreach (XmlNode xnlTileSet in xnlTileSets)
                {
                    ITileSource tileSource = ParseTileSetNode(xnlTileSet, nsmgr, onlineResource);
                    tileSets.Add(tileSource);
                }
            }
            return tileSets;
        }

        private static ITileSource ParseTileSetNode(XmlNode xnlTileSet, XmlNamespaceManager nsmgr, string onlineResource)
        {
            var schema = new TileSchema();
            var layers = new List<string>();
            var styles = new List<string>();

            XmlNode xnStyles = xnlTileSet.SelectSingleNode("sm:Styles", nsmgr);
            if (xnStyles != null)
                styles.AddRange(xnStyles.InnerText.Split(new char[] { ',' }));

            XmlNode xnLayers = xnlTileSet.SelectSingleNode("sm:Layers", nsmgr);
            if (xnLayers != null)
                layers.AddRange(xnLayers.InnerText.Split(new char[] { ',' }));

            schema.Name = CreateDefaultName(layers);

            XmlNode xnSRS = xnlTileSet.SelectSingleNode("sm:SRS", nsmgr);
            if (xnSRS != null)
                schema.Srs = xnSRS.InnerText;

            XmlNode xnWidth = xnlTileSet.SelectSingleNode("sm:Width", nsmgr);
            if (xnWidth != null)
            {
                int width;
                if (!Int32.TryParse(xnWidth.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out width))
                    throw new ArgumentException("Invalid width on tileset '" + schema.Name + "'");
                schema.Width = width;
            }

            XmlNode xnHeight = xnlTileSet.SelectSingleNode("sm:Height", nsmgr);
            if (xnHeight != null)
            {
                int height;
                if (!Int32.TryParse(xnWidth.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out height))
                    throw new ArgumentException("Invalid width on tileset '" + schema.Name + "'");
                schema.Height = height;
            }

            XmlNode xnFormat = xnlTileSet.SelectSingleNode("sm:Format", nsmgr);
            if (xnFormat != null)
                schema.Format = xnFormat.InnerText;

            XmlNode xnBoundingBox = xnlTileSet.SelectSingleNode("sm:BoundingBox", nsmgr);
            if (xnBoundingBox != null)
            {
                double minx = 0;
                double miny = 0;
                double maxx = 0;
                double maxy = 0;

                if (!double.TryParse(xnBoundingBox.Attributes["minx"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out minx) &
                    !double.TryParse(xnBoundingBox.Attributes["miny"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out miny) &
                    !double.TryParse(xnBoundingBox.Attributes["maxx"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out maxx) &
                    !double.TryParse(xnBoundingBox.Attributes["maxy"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out maxy))
                {
                    throw new ArgumentException("Invalid LatLonBoundingBox on tileset '" + schema.Name + "'");
                }

                schema.Extent = new Extent(minx, miny, maxx, maxy);

                //In WMS-C the origin is defined as the lower left corner of the boundingbox
                schema.OriginX = minx;
                schema.OriginY = miny;
            }

            XmlNode xnResolutions = xnlTileSet.SelectSingleNode("sm:Resolutions", nsmgr);
            if (xnResolutions != null)
            {
                var count = 0;
                string[] resolutions = xnResolutions.InnerText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string resolutionStr in resolutions)
                {
                    double resolution;
                    if (!Double.TryParse(resolutionStr, NumberStyles.Any, CultureInfo.InvariantCulture, out resolution))
                        throw new ArgumentException("Invalid resolution on tileset '" + schema.Name + "'");
                    schema.Resolutions.Add(new Resolution { Id = count++, UnitsPerPixel = resolution });
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

        #endregion
    }
}
