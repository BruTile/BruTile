using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using mapbox.vector.tile;

namespace BruTile.Samples.VectorTileToBitmap
{
    public class VectorTileConverter
    {
        public byte[] ToBitmap(LayerInfo layerInfo)
        {
            using (var bitmap = new Bitmap(256, 256))
            using (var canvas = Graphics.FromImage(bitmap))
            using (var pen = new Pen(Color.Red, 2))
            {
                foreach (var feature in layerInfo.FeatureCollection.Features)
                {
                    if (feature.Geometry.Type == GeoJSONObjectType.Polygon)
                    {
                        var polygon = (Polygon)feature.Geometry;
                        
                        foreach (var lineString in polygon.Coordinates)
                        {
                            canvas.Transform = new Matrix(); // todo set te matrix transform to match the tile extent.
                            canvas.DrawPolygon(pen, ToGdi(lineString));
                        }
                    }
                }

                return ToBytes(bitmap);
            }
        }

        public PointF[] ToGdi(LineString lineString)
        {
            var result = new List<PointF>();

            foreach (var coordinate in lineString.Coordinates)
            {
                var position = (GeographicPosition) coordinate;
                result.Add(SphericalMercator.FromLonLat(position.Longitude, position.Latitude));

            }

            return result.ToArray();
        }

        public static byte[] ToBytes(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
