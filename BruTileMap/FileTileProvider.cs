using System;
using BruTile;
using System.IO;

namespace BruTileMap
{
    public class FileTileProvider : ITileProvider
    {
        FileCache fileCache;

        public FileTileProvider(FileCache fileCache)
        {
            this.fileCache = fileCache;
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = null;

            bytes = fileCache.Find(tileInfo.Key);
            if (bytes == null) throw new FileNotFoundException("The tile was not found at it's expected location");
            return bytes;
        }

    }
}
