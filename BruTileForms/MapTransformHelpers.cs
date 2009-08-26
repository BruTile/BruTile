using System.Windows;
using BruTileMap;

namespace BruTileForms
{
  public static class MapTransformHelpers
  {
    public static void Pan(MapTransform transform, PointF currentMap, PointF previousMap)
    {
      PointF current = transform.MapToWorld(currentMap.X, currentMap.Y);
      PointF previous = transform.MapToWorld(previousMap.X, previousMap.Y);
      float diffX = previous.X - current.X;
      float diffY = previous.Y - current.Y;
      transform.Center = new PointF(transform.Center.X + diffX, transform.Center.Y + diffY);
    }
  }
}
