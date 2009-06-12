using System;
using System.Collections.Generic;
using System.Text;

namespace Tiling
{
  public interface ITileProvider
  {
    byte[] GetTile(TileInfo tileInfo);
  }
}
