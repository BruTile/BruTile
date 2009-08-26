using System;
using BruTile;
using BruTileMap;

namespace BruTileDemo
{
  class FetchTileWeb : IFetchTile
  {
    IRequestBuilder requestBuilder;
    
    public FetchTileWeb(IRequestBuilder requestBuilder)
    {
      if (requestBuilder == null) throw new ArgumentException("RequestBuilder can not be null");
      this.requestBuilder = requestBuilder;
    }

    #region IFetchTile Members

    public void GetTile(TileInfo tileInfo, FetchCompletedEventHandler fetchCompleted)
    {
      Exception error = null;
      byte[] bytes = null;

      try
      {
        bytes = ImageRequest.GetImageFromServer(requestBuilder.GetUrl(tileInfo));
      }
      catch (Exception ex) //This may seem a bit weird. We catch the exception to pass it as an argument. This is because we are on a worker thread here, we cannot just let it fall through. 
      {
        error = ex;
      }
      fetchCompleted(this, new FetchCompletedEventArgs(error, false, tileInfo, bytes)); 

    }

    #endregion
  }
}
