using System.IO;
using BruTile.Cache;

namespace BruTile.FileSystem
{
    public class FileTileProvider : ITileProvider
    {
        readonly FileCache _fileCache;

        public FileTileProvider(FileCache fileCache)
        {
            _fileCache = fileCache;
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = _fileCache.Find(tileInfo.Index);
            if (bytes == null) throw new FileNotFoundException("The tile was not found at it's expected location");
            return bytes;
        }

    }
}
