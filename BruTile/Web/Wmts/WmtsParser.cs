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
            
            var tileSchemas = GetTileMatrices(capabilties.Contents.TileMatrixSet);
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
                        // this if is not correct but works for my current samples:
                        if (layer.ResourceURL == null)
                        {
                            wmtsRequest = new WmtsRequest(GetGetTileUrls(capabilties.OperationsMetadata.Operation, 
                                layer.Format.First(), capabilties.ServiceIdentification.ServiceTypeVersion.First(), 
                                layer.Identifier.Value, style.Identifier.Value, tileMatrixLink.TileMatrixSet));
                        }
                        else
                        {
                            wmtsRequest = new WmtsRequest(GetResourceUrls(layer.ResourceURL,
                                style.Identifier.Value, tileMatrixLink.TileMatrixSet));
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

        private static IEnumerable<ResourceUrl> GetGetTileUrls(IEnumerable<Generated.Operation> operations, 
            string format, string version, string layer, string style, string tileMatrixSet)
        {
            var list = new List<string>();
            foreach (var operation in operations)
            {
                if (!operation.name.ToLower().Equals("gettile")) continue;
                foreach (var dcp in operation.DCP)
                {
                    foreach (var item in dcp.Item.Items)
                    {
                        list.Add(item.href);
                    }
                }
            }

            return list.Select(s => new ResourceUrl
                {
                    Template = CreateFormatter(s, format, version, layer, style, tileMatrixSet),
                    ResourceType =  Generated.URLTemplateTypeResourceType.tile,
                    Format = format
                });
        }

        private static string CreateFormatter(string baseUrl, string format, string version, string layer, string style, string tileMatrixSet)
        {
            var requestBuilder = new StringBuilder(baseUrl);
            if (!baseUrl.Contains("?")) requestBuilder.Append("?");
            requestBuilder.Append("SERVICE=").Append("WMTS");
            requestBuilder.Append("&REQUEST=").Append("GetTile");
            requestBuilder.Append("&VERSION=").Append(version); 
            requestBuilder.Append("&LAYER=").Append(layer); 
            requestBuilder.Append("&STYLE=").Append(style);
            requestBuilder.Append("&TILEMATRIXSET=").Append(tileMatrixSet);
            requestBuilder.Append("&TILEMATRIX=").Append(WmtsRequest.ZTag);
            requestBuilder.Append("&TILEROW=").Append(WmtsRequest.YTag);
            requestBuilder.Append("&TILECOL=").Append(WmtsRequest.XTag);
            requestBuilder.Append("&FORMAT=").Append(format);
            return requestBuilder.ToString();
        }

        private static IEnumerable<ResourceUrl> GetResourceUrls(IEnumerable<Generated.URLTemplateType> inputResourceUrls,
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

        private static List<TileSchema> GetTileMatrices(IEnumerable<Generated.TileMatrixSet> tileMatrixSets)
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
