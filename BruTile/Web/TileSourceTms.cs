using System;
using System.Collections.Generic;
using System.Text;
using BruTile;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using System.Net;

namespace BruTile.Web
{
    public class TileSourceTms : ITileSource
    {
        TileSchema tileSchema;
        WebTileProvider tileProvider;
        string overwriteTileURL; //When the configfile points to an invalid tile url fill this one

        public TileSourceTms(string configUrl)
        {
            Create(configUrl);
        }

        public TileSourceTms(string configUrl, string overwriteTileURL)
        {
            this.overwriteTileURL = overwriteTileURL;
            Create(configUrl);
        }

        public TileSourceTms(TileSchema tileSchema)
        {
            this.tileSchema = tileSchema;

        }

        private void Create(string url)
        {
            //Get stream from url
            MemoryStream stream = CreateStream(url);
            StreamReader reader = new StreamReader(stream);
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

        private MemoryStream CreateStream(String url)
        {
            MemoryStream memStream;
            //TODO: If no internet available try load saved configs
            WebClient client = new WebClient();
            try { memStream = new MemoryStream(client.DownloadData(url)); }
            catch (Exception e) { return null; }
            client.Dispose();
            return memStream;
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
            //TODO: Send parameters with config
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("seriveparam", "ortho10");
            IRequestBuilder request;

            if (overwriteTileURL != null)
                request = new RequestTms(new Uri(overwriteTileURL), Schema.Format, null);
            else
                request = new RequestTms(tileUrls, Schema.Format, parameters);

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
