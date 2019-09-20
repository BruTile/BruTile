// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.
// Merged TileSource, Provider and Cache into MbTilesTileSource. PDD, 2017.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BruTile.Predefined;
using SQLite;

namespace BruTile.MbTiles
{
    /// <summary>
    /// An <see cref="ITileSource"/> implementation for MapBox Tiles files
    /// </summary>
    /// <seealso href="https://www.mapbox.com/developers/mbtiles/"/>
    public class MbTilesTileSource : ITileSource
    {
        public MbTilesType Type { get; }
        public string Version { get; }
        public string Description { get; }
        public int ZoomMin { get; }
        public int ZoomMax { get; }
        public string Json { get; }
        public string Compression { get; }

        private readonly SQLiteConnectionString _connectionString;
        private const string MetadataSql = "SELECT \"value\" FROM metadata WHERE \"name\"=?;";
        private readonly Dictionary<string, ZoomLevelMinMax> _tileRange;

        public MbTilesTileSource(SQLiteConnectionString connectionString, ITileSchema schema = null, MbTilesType type = MbTilesType.None, bool tileRangeOptimization = true)
        {
            _connectionString = connectionString;
			if (tileRangeOptimization || schema == null)
			{
				using (var connection = new SQLiteConnectionWithLock(connectionString))
				using (connection.Lock())
				{
					Type = type == MbTilesType.None ? ReadType(connection) : type;
					var schemaFromDatabase = ReadSchemaFromDatabase(connection);
					Schema = schema ?? schemaFromDatabase;

                    // Read other stuff
				    Version = ReadString(connection, "version");
				    Attribution = new Attribution(ReadString(connection, "attribution"));
				    Description = ReadString(connection, "description");
				    Name = ReadString(connection, "name");
				    Json = ReadString(connection, "json");
				    Compression = ReadString(connection, "compression");

				    var zoomMin = ReadInt(connection, "minzoom");
				    ZoomMin = zoomMin < 0 ? 0 : zoomMin;

				    var zoomMax = ReadInt(connection, "maxzoom");
				    ZoomMax = zoomMax < 0 ? int.MaxValue : zoomMax;

                    // the tile range should be based on the tiles actually present. 
                    var zoomLevelsFromDatabase = schemaFromDatabase.Resolutions.Select(r => r.Key);
					_tileRange = ReadZoomLevelsFromTilesTable(connection, zoomLevelsFromDatabase);
				}
			}
			else {
				Schema = schema;
				_tileRange = null;
			}
        }

        private static string ReadString(SQLiteConnection connection, string name)
        {
            const string sql = "SELECT \"value\" FROM metadata WHERE \"name\"=?;";
            try
            {
                var result = connection.ExecuteScalar<string>(sql, name);

                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static int ReadInt(SQLiteConnection connection, string name)
        {
            const string sql = "SELECT \"value\" FROM metadata WHERE \"name\"=?;";
            try
            {
                var result = connection.ExecuteScalar<int>(sql, name);

                return result;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static ITileSchema ReadSchemaFromDatabase(SQLiteConnectionWithLock connection)
        {
            var format = ReadFormat(connection);
            var extent = ReadExtent(connection);
            var zoomLevels = ReadZoomLevels(connection);

            // Create schema
            return new GlobalSphericalMercator(format.ToString(), YAxis.TMS, zoomLevels, extent: extent);
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
				using (var cn = new SQLiteConnectionWithLock(_connectionString))
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

        private static int[] ReadZoomLevels(SQLiteConnection connection)
        {
            var sql = "select distinct zoom_level as level from tiles";
            var zoomLevelsObjects = connection.Query<ZoomLevel>(sql);
            var zoomLevels = zoomLevelsObjects.Select(z => z.Level).ToArray();
            return zoomLevels;
        }

        [Table("tiles")]
        private class ZoomLevel // I would rather just user 'int' instead of this class in Query, but can't get it to work
        {
            [Column("level")]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Level { get; set; }
        }

        private static Dictionary<string, ZoomLevelMinMax> ReadZoomLevelsFromTilesTable(SQLiteConnection connection, IEnumerable<string> zoomLevels)
        {
            var tableName = "tiles";
            var tileRange = new Dictionary<string, ZoomLevelMinMax>();
            foreach (var zoomLevel in zoomLevels)
            {
                var sql = $"select min(tile_column) AS tc_min, max(tile_column) AS tc_max, min(tile_row) AS tr_min, max(tile_row) AS tr_max from {tableName} where zoom_level = {zoomLevel};";
                var rangeForLevel = connection.Query<ZoomLevelMinMax>(sql);
                tileRange.Add(zoomLevel, rangeForLevel.First());
            }
            return tileRange;
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
            ZoomLevelMinMax tileRange;
            if (_tileRange.TryGetValue(index.Level, out tileRange))
            {
                return
                    tileRange.TileColMin <= index.Col &&
                    index.Col <= tileRange.TileColMax &&
                    tileRange.TileRowMin <= index.Row && 
                    index.Row <= tileRange.TileRowMax;
            }
            return false;
        }
        public ITileSchema Schema { get; }
        public string Name { get; } = nameof(MbTilesTileSource);
        public Attribution Attribution { get; set; }

        [Table("tiles")]
        private class ZoomLevelMinMax
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            [Column("tr_min")]
            public int TileRowMin { get; set; }
            [Column("tr_max")]
            public int TileRowMax { get; set; }
            [Column("tc_min")]
            public int TileColMin { get; set; }
            [Column("tc_max")]
            public int TileColMax { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }
}
