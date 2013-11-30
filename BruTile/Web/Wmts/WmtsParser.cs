using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BruTile.Web.Wmts
{
    public class WmtsParser
    {
        public static IEnumerable<ITileSource> Parse(Stream source)
        {
            var ser = new XmlSerializer(typeof(Generated.Capabilities));
            Generated.Capabilities capabilties;

            using (var reader = new StreamReader(source))
            {
                capabilties = (Generated.Capabilities)ser.Deserialize(reader);
            }
            
            var tileSchemas = GetTileMatrixSets(capabilties.Contents.TileMatrixSet);
            var tileSources = GetLayers(capabilties, tileSchemas);

            return tileSources;
        }

        private static IEnumerable<ITileSource> GetLayers(Generated.Capabilities capabilties, List<TileSchema> tileSchemas)
        {
            var tileSources = new List<ITileSource>();

            foreach (var layer in capabilties.Contents.Layers)
            {
                foreach (var tileMatrixLink in layer.TileMatrixSetLink)
                {
                    foreach (var style in layer.Style)
                    {
                        IRequest wmtsRequest;
                        
                        if (true)//layer.ResourceURL == null)
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

        private static IEnumerable<ResourceUrl> CreateResourceUrlsFromOperations(IEnumerable<Generated.Operation> operations, 
            string format, string version, string layer, string style, string tileMatrixSet)
        {
            var list = new List<Tuple<string, string>>();
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
                                list.Add(new Tuple<string, string>(allowedValue.ToString(), item.href));
                            }
                        }
                    }
                }
            }

            return list.Select(s => new ResourceUrl
                {
                    Template = s.Item1.ToLower() =="kvp" ? 
                        CreateKvpFormatter(s.Item2, format, version, layer, style, tileMatrixSet):
                        CreateRestfulFormatter(s.Item2, format, style, tileMatrixSet),
                    ResourceType =  Generated.URLTemplateTypeResourceType.tile,
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

        private static IEnumerable<ResourceUrl> CreateResourceUrlsFromResourceUrlNode(IEnumerable<Generated.URLTemplateType> inputResourceUrls,
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

        private static List<TileSchema> GetTileMatrixSets(IEnumerable<Generated.TileMatrixSet> tileMatrixSets)
        {
            var tileSchemas = new List<TileSchema>();
            foreach (var tileMatrixSet in tileMatrixSets)
            {
                var tileSchema = new TileSchema();
                foreach (var tileMatrix in tileMatrixSet.TileMatrix)
                {
                    tileSchema.Resolutions.Add(ToResolution(tileMatrix));
                }
                var firstTileMatrix = tileSchema.Resolutions.First().Value;

                tileSchema.Width = firstTileMatrix.TileWidth;
                tileSchema.Height = firstTileMatrix.TileHeight;
                tileSchema.OriginX = firstTileMatrix.Left;
                tileSchema.OriginY = firstTileMatrix.Top;
                tileSchema.Extent = ToExtent(firstTileMatrix);
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
                tileMatrix.Top - tileMatrix.ScaleDenominator * 0.00028 * tileMatrix.TileHeight * tileMatrix.MatrixHeight,
                tileMatrix.Left + tileMatrix.ScaleDenominator * 0.00028 * tileMatrix.TileWidth * tileMatrix.MatrixWidth,
                tileMatrix.Top);
        }

        private static KeyValuePair<string, Resolution> ToResolution(Generated.TileMatrix tileMatrix)
        {
            var coords = tileMatrix.TopLeftCorner.Trim().Split(' ');

            return new KeyValuePair<string, Resolution>(tileMatrix.Identifier.Value,
                new Resolution
                {
                    Id = tileMatrix.Identifier.Value,
                    UnitsPerPixel = tileMatrix.ScaleDenominator * 0.00028,
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
