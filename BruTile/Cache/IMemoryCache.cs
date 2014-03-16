namespace BruTile.Cache
{
    interface IMemoryCache<T> : ITileCache<T>
    {
        int MinTiles { get; set; }
        int MaxTiles { get; set; }
    }
}
