// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Tim Ebben (Geodan) 2009

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BruTile.Cache;
using BruTile.Web;

namespace BruTile.Tms;

public static class TileMapParser
{
    public delegate void CreateTileSourceCompleted(ITileSource? tileSource, Exception? error);

    public static HttpTileSource CreateTileSource(Stream tileMapResource, string? overrideUrl = null,
        Dictionary<string, string>? customParameters = null, IPersistentCache<byte[]>? persistentCache = null,
        Action<HttpRequestMessage>? configureHttpRequestMessage = null)
    {
        var reader = new StreamReader(tileMapResource);
        var serializer = new XmlSerializer(typeof(TileMap))
            ?? throw new InvalidOperationException("Failed to create XmlSerializer");
        var tileMap = (TileMap?)serializer.Deserialize(reader)
            ?? throw new InvalidOperationException("Failed to deserialize TileMap");
        var tileSchema = CreateSchema(tileMap);

        var tileUrls = new Dictionary<int, Uri>();
        foreach (var ts in tileMap.TileSets.TileSet)
        {
            tileUrls[int.Parse(ts.order)] = new Uri(ts.href);
        }
        var urlBuilder = CreateUrlBuilder(tileUrls, tileSchema.Format, overrideUrl, customParameters);
        var httpTileSource = new HttpTileSource(tileSchema, urlBuilder, persistentCache: persistentCache,
            configureHttpRequestMessage: configureHttpRequestMessage);

        return httpTileSource;
    }

    public static void CreateTileSourceAsync(string tileMapResourceUrl, CreateTileSourceCompleted callback)
    {
        CreateTileSourceAsync(tileMapResourceUrl, callback, null);
    }

    public static void CreateTileSourceAsync(string url, CreateTileSourceCompleted callback, string? overrideUrl)
    {
        var httpClientHandler = new HttpClientHandler();
        var httpClient = new HttpClient(httpClientHandler);

        _ = Task.Run(async () =>
        {
            HttpTileSource? httpTileSource = null;
            Exception? error = null;
            try
            {
                var stream = await httpClient.GetStreamAsync(new Uri(url)).ConfigureAwait(false);
                httpTileSource = CreateTileSource(stream, overrideUrl);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback?.Invoke(httpTileSource, error);

        });
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

    private static TmsUrlBuilder CreateUrlBuilder(IDictionary<int, Uri> tileUrls, string format, string? overrideUrl = null,
        Dictionary<string, string>? customParameters = null)
    {
        if (string.IsNullOrEmpty(overrideUrl))
            return new TmsUrlBuilder(tileUrls, format, customParameters);

        return new TmsUrlBuilder(new Uri(overrideUrl), format, customParameters);
    }
}
