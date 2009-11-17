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

        public void GetTile(TileInfo tileInfo, FetchCompletedEventHandler fetchCompleted)
        {
            Exception exception = null;
            byte[] bytes = null;

            try
            {
                bytes = fileCache.Find(tileInfo.Key);
                if (bytes == null) exception = new FileNotFoundException("The tile was not found at it's expected location");
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            fetchCompleted(this, new FetchCompletedEventArgs(exception, false, tileInfo, bytes));
        }
    }
}
