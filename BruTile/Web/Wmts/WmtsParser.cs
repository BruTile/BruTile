using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Generated = BruTile.Web.Wmts.Generated;

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
                        var wmtsRequest = new WmtsRequest(GetResourceUrls(layer.ResourceURL), tileMatrixLink.TileMatrixSet, style.Identifier.Value);

                        var tileSchema = tileSchemas.First(s => Equals(s.Name, layer.TileMatrixSetLink[0].TileMatrixSet));
                        var tileSource = new TileSource(new WebTileProvider(wmtsRequest), tileSchema)
                            {
                                Title = layer.Title[0].Value
                            };

                        tileSources.Add(tileSource);
                    }
                }
            }
            return tileSources;
        }

        private static IEnumerable<ResourceUrl> GetResourceUrls(IEnumerable<Generated.URLTemplateType> inputResourceUrls)
        {
            var resourceUrls = new List<ResourceUrl>();
            foreach (var resourceUrl in inputResourceUrls)
            {
                resourceUrls.Add(new ResourceUrl
                    {
                        Format = resourceUrl.format,
                        ResourceType = resourceUrl.resourceType,
                        Template = resourceUrl.template
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
                    Left = Convert.ToDouble(coords[0]),
                    Top = Convert.ToDouble(coords[1]),
                    MatrixWidth = tileMatrix.MatrixWidth,
                    MatrixHeight = tileMatrix.MatrixHeight,
                    TileWidth = tileMatrix.TileWidth,
                    TileHeight = tileMatrix.TileHeight
                });
        }
    }
}
