using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BruTile.Cache;
using BruTile.Web;
using mapbox.vector.tile;

namespace BruTile.Samples.VectorTileToBitmap
{
    public class HttpVectorTileSource : HttpTileSource
    {
        public HttpVectorTileSource(ITileSchema tileSchema, string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey = null, string name = null, IPersistentCache<byte[]> persistentCache = null, Func<Uri, byte[]> tileFetcher = null) : base(tileSchema, urlFormatter, serverNodes, apiKey, name, persistentCache, tileFetcher)
        {
        }

        public override byte[] GetTile(TileInfo tileInfo)
        {
            var bytes = base.GetTile(tileInfo);
            var index = tileInfo.Index;
            var layerInfos = VectorTileParser.Parse(new MemoryStream(bytes), index.Col, index.Row, Int32.Parse(index.Level));
            var tileWidth = Schema.GetTileWidth(tileInfo.Index.Level);
            var tileHeight = Schema.GetTileHeight(tileInfo.Index.Level);
            var geoJSONRenderer = new GeoJSONRenderer(tileWidth, tileHeight, ToGeoJSONArray(tileInfo.Extent));
            return geoJSONRenderer.Render(layerInfos.Select(i => i.FeatureCollection));
        }

        private double[] ToGeoJSONArray(Extent extent)
        {
            // GeoJSON.NET has no class for bounding boxes. It just holds them in a double array. 
            // The spec says it should first the lowest and then all the highest values for all axes:
            // http://geojson.org/geojson-spec.html#bounding-boxes
            return new [] {extent.MinX, extent.MinY, extent.MaxX, extent.MaxY };

        }
    }
}
