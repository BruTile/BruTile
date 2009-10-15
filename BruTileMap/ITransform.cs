using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;

namespace BruTileMap
{
  public interface ITransform
  {
    BTPoint WorldToMap(double x, double y);
    BTPoint MapToWorld(double x, double y);
    double Resolution { get; }
    Extent Extent { get; }
  }
}
