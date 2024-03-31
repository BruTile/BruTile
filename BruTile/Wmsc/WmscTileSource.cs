// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BruTile.Web;
using BruTile.Wms;

namespace BruTile.Wmsc;

public class WmscTileSource : TileSource
{
    private WmscTileSource(ITileSchema tileSchema, ITileProvider tileProvider)
        : base(tileProvider, tileSchema)
    { }

    public static async Task<IEnumerable<ITileSource>> CreateFromWmscCapabilitiesAsync(Uri uri)
    {
        var wmsCapabilities = await WmsCapabilities.CreateAsync(uri);
        var cap = wmsCapabilities.Capability;
        var ec = cap.ExtendedCapabilities;
        if (!ec.TryGetValue(XName.Get("VendorSpecificCapabilities"), out var vsc))
            throw new WmsParsingException("Node 'VendorSpecificCapabilities' not found in wms capability document");

        return ParseVendorSpecificCapabilitiesNode(
            (XElement)vsc, cap.Request.GetCapabilities.DCPType[0].Http.Get.OnlineResource);
    }

    public static IEnumerable<ITileSource> CreateFromWmscCapabilities(XDocument document)
    {
        var wmsCapabilities = new WmsCapabilities(document);

        if (!wmsCapabilities.Capability.ExtendedCapabilities.ContainsKey(XName.Get("VendorSpecificCapabilities")))
            throw new System.Exception("VendorSpecificCapabilities node is missing from WMS capabilities (This is probably an ordinary WMS)");

        var vendorSpecificNode = (XElement)wmsCapabilities.Capability.ExtendedCapabilities[XName.Get("VendorSpecificCapabilities")];

        return ParseVendorSpecificCapabilitiesNode(vendorSpecificNode,
            wmsCapabilities.Capability.Request.GetCapabilities.DCPType[0].Http.Get.OnlineResource);
    }

    /// <remarks> WMS-C uses the VendorSpecificCapabilities to specify the tile schema.</remarks>
    private static List<WmscTileSource> ParseVendorSpecificCapabilitiesNode(
        XElement xVendorSpecificCapabilities, OnlineResource onlineResource)
    {
        var xTileSets = xVendorSpecificCapabilities.Elements(XName.Get("TileSet"));
        return xTileSets.Select(tileSet => ParseTileSetNode(tileSet, onlineResource)).ToList();
    }

    private static WmscTileSource ParseTileSetNode(XElement xTileSet, OnlineResource onlineResource)
    {
        var styles = xTileSet.Elements("Styles").Select(xStyle => xStyle.Value).ToList();
        var layers = xTileSet.Elements("Layers").Select(xLayer => xLayer.Value).ToList();
        var schema = ToTileSchema(xTileSet, CreateDefaultName(layers));
        var wmscRequest = new WmscRequest(new Uri(onlineResource.Href), schema, layers, styles);
        return new WmscTileSource(schema, new HttpTileProvider(wmscRequest));
    }

    private static TileSchema ToTileSchema(XElement xTileSet, string name)
    {
        var schema = new TileSchema { Name = name };

        var xSrs = xTileSet.Element("SRS");
        if (xSrs != null)
            schema.Srs = xSrs.Value;

        var xWidth = xTileSet.Element("Width") ?? throw new System.Exception("'Width' field not found in xml");
        if (!int.TryParse(xWidth.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var width))
            throw new ArgumentException("Invalid width on tile set '" + schema.Name + "'");

        var xHeight = xTileSet.Element("Height") ?? throw new System.Exception("'Height' field not found in xml");
        if (!int.TryParse(xHeight.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var height))
            throw new ArgumentException("Invalid width on tile set '" + schema.Name + "'");

        var xFormat = xTileSet.Element("Format");
        if (xFormat != null)
            schema.Format = xFormat.Value;

        var xBoundingBox = xTileSet.Element("BoundingBox");
        if (xBoundingBox != null)
        {
            if (!double.TryParse(xBoundingBox.Attribute("minx")?.Value, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var minx) &
                !double.TryParse(xBoundingBox.Attribute("miny")?.Value, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var miny) &
                !double.TryParse(xBoundingBox.Attribute("maxx")?.Value, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var maxx) &
                !double.TryParse(xBoundingBox.Attribute("maxy")?.Value, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var maxy))
            {
                throw new ArgumentException("Invalid LatLonBoundingBox on tile set '" + schema.Name + "'");
            }

            schema.Extent = new Extent(minx, miny, maxx, maxy);

            // In WMS-C the origin is defined as the lower left corner of the bounding box
            schema.OriginX = minx;
            schema.OriginY = miny;
        }

        var xResolutions = xTileSet.Element("Resolutions");
        if (xResolutions != null)
        {
            var count = 0;
            var resolutions = xResolutions.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var resolution in resolutions)
            {
                if (!double.TryParse(resolution, NumberStyles.Any, CultureInfo.InvariantCulture, out var unitsPerPixel))
                    throw new ArgumentException("Invalid resolution on tileSet '" + schema.Name + "'");
                var level = count;
                schema.Resolutions[level] = new Resolution(level, unitsPerPixel, width, height);
                count++;
            }
        }
        return schema;
    }

    private static string CreateDefaultName(IEnumerable<string> layers)
    {
        var stringBuilder = new StringBuilder();
        foreach (var layer in layers)
        {
            stringBuilder.Append(layer + ",");
        }
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        return stringBuilder.ToString();
    }
}
