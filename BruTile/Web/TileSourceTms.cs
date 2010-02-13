using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace BruTile.Web
{
    public class TileSourceTms : ITileSource
    {
        ITileSchema tileSchema;
        ITileProvider tileProvider;
        string overrideTileURL; //When the configfile points to an invalid tile url fill this one

        public TileSourceTms(string serviceUrl, ITileSchema tileSchema)
            : this(new Uri(serviceUrl), tileSchema)
        {
        }

        public TileSourceTms(Uri serviceUri, ITileSchema tileSchema)
        {
            this.tileSchema = tileSchema;
            this.tileProvider = new WebTileProvider(new RequestTms(serviceUri, tileSchema.Format));
        }

        public TileSourceTms(Stream serviceDescription)
        {
            Create(serviceDescription);
        }

        public TileSourceTms(Stream serviceDescription, string overwriteSourceUrl)
        {
            this.overrideTileURL = overwriteSourceUrl;
            Create(serviceDescription);
        }

        private void Create(Stream serviceDescription)
        {
            StreamReader reader = new StreamReader(serviceDescription);
            XmlSerializer serializer = new XmlSerializer(typeof(TileMap));
            TileMap tileMap = (TileMap)serializer.Deserialize(reader);
            this.tileSchema = GenerateSchema(tileMap);

            List<Uri> tileUrls = new List<Uri>();
            foreach (TileMapTileSetsTileSet ts in tileMap.TileSets.TileSet)
            {
                tileUrls.Add(new Uri(ts.href));
            }
            tileProvider = new WebTileProvider(CreateRequestBuilder(tileUrls));
        }

        private static TileSchema GenerateSchema(TileMap tileMap)
        {
            TileSchema schema = new TileSchema();
            schema.OriginX = Double.Parse(tileMap.Origin.x, CultureInfo.InvariantCulture);
            schema.OriginY = Double.Parse(tileMap.Origin.x, CultureInfo.InvariantCulture);
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

        private IRequestBuilder CreateRequestBuilder(IList<Uri> tileUrls)
        {
            IRequestBuilder request;

            if (overrideTileURL != null)
                request = new RequestTms(new Uri(overrideTileURL), Schema.Format, null);
            else
                request = new RequestTms(tileUrls, Schema.Format, new Dictionary<string, string>());

            return request;
        }

        #region ITileSource Members

        public ITileProvider Provider
        {
            get { return tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return tileSchema; }
        }

        #endregion
    }
}
