// Copyright 2012 - Paul den Dulk (Geodan)

using BruTile.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace BruTile.Metro
{
    public static class TileLayer
    {        
        public static IEnumerable<Tile> GetTilesInView(Extent extent, double resolution,
            ITileSchema schema, ITileCache<Image> tileCache)
        {
            var dictionary = new Dictionary<TileIndex, Tile>();

            if (schema == null) return dictionary.Values;

            GetRecursive(dictionary, schema, tileCache, extent, BruTile.Utilities.GetNearestLevel(schema.Resolutions, resolution));
            var sortedDictionary = (from entry in dictionary orderby entry.Key ascending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            return sortedDictionary.Values;
        }

        public static void GetRecursive(IDictionary<TileIndex, Tile> resultTiles, ITileSchema schema, ITileCache<Image> cache, Extent extent, int level)
        {
            if (level < 0) return;

            var tiles = schema.GetTilesInView(extent, level);

            foreach (TileInfo tileInfo in tiles)
            {
                var feature = cache.Find(tileInfo.Index);
                if (feature == null)
                {
                    GetRecursive(resultTiles, schema, cache, tileInfo.Extent.Intersect(extent), level - 1);
                }
                else
                {
                    resultTiles[tileInfo.Index] = new Tile { Image = feature, Info = tileInfo };
                    if (!IsFullyShown(feature))
                    {
                        GetRecursive(resultTiles, schema, cache, tileInfo.Extent.Intersect(extent), level - 1);
                    }
                }
            }
        }

        public static bool IsFullyShown(Image tile)
        {
            if (tile.Tag == null) return false; // not yet shown at all
            var currentTile = DateTime.Now.Ticks;
            const long second = 10000000;
            return ((currentTile - (long)tile.Tag) > second);
        }
    }
}
