using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            return GeoJsonRenderer.ToBitmap(layerInfos.Select(i => i.FeatureCollection), tileInfo);
        }
    }
}
