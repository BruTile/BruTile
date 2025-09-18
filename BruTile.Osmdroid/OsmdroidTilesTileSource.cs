using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BruTile.Predefined;
using SQLite;
using SQLitePCL;

namespace BruTile.Osmdroid;

public class OsmdroidTilesTileSource : ILocalTileSource
{
    static OsmdroidTilesTileSource()
    {
        // Initialize Sqlite
        Batteries.Init();
    }

    private readonly SQLiteConnectionString _connectionString;

    public ITileSchema Schema { get; }
    public string Name { get; }
    public Attribution Attribution { get; set; }

    private readonly Dictionary<int, TileRange>? _tileRange;

    /// <summary>
    ///
    /// </summary>
    /// <param name="connectionString">The connection string to the mbtiles file</param>
    /// <param name="schema">The tile schema. Osmdroid doe not really have much of a schema, so this is here only because ITileSource defines it</param>
    /// <param name="zoomLevels">When 'zoomLevels' is null the zoom levels will be calculated. This can take a long time. Providing a list will speed up the process. </param>
    public OsmdroidTilesTileSource(SQLiteConnectionString connectionString, ITileSchema? schema = null, IEnumerable<int>? zoomLevels = null)
    {
        if (!File.Exists(connectionString.DatabasePath))
        {
            throw new FileNotFoundException($"The sqlite file does not exist: '{connectionString.DatabasePath}'", connectionString.DatabasePath);
        }

        _connectionString = connectionString;

        using var connection = new SQLiteConnection(connectionString);
        Schema = schema ?? ReadSchemaFromDatabase(connection, zoomLevels);

        var provider = GetProvider(connection);

        Attribution = new Attribution($"Source: {provider}"); // Osmdroid does not really provide this. We will just take the provider string and use that for now.
        Name = "Osmdroid"; // placeholder

        if (zoomLevels is null)
        {
            // The tile range should be based on the tiles actually present.
            var zoomLevelsFromDatabase = Schema.Resolutions.Select(r => r.Key);
            _tileRange = ReadTileRangeForEachLevelFromTilesTable(connection, zoomLevelsFromDatabase);
        }
    }

    private static GlobalSphericalMercator ReadSchemaFromDatabase(SQLiteConnection connection, IEnumerable<int>? zoomLevels)
    {
        zoomLevels ??= ReadZoomLevelsFromTilesTable(connection);

        var format = ReadFormat(connection);

        // we have no idea what the extent is and Osmdroid doesn't have that info available readily
        var extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);

        return new GlobalSphericalMercator(format.ToString(), YAxis.TMS, zoomLevels, extent: extent);
    }

    // we can determine what the min and max is fairly easily.
    private static int[]? ReadZoomLevelsFromTilesTable(SQLiteConnection connection)
    {
        var minIndex = ReadMinIndex(connection);
        int? zoomMin = minIndex is not null ? GetZoomLevel(minIndex.Value) : null;

        if (zoomMin == null) return null;

        var maxIndex = ReadMaxIndex(connection);
        int? zoomMax = maxIndex is not null ? GetZoomLevel(maxIndex.Value) : null;

        if (zoomMax == null) return null;

        var length = zoomMax.Value - zoomMin.Value + 1;
        var levels = new int[length];
        for (var i = 0; i < length; i++) levels[i] = i + zoomMin.Value;

        return levels;
    }

    private static OsmdroidTilesFormat ReadFormat(SQLiteConnection connection)
    {
        var sql = "SELECT \"tile\" FROM tiles ORDER BY key ASC LIMIT 1 ;";
        var bytes = connection.ExecuteScalar<byte[]>(sql);

        if (IsJpeg(bytes)) return OsmdroidTilesFormat.Jpeg;
        if (IsPng(bytes)) return OsmdroidTilesFormat.Png;

        return OsmdroidTilesFormat.Png; // we need a default
    }

    // Raw  : FF D8 FF DB
    // JFIF : FF D8 FF E0
    // EXIF : FF D8 FF E1
    private static bool IsJpeg(byte[] bytes) =>
        bytes is [0xFF, 0xD8, 0xFF, _, ..] && bytes[3] switch
        {
            0xDB => true,
            0xE0 => true,
            0xE1 => true,
            _ => false
        };

    // PNG header = 137 80 78 71 13 10 26 10
    private static bool IsPng(byte[] bytes) => bytes is [137, 80, 78, 71, 13, 10, 26, 10, ..];

    // this takes a XYZ style index and generates a Osmdroid key
    public static long ToOsmdroidKey(int pZoom, int pX, int pY)
    {
        var z = (int)(Math.Pow(2, pZoom));
        var y = pY + 1;
        var mY = z - y;

        return (((long)pZoom) << (pZoom * 2)) + (((long)pX) << pZoom) + mY;
    }

    public async Task<byte[]?> GetTileAsync(TileInfo tileInfo)
    {
        var index = tileInfo.Index;

        if (IsTileIndexValid(index))
        {
            byte[] result;
            var cn = new SQLiteAsyncConnection(_connectionString);
            {
                var key = ToOsmdroidKey(index.Level, index.Col, index.Row);
                System.Diagnostics.Debug.WriteLine($"{index.Level}, {index.Row}, {index.Col} => {key}");
                var sql = "SELECT tile FROM \"tiles\" WHERE  key=?;";
                result = await cn.ExecuteScalarAsync<byte[]>(sql, key)
                    .ConfigureAwait(false);
            }

            return result == null || result.Length == 0
                ? null
                : result;
        }

        return null;
    }

    private bool IsTileIndexValid(TileIndex index)
    {
        if (_tileRange == null)
            return true;

        // This is an optimization that makes use of an additional 'map' table which is not part of the spec
        if (_tileRange.TryGetValue(index.Level, out var tileRange))
            return
                tileRange.FirstCol <= index.Col &&
                index.Col <= tileRange.LastCol &&
                tileRange.FirstRow <= index.Row &&
                index.Row <= tileRange.LastRow;
        return false;
    }

    /// <summary>
    /// Gets the minimum key in the database
    /// </summary>
    /// <param name="connection">a valid connection to an osmdroid sqlite format database</param>
    /// <returns>a key, or null</returns>
    private static long? ReadMinIndex(SQLiteConnection connection)
    {
        const string sql = "SELECT min(key) FROM \"tiles\"";
        try
        {
            return connection.ExecuteScalar<long?>(sql);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the maximum key in the database
    /// </summary>
    /// <param name="connection">a valid connection to an osmdroid sqlite format database</param>
    /// <returns>a key, or null</returns>
    private static long? ReadMaxIndex(SQLiteConnection connection)
    {
        const string sql = "SELECT max(key) FROM \"tiles\"";
        try
        {
            return connection.ExecuteScalar<long?>(sql);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// This takes a calculates a zoom level from a given index by brute force.
    /// </summary>
    /// <param name="index">the "key" field value from the tiles table</param>
    /// <returns>A zoom level when one is matched, else -1</returns>
    public static int GetZoomLevel(long index)
    {
        for (var i = 0; i < 19; i++)
        {
            if ((index >> (i * 2)) == i)
            {
                return i;
            }
        }

        return -1;
    }

    private static string? GetProvider(SQLiteConnection connection)
    {
        const string sql = "SELECT provider FROM tiles ORDER BY key ASC LIMIT 1 ;";
        try
        {
            return connection.ExecuteScalar<string?>(sql);
        }
        catch (Exception)
        {
            return null;
        }
    }

    // this is identical to the MbTiles version because it is essentially the same data
    private sealed class ZoomLevelMinMax
    {
        public int TileRowMin { get; set; }
        public int TileRowMax { get; set; }
        public int TileColMin { get; set; }
        public int TileColMax { get; set; }

        public TileRange ToTileRange()
        {
            return new TileRange(TileColMin, TileRowMin, TileColMax - TileColMin + 1, TileRowMax - TileRowMin + 1);
        }
    }

    // This is probably even more of a hack than the MbTiles version.
    [Table("tiles")]
    private sealed class KeyQuery // I would rather just user 'int' instead of this class in Query, but can't get it to work
    {
        [Column("key")]
        public int Key { get; set; }

        private int? zoomLevel;
        [Ignore]
        public int ZoomLevel => zoomLevel ??= GetZoomLevel(Key);

        private int? modulo;
        [Ignore]
        public int Modulo => modulo ??= 1 << ZoomLevel;

        private int? x;
        [Ignore]
        public int X => x ??= ((Key >> ZoomLevel) % Modulo);

        private int? y;
        [Ignore]
        public int Y => y ??= (Key % Modulo);
    }

    private static Dictionary<int, TileRange> ReadTileRangeForEachLevelFromTilesTable(SQLiteConnection connection, IEnumerable<int> zoomLevels)
    {
        var tileRange = new Dictionary<int, TileRange>();

        // we need to do this differently than the way MbTiles does it. I don't know if this is optimised, but it does work.

        // get all the keys
        var sql = "select key from tiles;";

        var enumerable = zoomLevels as int[] ?? zoomLevels.ToArray();
        var query = connection.Query<KeyQuery>(sql);

        foreach (var zoomLevel in enumerable)
        {
            // we now need to calc the extents
            var rangeForLevel = query
                .Where(x => x.ZoomLevel == zoomLevel)
                .GroupBy(key => key.ZoomLevel)
                .Select(group => new ZoomLevelMinMax
                {
                    TileColMin = group.Min(t => t.X),
                    TileColMax = group.Max(t => t.X),
                    TileRowMin = group.Min(t => t.Y),
                    TileRowMax = group.Max(t => t.Y)
                }).FirstOrDefault();
            if (rangeForLevel is not null)
            {
                tileRange.Add(zoomLevel, rangeForLevel.ToTileRange());
            }
        }

        return tileRange;
    }
}
