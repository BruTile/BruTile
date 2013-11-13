using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BruTile.Web.Wmts
{
    public class WmtsParser
    {
        public static IEnumerable<ITileSource> Parse(Stream source)
        {
            var ser = new XmlSerializer(typeof(Capabilities));

            Capabilities capabilties;

            using (var reader = new StreamReader(source))
            {
                capabilties = (Capabilities)ser.Deserialize(reader);
            }
            
            var tileSchemas = GetTileMatrices(capabilties.Contents.TileMatrixSet);

            var tileSources = GetLayers(capabilties, tileSchemas);

            return tileSources;
        }

        private static IEnumerable<ITileSource> GetLayers(Capabilities capabilties, List<TileSchema> tileSchemas)
        {
            var tileSources = new List<ITileSource>();
            foreach (var layer in capabilties.Contents.Layers)
            {
                var tileSchema = tileSchemas.First(s => Equals(s.Name, layer.TileMatrixSetLink[0].TileMatrixSet));
                var tileSource = new TileSource(new WebTileProvider(new WmtsRequest()), tileSchema)
                    {
                        Title = layer.Title[0].Value
                    };

                tileSources.Add(tileSource);
            }
            return tileSources;
        }

        private static List<TileSchema> GetTileMatrices(IEnumerable<TileMatrixSet> tileMatrixSets)
        {
            var tileSchemas = new List<TileSchema>();
            foreach (var tileMatrixSet in tileMatrixSets)
            {
                var tileSchema = new TileSchema();
                var counter = 0;
                foreach (var tileMatrix in tileMatrixSet.TileMatrix)
                {
                    var coords = tileMatrix.TopLeftCorner.Trim().Split(' ');

                    tileSchema.Matrices.Add(new KeyValuePair<int, TileMatrix>(counter,
                        new TileMatrix
                        {
                            Id = tileMatrix.Identifier.Value,
                            ScaleDenominator = tileMatrix.ScaleDenominator,
                            Left = Convert.ToDouble(coords[0]),
                            Top = Convert.ToDouble(coords[1]),
                            MatrixWidth =  tileMatrix.TileWidth,
                            MatrixHeight = tileMatrix.TileHeight,
                            TileWidth = tileMatrix.TileWidth,
                            TileHeight = tileMatrix.TileHeight
                        }));
                    counter++;
                }
                tileSchema.Name = tileMatrixSet.Identifier.Value;
                tileSchemas.Add(tileSchema);
            }
            return tileSchemas;
        }
    }
}
