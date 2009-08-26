using System;
using System.Collections.Generic;
using System.Text;
using Tiling;

namespace BruTileMap
{
  public delegate void FetchCompletedEventHandler(object sender, FetchCompletedEventArgs e);

  public interface IFetchTile
  {
    void GetTile(TileInfo tileInfo, FetchCompletedEventHandler fetchCompleted);
  }

  public class FetchCompletedEventArgs
  {
    public FetchCompletedEventArgs(Exception error, bool cancelled, TileInfo tileInfo, byte[] image)
    {
      this.Error = error;
      this.Cancelled = cancelled;
      this.TileInfo = tileInfo;
      this.Image = image;
    }

    public Exception Error;
    public bool Cancelled;
    public TileInfo TileInfo;
    public byte[] Image;
  }

}
