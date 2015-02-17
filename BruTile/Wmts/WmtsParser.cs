// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using BruTile.Web;
using BruTile.Wmts.Generated;

namespace BruTile.Wmts
{
    public class WmtsParser
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
        /// <returns>An enumeration of tile sources</returns>
        public static IEnumerable<ITileSource> Parse(Stream source)
        {
            var ser = new XmlSerializer(typeof(Capabilities));
            Capabilities capabilties;

            using (var reader = new StreamReader(source))
                capabilties = (Capabilities)ser.Deserialize(reader);
            
            var tileSchemas = GetTileMatrixSets(capabilties.Contents.TileMatrixSet);
            var tileSources = GetLayers(capabilties, tileSchemas);

            return tileSources;
        }

        /// <summary>
        /// Method to extract all image layers from a wmts capabilities document.
        /// </summary>
        /// <param name="capabilties">The capabilities document</param>
        /// <param name="tileSchemas">A set of</param>
        /// <returns></returns>
        private static IEnumerable<ITileSource> GetLayers(Capabilities capabilties, List<ITileSchema> tileSchemas)
        {
            var tileSources = new List<ITileSource>();

            foreach (var layer in capabilties.Contents.Layers)
            {
                var identifier = layer.Identifier.Value;
                var title = layer.Title[0].Value;
                string @abstract = layer.Abstract != null ? layer.Abstract[0].Value : string.Empty;

                foreach (var tileMatrixLink in layer.TileMatrixSetLink)
                {
                    foreach (var style in layer.Style)
                    {
                        foreach (var format in layer.Format)
                        {
                            if (!format.StartsWith("image/")) continue;

                            IRequest wmtsRequest;

                            if (layer.ResourceURL == null)
                            {
                                wmtsRequest = new WmtsRequest(CreateResourceUrlsFromOperations(
                                    capabilties.OperationsMetadata.Operation,
                                    format,
                                    capabilties.ServiceIdentification.ServiceTypeVersion.First(),
                                    layer.Identifier.Value,
                                    style.Identifier.Value,
                                    tileMatrixLink.TileMatrixSet));
                            }
                            else
                            {
                                wmtsRequest = new WmtsRequest(CreateResourceUrlsFromResourceUrlNode(
                                    layer.ResourceURL,
                                    style.Identifier.Value,
                                    tileMatrixLink.TileMatrixSet));
                            }

                            var tileMatrixSet = tileMatrixLink.TileMatrixSet;
                            var tileSchema = (WmtsTileSchema)tileSchemas.First(s => Equals(s.Name, tileMatrixSet));

                            //var layerName = layer.Identifier.Value;
                            var styleName = style.Identifier.Value;

                            var tileSource = new TileSource(new WebTileProvider(wmtsRequest),
                                tileSchema.CreateSpecific(title, identifier, @abstract, tileMatrixSet, styleName, format))
                                {
                                    Name = title
                                };

                            tileSources.Add(tileSource);
                        }
                    }
                }
            }
            return tileSources;
        }


        private static IEnumerable<ResourceUrl> CreateResourceUrlsFromOperations(IEnumerable<Operation> operations, 
            string format, string version, string layer, string style, string tileMatrixSet)
        {
            var list = new List<KeyValuePair<string, string>>();
            foreach (var operation in operations)
            {
                if (!operation.name.ToLower().Equals("gettile")) continue;
                foreach (var dcp in operation.DCP)
                {
                    foreach (var item in dcp.Http.Items)
                    {
                        foreach (var constraint in item.Constraint)
                        {
                            foreach (var allowedValue in constraint.AllowedValues)
                            {
                                list.Add(new KeyValuePair<string, string>(((Generated.ValueType)allowedValue).Value, item.href));
                            }
                        }
                    }
                }
            }

            return list.Select(s => new ResourceUrl
                {
                    Template = s.Key.ToLower() =="kvp" ? 
                        CreateKvpFormatter(s.Value, format, version, layer, style, tileMatrixSet):
                        CreateRestfulFormatter(s.Value, format, style, tileMatrixSet),
                    ResourceType =  URLTemplateTypeResourceType.tile,
                    Format = format
                });
        }

        private static string CreateRestfulFormatter(string baseUrl, string format, string style, string tileMatrixSet)
        {
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            return new StringBuilder(baseUrl).Append(style).Append("/").Append(tileMatrixSet)
                .Append("/{TileMatrix}/{TileRow}/{TileCol}").Append(".").Append(format).ToString();
        }

        private static string CreateKvpFormatter(string baseUrl, string format, string version, string layer, string style, string tileMatrixSet)
        {
            var requestBuilder = new StringBuilder(baseUrl);
            if (!baseUrl.Contains("?")) requestBuilder.Append("?");
            requestBuilder.Append("SERVICE=").Append("WMTS")
                          .Append("&REQUEST=").Append("GetTile")
                          .Append("&VERSION=").Append(version)
                          .Append("&LAYER=").Append(layer)
                          .Append("&STYLE=").Append(style)
                          .Append("&TILEMATRIXSET=").Append(tileMatrixSet)
                          .Append("&TILEMATRIX=").Append(WmtsRequest.ZTag)
                          .Append("&TILEROW=").Append(WmtsRequest.YTag)
                          .Append("&TILECOL=").Append(WmtsRequest.XTag)
                          .Append("&FORMAT=").Append(format);
            return requestBuilder.ToString();
        }

        private static IEnumerable<ResourceUrl> CreateResourceUrlsFromResourceUrlNode(IEnumerable<URLTemplateType> inputResourceUrls,
            string style, string tileMatrixSet)
        {
            var resourceUrls = new List<ResourceUrl>();
            foreach (var resourceUrl in inputResourceUrls)
            {
                var template = resourceUrl.template.Replace(WmtsRequest.TileMatrixSetTag, tileMatrixSet);
                template = template.Replace(WmtsRequest.StyleTag, style);
                resourceUrls.Add(new ResourceUrl
                    {
                        Format = resourceUrl.format,
                        ResourceType = resourceUrl.resourceType,
                        Template = template
                    });
            }
            return resourceUrls;
        }


        private static List<ITileSchema> GetTileMatrixSets(IEnumerable<TileMatrixSet> tileMatrixSets)
        {
            // Get a set of well known scale sets. For these we don't need to have
            var wkss = new WellKnownScaleSets();
            
            // Axis order registry
            var crsAxisOrder = new CrsAxisOrderRegistry();
            // Unit of measure registry
            var crsUnitOfMeasure = new CrsUnitOfMeasureRegistry();

            var tileSchemas = new List<ITileSchema>();
            foreach (var tileMatrixSet in tileMatrixSets)
            {
                // Check if a Well-Known scale set is used, either by Identifier or WellKnownScaleSet property
                var ss = wkss[tileMatrixSet.Identifier.Value];
                if (ss == null && !string.IsNullOrEmpty(tileMatrixSet.WellKnownScaleSet))
                    ss = wkss[tileMatrixSet.WellKnownScaleSet.Split(':').Last()];

                // Try to parse the Crs
                var supportedCrs = tileMatrixSet.SupportedCRS;
                
                // Hack to fix broken crs spec
                supportedCrs = supportedCrs.Replace("6.18:3", "6.18.3");

                CrsIdentifier crs;
                if (!CrsIdentifier.TryParse(supportedCrs, out crs))
                {
                    // If we cannot parse the crs, we cannot compute tile schema, thus ignore.
                    // ToDo: Log this
                    continue;
                }

                // Get the ordinate order for the crs (x, y) or (y, x) aka (lat, long)
                var ordinateOrder = crsAxisOrder[crs];
                // Get the unit of measure for the crs
                var unitOfMeasure = crsUnitOfMeasure[crs];

                // Create a new WMTS tile schema 
                var tileSchema = new WmtsTileSchema();

                // Add the resolutions
                foreach (var tileMatrix in tileMatrixSet.TileMatrix)
                {
                    tileSchema.Resolutions.Add(ToResolution(tileMatrix, ordinateOrder, unitOfMeasure.ToMeter, ss));
                }

                tileSchema.Extent = ToExtent(tileMatrixSet, tileSchema, ordinateOrder);

                // Fill in the remaining properties
                tileSchema.Name = tileMatrixSet.Identifier.Value;
                tileSchema.YAxis = YAxis.OSM;
                tileSchema.Srs = supportedCrs;
                tileSchema.SupportedSRS = crs;

                // record the tile schema
                tileSchemas.Add(tileSchema);
            }
            return tileSchemas;
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
            var res = tileSchema.Resolutions.Last();
            return ToExtent(res.Value);
        }

        private static double[] GetDimensions(string s)
        {
            var dims = s.Split(' ');
            return new[]
            {
                double.Parse(dims[0], CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(dims[1], CultureInfo.InvariantCulture.NumberFormat)
            };
        }

        private static Extent ToExtent(Resolution tileMatrix)
        {
            //var pixelSpan = tileMatrix.
            return new Extent(
                tileMatrix.Left,
                tileMatrix.Top - tileMatrix.UnitsPerPixel * tileMatrix.TileHeight * tileMatrix.MatrixHeight,
                tileMatrix.Left + tileMatrix.UnitsPerPixel * tileMatrix.TileWidth * tileMatrix.MatrixWidth,
                tileMatrix.Top);
        }

        private static KeyValuePair<string, Resolution> ToResolution(Generated.TileMatrix tileMatrix, int[] ordinateOrder, double metersPerUnit = 1, ScaleSet ss = null)
        {
            
            // Get the coordinates
            var coords = tileMatrix.TopLeftCorner.Trim().Split(' ');
            
            // Try to get units per pixel from passed scale set
            var unitsPerPixel = tileMatrix.ScaleDenominator*ScaleHint/metersPerUnit;
            if (unitsPerPixel == 0 || double.IsNaN(unitsPerPixel))
            {
                if (ss == null)
                    throw new ArgumentNullException();
                unitsPerPixel = ss[tileMatrix.ScaleDenominator].GetValueOrDefault(0d);
            }

            return new KeyValuePair<string, Resolution>(tileMatrix.Identifier.Value,
                new Resolution
                {
                    Id = tileMatrix.Identifier.Value,
                    UnitsPerPixel = unitsPerPixel,
                    ScaleDenominator = tileMatrix.ScaleDenominator,
                    Left = Convert.ToDouble(coords[ordinateOrder[0]], CultureInfo.InvariantCulture),
                    Top = Convert.ToDouble(coords[ordinateOrder[1]], CultureInfo.InvariantCulture),
                    MatrixWidth = tileMatrix.MatrixWidth,
                    MatrixHeight = tileMatrix.MatrixHeight,
                    TileWidth = tileMatrix.TileWidth,
                    TileHeight = tileMatrix.TileHeight
                });
        }
    }
}
