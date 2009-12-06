using SharpMap;
using SharpMap.Data.Providers;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Styles;
using BruTile;
using System;
using BruTile.Cache;

namespace SharpMapProvider
{
    public class SharpMapTileProvider : ITileProvider
    {
        #region fields

        Map map;
        object syncRoot = new object();
        ITileCache<byte[]> fileCache;

        #endregion

        #region Public Methods

        public SharpMapTileProvider(Map map)
            : this(map, new NullCache())
        {
        }

        public SharpMapTileProvider(Map map, ITileCache<byte[]> fileCache)
        {
            this.map = map;

            if (fileCache == null) throw new ArgumentException("File can not be null");

            this.fileCache = fileCache;
        }

        #endregion

        #region Private Methods

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = null;
            bytes = fileCache.Find(tileInfo.Key);
            if (bytes == null)
            {
                lock (syncRoot)
                {
                    Extent extent = tileInfo.Extent;
                    map.ZoomToBox(new BoundingBox(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY));
                    bytes = map.GetMapAsByteArray();
                    if (bytes != null)
                        fileCache.Add(tileInfo.Key, bytes);
                }
            }
            return bytes;
        }

        #endregion

        #region Private classes

        private class NullCache : ITileCache<byte[]>
        {
            public NullCache()
            {
            }

            public void Add(TileKey key, byte[] image)
            {
                //do nothing
            }

            public void Remove(TileKey key)
            {
                throw new NotImplementedException(); //and should not
            }

            public byte[] Find(TileKey key)
            {
                return null;
            }
        }

        #endregion
    }
}
