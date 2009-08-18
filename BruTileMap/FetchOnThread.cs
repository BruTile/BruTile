using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Tiling;
using System.Threading;

namespace BruTileMap
{
  class FetchOnThread
  {
    ITileProvider tileProvider;
    TileInfo tileInfo;
    FetchCompletedEventHander fetchCompleted;
    ITileCache<byte[]> fileCache;

    public FetchOnThread(ITileProvider tileProvider, TileInfo tileInfo, ITileCache<byte[]> fileCache, FetchCompletedEventHander fetchCompleted)
    {
      this.tileProvider = tileProvider;
      this.tileInfo = tileInfo;
      this.fetchCompleted = fetchCompleted;
      this.fileCache = fileCache;
    }

    public void FetchTile()
    {
      Exception error = null;
      byte[] image = null;

      try
      {
        image = fileCache.Find(tileInfo.Key);
        if (image == null)
        {
          image = tileProvider.GetTile(tileInfo);
          fileCache.Add(tileInfo.Key, image);
        }
      }
      catch (Exception ex) //This may seem a bit weird. We catch the exception to pass it as an argument. This is because we are on a worker thread here, we cannot just let it fall through. 
      {
        error = ex;
      }
      fetchCompleted(this, new FetchCompletedEventArgs(error, false, tileInfo, image)); 
    }
  }
}
