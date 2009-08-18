
namespace BruTileMap
{
  public interface ITileFactory<T>
  {
    T GetTile(byte[] bytes);
  }
}
