using System;
using System.Collections.Generic;
using System.Text;

namespace BruTile
{
  public interface ITileProvider
  {
    byte[] GetTile(TileInfo tileInfo);
  }
}
