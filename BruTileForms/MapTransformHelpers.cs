using BruTileMap;

namespace BruTileForms
{
  public static class MapTransformHelpers
  {
    public static void Pan(MapTransform transform, BTPoint currentMap, BTPoint previousMap)
    {
      BTPoint current = transform.MapToWorld(currentMap.X, currentMap.Y);
      BTPoint previous = transform.MapToWorld(previousMap.X, previousMap.Y);
      float diffX = previous.X - current.X;
      float diffY = previous.Y - current.Y;
      transform.Center = new BTPoint(transform.Center.X + diffX, transform.Center.Y + diffY);
    }
  }
}
