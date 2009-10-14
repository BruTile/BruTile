using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;

namespace BruTileMap
{
  public interface ITransform
  {
    PointF WorldToMap(double x, double y);
    PointF MapToWorld(double x, double y);
    double Resolution { get; }
    Extent Extent { get; }
  }
}
