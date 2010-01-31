using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;

namespace BruTile.UI.Fetcher
{
  class FetchOnThread
  {
    ITileProvider tileProvider;
    TileInfo tileInfo;
    DataChangedEventHandler dataChanged;

    public FetchOnThread(ITileProvider tileProvider, TileInfo tileInfo, DataChangedEventHandler fetchCompleted)
    {
      this.tileProvider = tileProvider;
      this.tileInfo = tileInfo;
      this.dataChanged = fetchCompleted;
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
      this.dataChanged(this, new DataChangedEventArgs(error, false, tileInfo, image)); 
    }
  }
}
