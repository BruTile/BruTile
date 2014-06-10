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

                            var layerName = layer.Identifier.Value;
                            var styleName = style.Identifier.Value;

                            var tileSource = new TileSource(new WebTileProvider(wmtsRequest), 
                                tileSchema.CreateSpecific(layerName, styleName, format))
                                {
                                    Title = layer.Identifier.Value,
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
            var wkss = new WellKnownScaleSets();
            
            var crsAxisOrder = new CrsAxisOrderRegistry();
            var crsUnitOfMeasure = new CrsUnitOfMeasureRegistry();

            var tileSchemas = new List<ITileSchema>();
            foreach (var tileMatrixSet in tileMatrixSets)
            {
                // this is not according to the spec!
                var ss = wkss[tileMatrixSet.Identifier.Value];
                if (ss == null && !string.IsNullOrEmpty(tileMatrixSet.WellKnownScaleSet))
                {
                    ss = wkss[tileMatrixSet.WellKnownScaleSet.Split(':').Last()];
                }

                //Try to parse the Crs
                var supportedCrs = tileMatrixSet.SupportedCRS;
                
                //Hack to fix broken spec
                supportedCrs = supportedCrs.Replace("6.18:3", "6.18.3");

                CrsIdentifier crs;
                if (!CrsIdentifier.TryParse(supportedCrs, out crs))
                {
                    // ToDo: Log this
                    continue;
                }
                var ordinateOrder = crsAxisOrder[crs];
                var unitOfMeasure = crsUnitOfMeasure[crs];

                var tileSchema = new WmtsTileSchema();

                foreach (var tileMatrix in tileMatrixSet.TileMatrix)
                {
                    tileSchema.Resolutions.Add(ToResolution(tileMatrix, ordinateOrder, unitOfMeasure.ToMeter,  ss));
                }

                var res = tileSchema.Resolutions.Last();
                tileSchema.Extent = ToExtent(res.Value);

                tileSchema.Name = tileMatrixSet.Identifier.Value;
                tileSchema.Axis = AxisDirection.InvertedY;
                tileSchema.Srs = supportedCrs;
                tileSchema.SupportedSRS = crs;

                tileSchemas.Add(tileSchema);
            }
            return tileSchemas;
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
            
            var coords = tileMatrix.TopLeftCorner.Trim().Split(' ');
            var unitsPerPixel = ss != null ? ss[tileMatrix.ScaleDenominator] : null;

            return new KeyValuePair<string, Resolution>(tileMatrix.Identifier.Value,
                new Resolution
                {
                    Id = tileMatrix.Identifier.Value,
                    UnitsPerPixel = unitsPerPixel ?? tileMatrix.ScaleDenominator * ScaleHint / metersPerUnit ,
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
