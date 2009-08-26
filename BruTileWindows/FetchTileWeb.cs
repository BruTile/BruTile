using System;
using System.Collections.Generic;
using System.Text;
using Tiling;
using System.Net;

namespace BruTileMap
{
  public class FetchTileWeb : IFetchTile
  {
    IRequestBuilder requestBuilder;

    public FetchTileWeb(IRequestBuilder requestBuilder)
    {
      this.requestBuilder = requestBuilder;
    }

    #region TileProvider Members

    public void GetTile(TileInfo tileInfo, FetchCompletedEventHandler fetchCompleted)
    {
      WebClient webClient = new WebClient();
      webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
      webClient.OpenReadAsync(requestBuilder.GetUrl(tileInfo), new AsyncEventArgs() { TileInfo = tileInfo, FetchCompleted = fetchCompleted } );
    }

    private void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
    {
      //TODO rewrite this
      AsyncEventArgs state = (AsyncEventArgs)e.UserState;
      Exception exception = null;
      byte[] bytes = null;

      if (e.Error != null || e.Cancelled)
      {
        FetchCompletedEventArgs args1 = new FetchCompletedEventArgs(e.Error, e.Cancelled, state.TileInfo, null);
        state.FetchCompleted(this, args1);
        return;
      }
      try
      {
        bytes = Tiling.Util.ReadFully(e.Result);
      }
      catch (Exception ex)
      {
        exception = ex;
      }
        
      FetchCompletedEventArgs args = new FetchCompletedEventArgs(exception, e.Cancelled, state.TileInfo, bytes);
      state.FetchCompleted(this, args);
    }

    private class AsyncEventArgs
    {
      public TileInfo TileInfo { get; set; }
      public FetchCompletedEventHandler FetchCompleted  { get; set; }
    }

    #endregion
  }
}
