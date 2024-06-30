using BruTile.Predefined;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BruTile.Osmdroid;

public class OsmdroidTilesTileSource : ITileSource
{
    private readonly SQLiteConnectionString _connectionString;

    public ITileSchema Schema { get; }
    public string Name { get; }
    public Attribution Attribution { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="connectionString">The connection string to the mbtiles file</param>
    /// <param name="schema">The tile schema. Osmdroid doe not really have much of a schema, so this is here only because ITileSource defines it</param>
    /// <param name="determineZoomLevelsFromTilesTable">When 'determineZoomLevelsFromTilesTable' is true the zoom levels
    /// will be determined from the available tiles in the 'tiles' table. This operation can take long if there are many tiles in
    /// the 'tiles' table. When 'determineZoomLevelsFromTilesTable' is false the zoom levels will be read from the metadata table
    ///(by reading 'zoomMin' and 'zoomMax'). If there are no zoom levels specified in the metadata table the GlobalSphericalMercator
    ///default levels are assumed. This parameter will have no effect if the schema is passed in as argument. The default is false.</param>
    public OsmdroidTilesTileSource(SQLiteConnectionString connectionString, ITileSchema? schema = null, IEnumerable<int>? zoomLevels = null)
    {
        if (!File.Exists(connectionString.DatabasePath))
        {
            throw new FileNotFoundException($"The sqlite file does not exist: '{connectionString.DatabasePath}'", connectionString.DatabasePath);
        }

        _connectionString = connectionString;

        using var connection = new SQLiteConnection(connectionString);
        Schema = schema ?? ReadSchemaFromDatabase(connection, zoomLevels);

        Attribution = new Attribution(); // TODO - need to work out what to do with this (use the provider string?)
        Name = "OSMDroid"; // placeholder
    }


    private static ITileSchema ReadSchemaFromDatabase(SQLiteConnection connection, IEnumerable<int>? zoomLevels)
    {
        if (zoomLevels is null)
        {
            zoomLevels = ReadZoomLevelsFromTilesTable(connection);
        }

        var format = ReadFormat(connection);

        // we have no idea what the extent is and Osmdroid doesn't have that info available readily
        var extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);

        return new GlobalSphericalMercator(format.ToString(), YAxis.TMS, zoomLevels, extent: extent);
    }

    // we can determine what the min and max is fairly easily.
    private static IEnumerable<int>? ReadZoomLevelsFromTilesTable(SQLiteConnection connection)
    {
        // var zoomMin = ReadInt(connection, "minzoom");
        // if (zoomMin == null) return null;
        // var zoomMax = ReadInt(connection, "maxzoom");
        // if (zoomMax == null) return null;
        //
        // var length = zoomMax.Value - zoomMin.Value + 1;
        // var levels = new int[length];
        // for (var i = 0; i < length; i++) levels[i] = i + zoomMin.Value;
        //
        // return levels;
        return null; // TODO - we can probably work this out from the Osmdroid source
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
}