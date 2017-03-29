// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System;
using System.Collections.Generic;
using System.Globalization;
using BruTile.Predefined;
using SQLite;

namespace BruTile
{
    /// <summary>
    /// An <see cref="ITileSource"/> implementation for MapBox Tiles files
    /// </summary>
    /// <seealso href="https://www.mapbox.com/developers/mbtiles/"/>
    public class MbTilesTileSource : ITileSource
    {
        public MbTilesType Type { get; }

        private readonly SQLiteConnectionString _connectionString;
        private const string MetadataSql = "SELECT \"value\" FROM metadata WHERE \"name\"=?;";
        private Dictionary<string, int[]> _tileRange;

        public MbTilesTileSource(SQLiteConnectionString connectionString, ITileSchema schema = null, MbTilesType type = MbTilesType.None)
        {
            _connectionString = connectionString;
            var connection = new SQLiteConnectionWithLock(connectionString, SQLiteOpenFlags.ReadOnly);
            using (connection.Lock())
            {
                Type = type == MbTilesType.None ? ReadType(connection) : type;

                if (schema != null)
                {
                    Schema = schema;
                }
                else
                {
                    Schema = ReadSchemaFromDatabase(connection);
                }
            }
        }

        private ITileSchema ReadSchemaFromDatabase(SQLiteConnectionWithLock connection)
        {

            var format = ReadFormat(connection);

            var extent = ReadExtent(connection);

            if (HasMapTable(connection))
            {
                // it is possible to override the schema by definining it in a 'map' table.
                // This method depends on reading tiles from an 'images' table, which
                // is not part of the MBTiles spec

                // Declared zoom levels
                var declaredZoomLevels = ReadZoomLevels(connection, out _tileRange);

                // Create schema
                return new GlobalSphericalMercator(format.ToString(), YAxis.TMS, declaredZoomLevels, extent: extent);
            }
            else
            {
                // this is the most regular case:
                return new GlobalSphericalMercator(format.ToString(), YAxis.TMS, extent: extent);
            }
        }

        private static bool HasMapTable(SQLiteConnection connection)
        {
            const string sql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='map';";
            return connection.ExecuteScalar<int>(sql) > 0;
        }

        private static Extent ReadExtent(SQLiteConnection connection)
        {
            const string sql = "SELECT \"value\" FROM metadata WHERE \"name\"=?;";
            try
            {

                var extentString = connection.ExecuteScalar<string>(sql, "bounds");
                var components = extentString.Split(',');
                var extent = new Extent(
                    double.Parse(components[0], NumberFormatInfo.InvariantInfo),
                    double.Parse(components[1], NumberFormatInfo.InvariantInfo),
                    double.Parse(components[2], NumberFormatInfo.InvariantInfo),
                    double.Parse(components[3], NumberFormatInfo.InvariantInfo)
                    );

                return ToMercator(extent);
            }
            catch (Exception)
            {
                return new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
            }
        }

        private static Extent ToMercator(Extent extent)
        {
            var minX = extent.MinX;
            var minY = extent.MinY;
            ToMercator(ref minX, ref minY);
            var maxX = extent.MaxX;
            var maxY = extent.MaxY;
            ToMercator(ref maxX, ref maxY);

            return new Extent(minX, minY, maxX, maxY);
        }

        private static void ToMercator(ref double mercatorXLon, ref double mercatorYLat)
        {
            if ((Math.Abs(mercatorXLon) > 180 || Math.Abs(mercatorYLat) > 90))
                return;

            double num = mercatorXLon * 0.017453292519943295;
            double x = 6378137.0 * num;
            double a = mercatorYLat * 0.017453292519943295;

            mercatorXLon = x;
            mercatorYLat = 3189068.5 * Math.Log((1.0 + Math.Sin(a)) / (1.0 - Math.Sin(a)));
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            var index = tileInfo.Index;

            if (IsTileIndexValid(index))
            {
                byte[] result;
                var cn = new SQLiteConnectionWithLock(_connectionString, SQLiteOpenFlags.ReadOnly);
                using (cn.Lock())
                {
                    const string sql =
                        "SELECT tile_data FROM \"tiles\" WHERE zoom_level=? AND tile_row=? AND tile_column=?;";
                    result = cn.ExecuteScalar<byte[]>(sql, int.Parse(index.Level), index.Row, index.Col);
                }
                return result == null || result.Length == 0
                    ? null
                    : result;
            }
            return null;
        }
        private static int[] ReadZoomLevels(SQLiteConnection connection, out Dictionary<string, int[]> tileRange)
        {
            var zoomLevels = new List<int>();
            tileRange = new Dictionary<string, int[]>();

            //Hack to see if "tiles" is a view
            var sql = "SELECT count(*) FROM sqlite_master WHERE type = 'view' AND name = 'tiles';";
            var name = "tiles";
            if (connection.ExecuteScalar<int>(sql) == 1)
            {
                //Hack to choose the index table
                sql = "SELECT sql FROM sqlite_master WHERE type = 'view' AND name = 'tiles';";
                var sqlCreate = connection.ExecuteScalar<string>(sql);
                if (!string.IsNullOrEmpty(sqlCreate))
                {
                    sql = sql.Replace("\n", "");
                    var indexFrom = sql.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase) + 6;
                    var indexJoin = sql.IndexOf(" INNER ", StringComparison.OrdinalIgnoreCase);
                    if (indexJoin == -1)
                        indexJoin = sql.IndexOf(" JOIN ", StringComparison.OrdinalIgnoreCase);
                    if (indexJoin > indexFrom)
                    {
                        sql = sql.Substring(indexFrom, indexJoin - indexFrom).Trim();
                        name = sql.Replace("\"", "");
                    }
                }
            }

            sql = "select \"zoom_level\", " +
                  "min(\"tile_column\") AS tc_min, max(\"tile_column\") AS tc_max, " +
                  "min(\"tile_row\") AS tr_min, max(\"tile_row\") AS tr_max " +
                  "from \"" + name + "\" group by \"zoom_level\";";

            var zlminmax = connection.Query<ZoomLevelMinMax>(sql);
            if (zlminmax == null || zlminmax.Count == 0)
                throw new Exception("No data in MbTiles");

            foreach (var tmp in zlminmax)
            {
                var zlString = tmp.ZoomLevel.ToString(NumberFormatInfo.InvariantInfo);
                zoomLevels.Add(tmp.ZoomLevel);
                tileRange.Add(tmp.ZoomLevel.ToString(NumberFormatInfo.InvariantInfo), new[]
                {
                    tmp.TileColMin, tmp.TileColMax,
                    tmp.TileRowMin, tmp.TileRowMax
                });
            }

            return zoomLevels.ToArray();
        }

        private static MbTilesFormat ReadFormat(SQLiteConnection connection)
        {
            try
            {
                var formatString = connection.ExecuteScalar<string>(MetadataSql, "format");
                var format = (MbTilesFormat)Enum.Parse(typeof(MbTilesFormat), formatString, true);
                return format;
            }
            catch
            {
                return MbTilesFormat.Png;
            }
        }

        private static MbTilesType ReadType(SQLiteConnection connection)
        {
            try
            {
                var typeString = connection.ExecuteScalar<string>(MetadataSql, "type");
                var type = (MbTilesType)Enum.Parse(typeof(MbTilesType), typeString, true);
                return type;
            }
            catch
            {
                return MbTilesType.BaseLayer;
            }
        }

        private bool IsTileIndexValid(TileIndex index)
        {
            if (_tileRange == null) return true;

            // this is an optimization that makes use of an additional 'map' table which is not part of the spec
            int[] range;
            if (_tileRange.TryGetValue(index.Level, out range))
            {
                return ((range[0] <= index.Col) && (index.Col <= range[1]) &&
                        (range[2] <= index.Row) && (index.Row <= range[3]));
            }
            return false;
        }
        public ITileSchema Schema { get; }
        public string Name { get; } = nameof(SQLiteTileSource);
        public Attribution Attribution { get; set; }

        [Table("tiles")]
        private class ZoomLevelMinMax
        {
            [Column("zoom_level")]
            public int ZoomLevel { get; set; }
            [Column("tr_min")]
            public int TileRowMin { get; set; }
            [Column("tr_max")]
            public int TileRowMax { get; set; }
            [Column("tc_min")]
            public int TileColMin { get; set; }
            [Column("tc_max")]
            public int TileColMax { get; set; }
        }
    }
}
