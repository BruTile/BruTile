using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace BruTile.Web
{
    public class TmsTileSource : ITileSource
    {
        ITileSchema _tileSchema;
        ITileProvider _tileProvider;
        readonly string _overrideTileUrl; //When the configfile points to an invalid tile url fill this one

        public TmsTileSource(string serviceUrl, ITileSchema tileSchema)
            : this(new Uri(serviceUrl), tileSchema)
        {
        }

        public TmsTileSource(Uri serviceUri, ITileSchema tileSchema)
        {
            _tileSchema = tileSchema;
            _tileProvider = new WebTileProvider(new TmsRequest(serviceUri, tileSchema.Format));
        }

        public TmsTileSource(Stream serviceDescription)
        {
            Create(serviceDescription);
        }

        public TmsTileSource(Stream serviceDescription, string overwriteSourceUrl)
        {
            _overrideTileUrl = overwriteSourceUrl;
            Create(serviceDescription);
        }

        private void Create(Stream serviceDescription)
        {
            var reader = new StreamReader(serviceDescription);
            var serializer = new XmlSerializer(typeof(TileMap));
            var tileMap = (TileMap)serializer.Deserialize(reader);
            _tileSchema = GenerateSchema(tileMap);

            var tileUrls = new List<Uri>();
            foreach (TileMapTileSetsTileSet ts in tileMap.TileSets.TileSet)
            {
                tileUrls.Add(new Uri(ts.href));
            }
            _tileProvider = new WebTileProvider(CreateRequest(tileUrls));
        }

        private static TileSchema GenerateSchema(TileMap tileMap)
        {
            var schema = new TileSchema();
            schema.OriginX = Double.Parse(tileMap.Origin.x, CultureInfo.InvariantCulture);
            schema.OriginY = Double.Parse(tileMap.Origin.y, CultureInfo.InvariantCulture);
            schema.Srs = tileMap.SRS;
            schema.Width = Int32.Parse(tileMap.TileFormat.width);
            schema.Height = Int32.Parse(tileMap.TileFormat.height);
            schema.Name = tileMap.Title;
            schema.Format = tileMap.TileFormat.extension;
            schema.Axis = AxisDirection.Normal;
            schema.Extent = new Extent(Double.Parse(tileMap.BoundingBox.minx, CultureInfo.InvariantCulture), Double.Parse(tileMap.BoundingBox.miny, CultureInfo.InvariantCulture), Double.Parse(tileMap.BoundingBox.maxx, CultureInfo.InvariantCulture), Double.Parse(tileMap.BoundingBox.maxy, CultureInfo.InvariantCulture));
            for (int i = 0; i < tileMap.TileSets.TileSet.Length; i++)
            {
                double resolution = Double.Parse(tileMap.TileSets.TileSet[i].unitsperpixel, CultureInfo.InvariantCulture);
                schema.Resolutions.Add(resolution);
            }
            return schema;
        }

        private IRequest CreateRequest(IList<Uri> tileUrls)
        {
            IRequest request;

            if (_overrideTileUrl != null)
                request = new TmsRequest(new Uri(_overrideTileUrl), Schema.Format, null);
            else
                request = new TmsRequest(tileUrls, Schema.Format, new Dictionary<string, string>());

            return request;
        }

        #region ITileSource Members

        public ITileProvider Provider
        {
            get { return _tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return _tileSchema; }
        }

        #endregion
    }
}
