using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BruTileMap
{
  public interface ITileFactory<T>
  {
    T GetTile(byte[] bytes);
  }
}
