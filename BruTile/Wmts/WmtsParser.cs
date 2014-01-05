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

        public static IEnumerable<ITileSource> Parse(Stream source)
        {
            var ser = new XmlSerializer(typeof(Capabilities));
            Capabilities capabilties;

            using (var reader = new StreamReader(source))
            {
                capabilties = (Capabilities)ser.Deserialize(reader);
            }
            
            var tileSchemas = GetTileMatrixSets(capabilties.Contents.TileMatrixSet);
            var tileSources = GetLayers(capabilties, tileSchemas);

            return tileSources;
        }

        private static IEnumerable<ITileSource> GetLayers(Capabilities capabilties, List<ITileSchema> tileSchemas)
        {
            var tileSources = new List<ITileSource>();

            foreach (var layer in capabilties.Contents.Layers)
            {
                foreach (var tileMatrixLink in layer.TileMatrixSetLink)
                {
                    foreach (var style in layer.Style)
                    {
                        IRequest wmtsRequest;
                        
                        if (layer.ResourceURL == null)
                        {
                            wmtsRequest = new WmtsRequest(CreateResourceUrlsFromOperations(
                                capabilties.OperationsMetadata.Operation, 
                                layer.Format.First(), 
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
                        var tileSchema = tileSchemas.First(s => Equals(s.Name, tileMatrixLink.TileMatrixSet));
                        var tileSource = new TileSource(new WebTileProvider(wmtsRequest), tileSchema)
                            {
                                Title = layer.Identifier.Value
                            };

                        tileSources.Add(tileSource);
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
            var tileSchemas = new List<ITileSchema>();
            foreach (var tileMatrixSet in tileMatrixSets)
            {
                var tileSchema = new WmtsTileSchema();
                foreach (var tileMatrix in tileMatrixSet.TileMatrix)
                {
                    tileSchema.Resolutions.Add(ToResolution(tileMatrix));
                }

                // Extent should be determined by the WGS84BoundingBox of the layer but
                // this would involve projection.
                // also the Extent should move to the tileSource because it could differ 
                // between layers.
                tileSchema.Extent = ToExtent(tileSchema.Resolutions.First().Value);

                tileSchema.Name = tileMatrixSet.Identifier.Value;
                tileSchema.Axis = AxisDirection.InvertedY;
                tileSchema.Srs = tileMatrixSet.SupportedCRS;
                tileSchemas.Add(tileSchema);
            }
            return tileSchemas;
        }

        private static Extent ToExtent(Resolution tileMatrix)
        {
            return new Extent(
                tileMatrix.Left,
                tileMatrix.Top - tileMatrix.ScaleDenominator * ScaleHint * tileMatrix.TileHeight * tileMatrix.MatrixHeight,
                tileMatrix.Left + tileMatrix.ScaleDenominator * ScaleHint * tileMatrix.TileWidth * tileMatrix.MatrixWidth,
                tileMatrix.Top);
        }

        private static KeyValuePair<string, Resolution> ToResolution(Generated.TileMatrix tileMatrix)
        {
            var coords = tileMatrix.TopLeftCorner.Trim().Split(' ');

            return new KeyValuePair<string, Resolution>(tileMatrix.Identifier.Value,
                new Resolution
                {
                    Id = tileMatrix.Identifier.Value,
                    UnitsPerPixel = tileMatrix.ScaleDenominator * ScaleHint,
                    ScaleDenominator = tileMatrix.ScaleDenominator,
                    Left = Convert.ToDouble(coords[0], CultureInfo.InvariantCulture),
                    Top = Convert.ToDouble(coords[1], CultureInfo.InvariantCulture),
                    MatrixWidth = tileMatrix.MatrixWidth,
                    MatrixHeight = tileMatrix.MatrixHeight,
                    TileWidth = tileMatrix.TileWidth,
                    TileHeight = tileMatrix.TileHeight
                });
        }
    }
}
