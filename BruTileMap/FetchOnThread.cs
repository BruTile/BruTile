using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;

namespace BruTileMap
{
  class FetchOnThread
  {
    ITileProvider tileProvider;
    TileInfo tileInfo;
    FetchCompletedEventHandler fetchCompleted;

    public FetchOnThread(ITileProvider tileProvider, TileInfo tileInfo, FetchCompletedEventHandler fetchCompleted)
    {
      this.tileProvider = tileProvider;
      this.tileInfo = tileInfo;
      this.fetchCompleted = fetchCompleted;
    }

    public void FetchTile()
    {
      Exception error = null;
      byte[] image = null;

      try
      {
          image = tileProvider.GetTile(tileInfo);
      }
      catch (Exception ex) //This may seem a bit weird. We catch the exception to pass it as an argument. This is because we are on a worker thread here, we cannot just let it fall through. 
      {
        error = ex;
      }
      this.fetchCompleted(this, new FetchCompletedEventArgs(error, false, tileInfo, image)); 
    }
  }
}
