// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
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
            Func<Uri, byte[]> fetchTile = null)
        {
            var reader = new StreamReader(tileMapResource);
            var serializer = new XmlSerializer(typeof(TileMap));
            var tileMap = (TileMap)serializer.Deserialize(reader);
            var tileSchema = CreateSchema(tileMap);
            
            var tileUrls = new Dictionary<string, Uri>();
            foreach (TileMapTileSetsTileSet ts in tileMap.TileSets.TileSet)
            {
                tileUrls[ts.order] = new Uri(ts.href);
            }
            var tileProvider = new WebTileProvider(CreateRequest(tileUrls, tileSchema.Format, overrideUrl, customParameters),
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
            if (callback != null) callback(tileSource, error);            
        }

        private static TileSchema CreateSchema(TileMap tileMap)
        {
            var schema = new TileSchema();
            schema.OriginX = double.Parse(tileMap.Origin.x, CultureInfo.InvariantCulture);
            schema.OriginY = double.Parse(tileMap.Origin.y, CultureInfo.InvariantCulture);
            schema.Srs = tileMap.SRS;
            schema.Width = int.Parse(tileMap.TileFormat.width);
            schema.Height = int.Parse(tileMap.TileFormat.height);
            schema.Name = tileMap.Title;
            schema.Format = tileMap.TileFormat.extension;
            schema.Axis = AxisDirection.Normal;
            schema.Extent = new Extent(
                double.Parse(tileMap.BoundingBox.minx, CultureInfo.InvariantCulture),
                double.Parse(tileMap.BoundingBox.miny, CultureInfo.InvariantCulture),
                double.Parse(tileMap.BoundingBox.maxx, CultureInfo.InvariantCulture),
                double.Parse(tileMap.BoundingBox.maxy, CultureInfo.InvariantCulture));

  
            foreach (var tileSet in tileMap.TileSets.TileSet)
            {
                double resolution = double.Parse(tileSet.unitsperpixel, CultureInfo.InvariantCulture);
                schema.Resolutions[tileSet.order] = new Resolution { Id = tileSet.order, UnitsPerPixel = resolution };
            }
            return schema;
        }

        private static IRequest CreateRequest(IDictionary<string, Uri> tileUrls, string format, string overrideUrl,
            Dictionary<string, string> customParameters = null)
        {
            if (string.IsNullOrEmpty(overrideUrl))
                return new TmsRequest(tileUrls, format, customParameters);

            return new TmsRequest(new Uri(overrideUrl), format, customParameters);
        }
    }
}
