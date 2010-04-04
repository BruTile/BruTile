using System;
using BruTile;
using System.IO;
using BruTile.Cache;

namespace BruTile.FileSystem
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

            bytes = fileCache.Find(tileInfo.Index);
            if (bytes == null) throw new FileNotFoundException("The tile was not found at it's expected location");
            return bytes;
        }

    }
}
