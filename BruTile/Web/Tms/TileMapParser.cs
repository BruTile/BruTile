using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace BruTile.Web.TmsService
{
    public static class TileMapParser
    {
        public delegate void CreateTileSourceCompleted(ITileSource tileSource, Exception error);

        public static ITileSource CreateTileSource(Stream tileMapResource)
        {
            return CreateTileSource(tileMapResource, null);
        }

        public static TileSource CreateTileSource(Stream tileMapResource, string overrideUrl)
        {
            var reader = new StreamReader(tileMapResource);
            var serializer = new XmlSerializer(typeof(TileMap));
            var tileMap = (TileMap)serializer.Deserialize(reader);
            var tileSchema = CreateSchema(tileMap);

            var tileUrls = new List<Uri>();
            foreach (TileMapTileSetsTileSet ts in tileMap.TileSets.TileSet)
            {
                tileUrls.Add(new Uri(ts.href));
            }
            var tileProvider = new WebTileProvider(CreateRequest(tileUrls, tileSchema.Format, overrideUrl));

            return new TileSource(tileProvider, tileSchema);
        }

#if !PocketPC
        public static void CreateTileSourceAsync(string tileMapResourceUrl, CreateTileSourceCompleted callback)
        {
            CreateTileSourceAsync(tileMapResourceUrl, callback, null);
        }

        public static void CreateTileSourceAsync(string url, CreateTileSourceCompleted callback, string overrideUrl)
        {
            var client = new WebClient();
            client.OpenReadCompleted +=
                delegate(object s, OpenReadCompletedEventArgs eventArgs)
                {
                    if ((!eventArgs.Cancelled) && (eventArgs.Error == null))
                    {
                        var tileSource = CreateTileSource(eventArgs.Result, overrideUrl);
                        if (callback != null) callback(tileSource, eventArgs.Error);
                    }
                };
            client.OpenReadAsync(new Uri(url));
        }
#endif

        private static TileSchema CreateSchema(TileMap tileMap)
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
            schema.Extent = new Extent(
                Double.Parse(tileMap.BoundingBox.minx, CultureInfo.InvariantCulture),
                Double.Parse(tileMap.BoundingBox.miny, CultureInfo.InvariantCulture),
                Double.Parse(tileMap.BoundingBox.maxx, CultureInfo.InvariantCulture),
                Double.Parse(tileMap.BoundingBox.maxy, CultureInfo.InvariantCulture));
            for (int i = 0; i < tileMap.TileSets.TileSet.Length; i++)
            {
                double resolution = Double.Parse(tileMap.TileSets.TileSet[i].unitsperpixel, CultureInfo.InvariantCulture);
                schema.Resolutions.Add(resolution);
            }
            return schema;
        }

        private static IRequest CreateRequest(IList<Uri> tileUrls, string format, string overrideUrl)
        {
            if (string.IsNullOrEmpty(overrideUrl))
                return new TmsRequest(tileUrls, format, new Dictionary<string, string>());
            else
                return new TmsRequest(new Uri(overrideUrl), format, null);
        }
    }
}
