using System;
using System.Collections.Generic;
using System.Text;

namespace Tiling
{
  public class WebTileProvider : ITileProvider
  {
    IRequestBuilder requestBuilder;
    
    public WebTileProvider(IRequestBuilder requestBuilder)
    {
      this.requestBuilder = requestBuilder;
    }

    #region TileProvider Members

    public byte[] GetTile(TileInfo tileInfo)
    {
      Uri requestUrl = requestBuilder.GetUrl(tileInfo);
      return ImageRequest.GetImageFromServer(requestUrl);
    }

    #endregion
  }
}
