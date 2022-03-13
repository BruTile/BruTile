// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BruTile.Cache;
using BruTile.Web;

namespace BruTile.Tms
{
    public static class TileMapParser
    {
        public delegate void CreateTileSourceCompleted(ITileSource tileSource, Exception error);

        public static TileSource CreateTileSource(Stream tileMapResource, string overrideUrl = null,
            Dictionary<string, string> customParameters = null, IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, Task<byte[]>> fetchTile = null)
        {
            var reader = new StreamReader(tileMapResource);
            var serializer = new XmlSerializer(typeof(TileMap));
            var tileMap = (TileMap)serializer.Deserialize(reader);
            var tileSchema = CreateSchema(tileMap);

            var tileUrls = new Dictionary<int, Uri>();
            foreach (TileMapTileSetsTileSet ts in tileMap.TileSets.TileSet)
            {
                tileUrls[int.Parse(ts.order)] = new Uri(ts.href);
            }
            var tileProvider = new HttpTileProvider(CreateRequest(tileUrls, tileSchema.Format, overrideUrl, customParameters),
                persistentCache, fetchTile);

            return new TileSource(tileProvider, tileSchema);
        }

        public static void CreateTileSourceAsync(string tileMapResourceUrl, CreateTileSourceCompleted callback)
        {
            CreateTileSourceAsync(tileMapResourceUrl, callback, null);
        }

        public static void CreateTileSourceAsync(string url, CreateTileSourceCompleted callback, string overrideUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));

            request.BeginGetResponse(MethodOnThread,
                new object[] { callback, request, overrideUrl });
        }

        public static void MethodOnThread(IAsyncResult ar)
        {
            CreateTileSourceCompleted callback = null;
            Exception error = null;
            ITileSource tileSource = null;

            try
            {
                var args = (object[])ar.AsyncState;
                callback = (CreateTileSourceCompleted)args[0];
                var request = (HttpWebRequest)args[1];
                var overrideUrl = (string)args[2];

                var response = request.EndGetResponse(ar);
                tileSource = CreateTileSource(response.GetResponseStream(), overrideUrl);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            callback?.Invoke(tileSource, error);
        }

        private static TileSchema CreateSchema(TileMap tileMap)
        {
            var schema = new TileSchema
            {
                OriginX = double.Parse(tileMap.Origin.x, CultureInfo.InvariantCulture),
                OriginY = double.Parse(tileMap.Origin.y, CultureInfo.InvariantCulture),
                Srs = tileMap.SRS,
                Name = tileMap.Title,
                Format = tileMap.TileFormat.extension,
                YAxis = YAxis.TMS,
                Extent = new Extent(
                    double.Parse(tileMap.BoundingBox.minx, CultureInfo.InvariantCulture),
                    double.Parse(tileMap.BoundingBox.miny, CultureInfo.InvariantCulture),
                    double.Parse(tileMap.BoundingBox.maxx, CultureInfo.InvariantCulture),
                    double.Parse(tileMap.BoundingBox.maxy, CultureInfo.InvariantCulture))
            };

            foreach (var tileSet in tileMap.TileSets.TileSet)
            {
                var unitsPerPixel = double.Parse(tileSet.unitsperpixel, CultureInfo.InvariantCulture);
                var level = int.Parse(tileSet.order);
                schema.Resolutions[level] = new Resolution
                (
                    level,
                    unitsPerPixel,
                    int.Parse(tileMap.TileFormat.width),
                    int.Parse(tileMap.TileFormat.height)
                );
            }

            return schema;
        }

        private static IRequest CreateRequest(IDictionary<int, Uri> tileUrls, string format, string overrideUrl,
            Dictionary<string, string> customParameters = null)
        {
            if (string.IsNullOrEmpty(overrideUrl))
                return new TmsRequest(tileUrls, format, customParameters);

            return new TmsRequest(new Uri(overrideUrl), format, customParameters);
        }
    }
}
