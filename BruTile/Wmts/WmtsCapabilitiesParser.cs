// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using BruTile.Web;
using BruTile.Wmts.Generated;

namespace BruTile.Wmts;

/// <summary>
/// An enumeration of possibilities of how to interpret the axis order in &lt;ows:BoundingBox&gt; definitions
/// </summary>
public enum BoundingBoxAxisOrderInterpretation
{
    /// <summary>
    /// Natural, first x, then y
    /// </summary>
    Natural,

    /// <summary>
    /// As defined in the definition of the coordinate reference system
    /// </summary>
    CRS,

    /// <summary>
    /// Geographic, first y (latitude), then x (longitude)
    /// </summary>
    Geographic

}

public class WmtsCapabilitiesParser
{
    /// <summary>
    /// According to OGC SLD 1.0 specification:
    /// The "standardized rendering pixel size" is defined to be 0.28mm × 0.28mm (millimeters).
    /// </summary>
    private const double ScaleHint = 0.00028;

    /// <summary>
    /// Method to parse WMTS tile sources from a stream
    /// </summary>
    /// <param name="source">The source stream</param>
    /// <param name="axisOrder">The way how axis order should be interpreted for &lt;ows:BoundingBox&gt;es</param>
    /// <returns>An enumeration of tile sources</returns>
    public static List<HttpTileSource> Parse(Stream source,
        BoundingBoxAxisOrderInterpretation axisOrder = BoundingBoxAxisOrderInterpretation.Natural,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null)
    {
#pragma warning disable IL2026
        var serializer = new XmlSerializer(typeof(Capabilities));
        Capabilities capabilities;

        using (var reader = new StreamReader(source))
            capabilities = (Capabilities)serializer.Deserialize(reader);
#pragma warning restore IL2026        

        var tileSchemas = GetTileMatrixSets(capabilities.Contents.TileMatrixSet, axisOrder);
        var tileSources = GetLayers(capabilities, tileSchemas, configureHttpRequestMessage);

        return tileSources;
    }

    /// <summary>
    /// Method to extract all image layers from a wmts capabilities document.
    /// </summary>
    /// <param name="capabilities">The capabilities document</param>
    /// <param name="tileSchemas">A set of</param>
    /// <returns></returns>
    private static List<HttpTileSource> GetLayers(Capabilities capabilities, List<WmtsTileSchema> tileSchemas,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null)
    {
        var tileSources = new List<HttpTileSource>();

        foreach (var layer in capabilities.Contents.Layers)
        {
            var identifier = layer.Identifier.Value;
            var title = layer.Title?[0].Value;
            var @abstract = layer.Abstract != null ? layer.Abstract[0].Value : string.Empty;

            foreach (var tileMatrixLink in layer.TileMatrixSetLink)
            {
                if (layer.Style == null)
                    continue;

                foreach (var style in layer.Style)
                {
                    var styleName = style.Identifier.Value ?? "default";
                    foreach (var format in layer.Format)
                    {
                        if (!format.StartsWith("image/"))
                            continue;

                        var tileMatrixSet = tileMatrixLink.TileMatrixSet;
                        var tileSchema = tileSchemas.First(s => Equals(s.Name, tileMatrixSet));
                        var schema = tileSchema.CreateSpecific(title, identifier, @abstract, tileMatrixSet, styleName, format);
                        var wmtsRequest = CreateUrlBuilder(capabilities, layer, tileMatrixLink, format, tileSchema, styleName);
                        var tileSource = new HttpTileSource(schema, wmtsRequest, name: title, configureHttpRequestMessage: configureHttpRequestMessage);

                        tileSources.Add(tileSource);
                    }
                }
            }
        }
        return tileSources;
    }

    private static WmtsUrlBuilder CreateUrlBuilder(Capabilities capabilities, LayerType? layer, TileMatrixSetLink? tileMatrixLink, string? format, WmtsTileSchema tileSchema, string styleName)
    {
        if (layer.ResourceURL == null)
        {
            var resourceUrls = CreateResourceUrlsFromOperations(
                capabilities.OperationsMetadata.Operation,
                format,
                capabilities.ServiceIdentification.ServiceTypeVersion.First(),
                layer.Identifier.Value,
                styleName,
                tileMatrixLink.TileMatrixSet);

            return new WmtsUrlBuilder(resourceUrls, tileSchema.LevelToIdentifier);
        }

        var resourceUrlsFromNode = CreateResourceUrlsFromResourceUrlNode(
            layer.ResourceURL,
            styleName,
            tileMatrixLink.TileMatrixSet);

        return new WmtsUrlBuilder(resourceUrlsFromNode, tileSchema.LevelToIdentifier);
    }

    private static IEnumerable<ResourceUrl> CreateResourceUrlsFromOperations(IEnumerable<Operation> operations,
        string format, string version, string layer, string style, string tileMatrixSet)
    {
        var list = new List<KeyValuePair<string, string>>();
        foreach (var operation in operations)
        {
            if (!operation.name.ToLower().Equals("gettile"))
                continue;

            foreach (var dcp in operation.DCP)
            {
                foreach (var item in dcp.Http.Items)
                {
                    if (item.Constraint != null)
                    {
                        foreach (var constraint in item.Constraint)
                        {
                            foreach (var allowedValue in constraint.AllowedValues)
                            {
                                list.Add(new KeyValuePair<string, string>(((Generated.ValueType)allowedValue).Value, item.href));
                            }
                        }
                    }
                    else
                    {
                        list.Add(item.href.Contains('?')
                            ? new KeyValuePair<string, string>("kvp", item.href)
                            : new KeyValuePair<string, string>(string.Empty, item.href));
                    }
                }
            }
        }

        return list.Select(s => new ResourceUrl
        {
            Template = s.Key.Equals("kvp", StringComparison.CurrentCultureIgnoreCase) ?
                    CreateKvpFormatter(s.Value, format, version, layer, style, tileMatrixSet) :
                    CreateRestfulFormatter(s.Value, format, style, tileMatrixSet),
            ResourceType = URLTemplateTypeResourceType.tile,
            Format = format
        });
    }

    private static string CreateRestfulFormatter(string baseUrl, string format, string style, string tileMatrixSet)
    {
        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";
        return new StringBuilder(baseUrl).Append(style).Append('/').Append(tileMatrixSet)
            .Append("/{TileMatrix}/{TileRow}/{TileCol}").Append('.').Append(format).ToString();
    }

    private static string CreateKvpFormatter(string baseUrl, string format, string version, string layer, string style, string tileMatrixSet)
    {
        var requestBuilder = new StringBuilder(baseUrl);
        if (!baseUrl.Contains('?')) requestBuilder.Append('?');
        requestBuilder
            .Append("SERVICE=").Append("WMTS")
            .Append("&REQUEST=").Append("GetTile")
            .Append("&VERSION=").Append(version)
            .Append("&LAYER=").Append(layer)
            .Append("&STYLE=").Append(style)
            .Append("&TILEMATRIXSET=").Append(tileMatrixSet)
            .Append("&TILEMATRIX=").Append(WmtsUrlBuilder.ZTag)
            .Append("&TILEROW=").Append(WmtsUrlBuilder.YTag)
            .Append("&TILECOL=").Append(WmtsUrlBuilder.XTag)
            .Append("&FORMAT=").Append(format);
        return requestBuilder.ToString();
    }

    private static List<ResourceUrl> CreateResourceUrlsFromResourceUrlNode(
        IEnumerable<URLTemplateType> inputResourceUrls, string style, string tileMatrixSet)
    {
        var resourceUrls = new List<ResourceUrl>();
        foreach (var resourceUrl in inputResourceUrls)
        {
            var template = resourceUrl.template.Replace(WmtsUrlBuilder.TileMatrixSetTag, tileMatrixSet);
            template = Regex.Replace(template, WmtsUrlBuilder.StyleTag, style, RegexOptions.IgnoreCase);
            resourceUrls.Add(new ResourceUrl
            {
                Format = resourceUrl.format,
                ResourceType = resourceUrl.resourceType,
                Template = template
            });
        }
        return resourceUrls;
    }

    private static List<WmtsTileSchema> GetTileMatrixSets(IEnumerable<TileMatrixSet> tileMatrixSets,
        BoundingBoxAxisOrderInterpretation axisOrder)
    {
        // Get a set of well known scale sets. For these we don't need to have
        var wellKnownScaleSets = new WellKnownScaleSets();

        // Axis order registry
        var crsAxisOrder = new CrsAxisOrderRegistry();
        // Unit of measure registry
        var crsUnitOfMeasure = new CrsUnitOfMeasureRegistry();

        var tileSchemas = new List<WmtsTileSchema>();
        foreach (var tileMatrixSet in tileMatrixSets)
        {
            // Check if a Well-Known scale set is used, either by Identifier or WellKnownScaleSet property
            var ss = wellKnownScaleSets[tileMatrixSet.Identifier.Value];
            if (ss == null && !string.IsNullOrEmpty(tileMatrixSet.WellKnownScaleSet))
                ss = wellKnownScaleSets[tileMatrixSet.WellKnownScaleSet.Split(':').Last()];

            // Try to parse the Crs
            var supportedCrs = tileMatrixSet.SupportedCRS;

            // Hack to fix broken crs spec
            supportedCrs = supportedCrs.Replace("6.18:3", "6.18.3");

            if (!CrsIdentifier.TryParse(supportedCrs, out var crs))
            {
                // If we cannot parse the crs, we cannot compute tile schema, thus ignore.
                // Todo: Log this
                continue;
            }

            // Get the ordinate order for the crs (x, y) or (y, x) aka (lat, long)
            var ordinateOrder = crsAxisOrder[crs];
            // Get the unit of measure for the crs
            var unitOfMeasure = crsUnitOfMeasure[crs];

            // Create a new WMTS tile schema 
            var tileSchema = new WmtsTileSchema();

            var tileMatrices = SetLevelOnTileTileMatrix(tileMatrixSet);

            // Add the resolutions
            foreach (var tileMatrix in tileMatrices)
            {
                tileSchema.Resolutions.Add(ToResolution(tileMatrix, ordinateOrder, unitOfMeasure.ToMeter, ss));
            }

            tileSchema.Extent = ToExtent(tileMatrixSet, tileSchema, GetOrdinateOrder(axisOrder, ordinateOrder));

            // Fill in the remaining properties
            tileSchema.Name = tileMatrixSet.Identifier.Value;
            tileSchema.YAxis = YAxis.OSM;
            tileSchema.Srs = supportedCrs;
            tileSchema.SupportedSRS = crs;

            foreach (var tileMatrix in tileMatrices)
            {
                tileSchema.LevelToIdentifier[tileMatrix.Level] = tileMatrix.Identifier.Value;
            }

            // Record the tile schema
            tileSchemas.Add(tileSchema);
        }
        return tileSchemas;
    }

    private static List<TileMatrix> SetLevelOnTileTileMatrix(TileMatrixSet tileMatrixSet)
    {
        var tileMatrices = tileMatrixSet.TileMatrix.OrderByDescending(m => m.ScaleDenominator).ToList();
        var count = 0;
        foreach (var tileMatrix in tileMatrices)
        {
            tileMatrix.Level = count;
            count++;
        }

        return tileMatrices;
    }

    private static int[] GetOrdinateOrder(BoundingBoxAxisOrderInterpretation axisOrder, int[] ordinateOrder)
    {
        return axisOrder switch
        {
            BoundingBoxAxisOrderInterpretation.Natural => [0, 1],
            BoundingBoxAxisOrderInterpretation.Geographic => [1, 0],
            _ => ordinateOrder,
        };
    }

    private static Extent ToExtent(TileMatrixSet tileMatrixSet, WmtsTileSchema tileSchema, int[] ordinateOrder)
    {
        var boundingBox = tileMatrixSet.Item;
        if (boundingBox != null)
        {
            // the BoundingBox element should always be in the same CRS as the SupportedCRS, we make a check anyway
            if (string.IsNullOrEmpty(boundingBox.crs) || boundingBox.crs == tileMatrixSet.SupportedCRS)
            {
                var lowerCorner = boundingBox.LowerCorner;
                var upperCorner = boundingBox.UpperCorner;

                var lowerCornerDimensions = GetDimensions(lowerCorner);
                var upperCornerDimensions = GetDimensions(upperCorner);

                var xi = ordinateOrder[0];
                var yi = ordinateOrder[1];

                return new Extent(
                    lowerCornerDimensions[xi],
                    lowerCornerDimensions[yi],
                    upperCornerDimensions[xi],
                    upperCornerDimensions[yi]);
            }
        }

        // Compute the extent of the tile schema
        var resolution = tileSchema.Resolutions.Last();
        return ToExtent(resolution.Value);
    }

    private static double[] GetDimensions(string s)
    {
        var dims = s.Split(' ');
        return
        [
            double.Parse(dims[0], CultureInfo.InvariantCulture.NumberFormat),
            double.Parse(dims[1], CultureInfo.InvariantCulture.NumberFormat)
        ];
    }

    private static Extent ToExtent(Resolution tileMatrix)
    {
        return new Extent(
            tileMatrix.Left,
            tileMatrix.Top - tileMatrix.UnitsPerPixel * tileMatrix.TileHeight * tileMatrix.MatrixHeight,
            tileMatrix.Left + tileMatrix.UnitsPerPixel * tileMatrix.TileWidth * tileMatrix.MatrixWidth,
            tileMatrix.Top);
    }

    private static KeyValuePair<int, Resolution> ToResolution(TileMatrix tileMatrix,
        int[] ordinateOrder, double metersPerUnit = 1, ScaleSet? scaleSet = null)
    {
        // Get the coordinates
        var coordinates = tileMatrix.TopLeftCorner.Trim().Split(' ');

        // Try to get units per pixel from passed scale set
        var unitsPerPixel = tileMatrix.ScaleDenominator * ScaleHint / metersPerUnit;
        if (unitsPerPixel == 0 || double.IsNaN(unitsPerPixel))
        {
            ArgumentNullException.ThrowIfNull(scaleSet);
            unitsPerPixel = scaleSet[tileMatrix.ScaleDenominator].GetValueOrDefault(0d);
        }

        return new KeyValuePair<int, Resolution>(tileMatrix.Level,
            new Resolution(
                tileMatrix.Level,
                unitsPerPixel,
                tileMatrix.TileWidth,
                tileMatrix.TileHeight,
                Convert.ToDouble(coordinates[ordinateOrder[0]],
                CultureInfo.InvariantCulture),
                Convert.ToDouble(coordinates[ordinateOrder[1]],
                CultureInfo.InvariantCulture),
                (long)tileMatrix.MatrixWidth,
                (long)tileMatrix.MatrixHeight,
                tileMatrix.ScaleDenominator));
    }
}
