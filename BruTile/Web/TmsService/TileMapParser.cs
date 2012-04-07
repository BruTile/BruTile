#region License

// Copyright 2008 - Paul den Dulk (Geodan)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

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

        public static TileSource CreateTileSource(Stream tileMapResource, string overrideUrl,
            Dictionary<string, string> customParameters = null)
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
            var tileProvider = new WebTileProvider(CreateRequest(tileUrls, tileSchema.Format, overrideUrl, customParameters));

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

            foreach (TileMapTileSetsTileSet tileSet in tileMap.TileSets.TileSet)
            {
                double resolution = double.Parse(tileSet.unitsperpixel, CultureInfo.InvariantCulture);
                schema.Resolutions.Add(new Resolution { Id = tileSet.order, UnitsPerPixel = resolution });
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
