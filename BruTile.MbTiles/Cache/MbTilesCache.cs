#region License

// Copyright 2011 - Felix Obermaier (www.ivv-aachen.de)
//
// This file is part of BruTile.MbTiles.
// BruTile.MbTiles is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// BruTile.MbTiles is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

#endregion License

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using BruTile.PreDefined;
using Community.CsharpSqlite.SQLiteClient;

namespace BruTile.Cache
{
    internal class MbTilesCache : DbCache<SqliteConnection>
    {
        //private static DbCommand AddTileCommand(DbConnection connection,
        //    DecorateDbObjects qualifier, String schema, String table, char parameterPrefix = ':')
        //{
        //    /*
        //    DbCommand cmd = connection.CreateCommand();
        //    cmd.CommandText = String.Format(
        //        "INSERT INTO {0} VALUES(:Level, :Col, :Row, :Image);", qualifier(schema, table));

        //    DbParameter par = cmd.CreateParameter();
        //    par.DbType = DbType.Int32;
        //    par.ParameterName = "Level";
        //    cmd.Parameters.Add(par);

        //    par = cmd.CreateParameter();
        //    par.DbType = DbType.Int32;
        //    par.ParameterName = "Row";
        //    cmd.Parameters.Add(par);

        //    par = cmd.CreateParameter();
        //    par.DbType = DbType.Int32;
        //    par.ParameterName = "Col";
        //    cmd.Parameters.Add(par);

        //    par = cmd.CreateParameter();
        //    par.DbType = DbType.Binary;
        //    par.ParameterName = "Image";
        //    cmd.Parameters.Add(par);

        //    return cmd;
        //    */
        //    throw new InvalidOperationException("Removing tiles from MbTiles is not allowed");
        //}

        //private static DbCommand RemoveTileCommand(DbConnection connection,
        //    DecorateDbObjects qualifier, String schema, String table, char parameterPrefix = ':')
        //{
        //    /*
        //    DbCommand cmd = connection.CreateCommand();
        //    cmd.CommandText = string.Format("DELETE FROM {0} WHERE ({1}=:Level AND {3}=:Col AND {2}=:Row);",
        //        qualifier(schema, table), qualifier(table, "zoom_level"), qualifier(table, "tile_row"),
        //        qualifier(table, "tile_col"));

        //    DbParameter par = cmd.CreateParameter();
        //    par.DbType = DbType.Int32;
        //    par.ParameterName = "Level";
        //    cmd.Parameters.Add(par);

        //    par = cmd.CreateParameter();
        //    par.DbType = DbType.Int32;
        //    par.ParameterName = "Row";
        //    cmd.Parameters.Add(par);

        //    par = cmd.CreateParameter();
        //    par.DbType = DbType.Int32;
        //    par.ParameterName = "Col";
        //    cmd.Parameters.Add(par);

        //    return cmd;
        //     */
        //    throw new InvalidOperationException("Removing tiles from MbTiles is not allowed");
        //}

        private static DbCommand FindTileCommand(DbConnection connection,
            DecorateDbObjects qualifier, String schema, String table, char parameterPrefix = ':')
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = String.Format("SELECT {0} FROM {1} WHERE ({2}={5}Level AND {4}={5}Col AND {3}={5}Row);",
                qualifier(table, "tile_data"), qualifier(schema, table), qualifier(table, "zoom_level"), qualifier(table, "tile_row"),
                qualifier(table, "tile_column"), parameterPrefix);

            DbParameter par = cmd.CreateParameter();
            par.DbType = DbType.Int32;
            par.ParameterName = "Level";
            cmd.Parameters.Add(par);

            par = cmd.CreateParameter();
            par.DbType = DbType.Int32;
            par.ParameterName = "Row";
            cmd.Parameters.Add(par);

            par = cmd.CreateParameter();
            par.DbType = DbType.Int32;
            par.ParameterName = "Col";
            cmd.Parameters.Add(par);

            return cmd;
        }

        private readonly int[] _declaredZoomLevels;
        private readonly Dictionary<int, int[]> _tileRange;

        private readonly GlobalMercator _schema;

        public MbTilesCache(SqliteConnection connection)
            : base(connection, (parent, child) => string.Format("\"{0}\"", child), string.Empty, "tiles",
            null, null, FindTileCommand)
        {
            var wasOpen = true;
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
                wasOpen = false;
            }

            //Declared zoom levels
            _declaredZoomLevels = ReadZoomLevels(connection, out _tileRange);

            //Format (if defined)
            _format = ReadFormat(connection);

            //Type (if defined)
            _type = ReadType(connection);

            //Extent
            _extent = ReadExtent(connection);

            //Create schema
            _schema = new GlobalMercator(Format.ToString(), _declaredZoomLevels);

            if (!wasOpen)
                Connection.Close();
        }

        private static Extent ReadExtent(SqliteConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT \"value\" FROM metadata WHERE \"name\"=:PName;";
                cmd.Parameters.Add(new SqliteParameter("PName", DbType.String) { Value = "bounds" });
                var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                    try
                    {
                        var extentString = reader.GetString(0);
                        var components = extentString.Split(',');
                        return new Extent(
                            double.Parse(components[0], System.Globalization.NumberFormatInfo.InvariantInfo),
                            double.Parse(components[1], System.Globalization.NumberFormatInfo.InvariantInfo),
                            double.Parse(components[2], System.Globalization.NumberFormatInfo.InvariantInfo),
                            double.Parse(components[3], System.Globalization.NumberFormatInfo.InvariantInfo)
                            );
                    }
                    catch { }
                    /*
                    if (Enum.TryParse(reader.GetString(0), true, out format))
                        Format = format;
                        */
                }
            }
            return new Extent(-180, -90, 180, 90);
        }

        private static int[] ReadZoomLevels(SqliteConnection connection, out Dictionary<int, int[]> tileRange)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var zoomLevels = new List<int>();
            tileRange = new Dictionary<int, int[]>();

            using (var cmd = connection.CreateCommand())
            {
                //Hack to see if "tiles" is a view
                cmd.CommandText = "SELECT count(*) FROM sqlite_master WHERE type = 'view' AND name = 'tiles';";
                var name = "tiles";
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 1)
                {
                    //Hack to choose the index table
                    cmd.CommandText = "SELECT sql FROM sqlite_master WHERE type = 'view' AND name = 'tiles';";
                    var sql = (string)cmd.ExecuteScalar();
                    if (!string.IsNullOrEmpty(sql))
                    {
                        sql = sql.Replace("\n", "");
                        var indexFrom = sql.IndexOf(" FROM ", StringComparison.InvariantCultureIgnoreCase) + 6;
                        var indexJoin = sql.IndexOf(" INNER ", StringComparison.InvariantCultureIgnoreCase);
                        if (indexJoin == -1)
                            indexJoin = sql.IndexOf(" JOIN ", StringComparison.InvariantCultureIgnoreCase);
                        if (indexJoin > indexFrom)
                        {
                            sql = sql.Substring(indexFrom, indexJoin - indexFrom).Trim();
                            name = sql.Replace("\"", "");
                        }
                    }
                }

                cmd.CommandText =
                    "select \"zoom_level\", " +
                            "min(\"tile_column\"), max(\"tile_column\"), " +
                            "min(\"tile_row\"), max(\"tile_row\") " +
                            "from \"" + name + "\" group by \"zoom_level\";";
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var zoomLevel = reader.GetInt32(0);
                        zoomLevels.Add(zoomLevel);
                        tileRange.Add(zoomLevel, new[] { reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4) });
                    }
                }
                if (zoomLevels.Count == 0)
                {
                    throw new Exception("No data in MbTiles");
                }
            }
            return zoomLevels.ToArray();
        }

        private static MbTilesFormat ReadFormat(SqliteConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT \"value\" FROM metadata WHERE \"name\"=:PName;";
                cmd.Parameters.Add(new SqliteParameter("PName", DbType.String) { Value = "format" });
                var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                    try
                    {
                        var format = (MbTilesFormat)Enum.Parse(typeof(MbTilesFormat), reader.GetString(0), true);
                        return format;
                    }
                    catch { }
                    /*
                    if (Enum.TryParse(reader.GetString(0), true, out format))
                        Format = format;
                        */
                }
            }
            return MbTilesFormat.Png;
        }

        private static MbTilesType ReadType(SqliteConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT \"value\" FROM metadata WHERE \"name\"=:PName;";
                cmd.Parameters.Add(new SqliteParameter("PName", DbType.String) { Value = "type" });
                var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                    try
                    {
                        var type = (MbTilesType)Enum.Parse(typeof(MbTilesType), reader.GetString(0), true);
                        return type;
                    }
                    catch { }
                    /*
                    if (Enum.TryParse(reader.GetString(0), true, out format))
                        Format = format;
                        */
                }
            }
            return MbTilesType.BaseLayer;
        }

        /*
        public int[] DeclaredZoomLevels { get { return _declaredZoomLevels; } }
        */

        internal static Extent MbTilesFullExtent { get { return new Extent(-180, -85, 180, 85); } }

        internal static void Create(string connectionString,
            string name, MbTilesType type, double version, string description,
            MbTilesFormat format, Extent extent, params string[] kvp)
        {
            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(name)) dict.Add("name", name);
            dict.Add("type", type.ToString().ToLowerInvariant());
            dict.Add("version", version.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (!string.IsNullOrEmpty(description)) dict.Add("description", description);
            dict.Add("format", format.ToString().ToLower());
            dict.Add("bounds", string.Format(System.Globalization.NumberFormatInfo.InvariantInfo,
                "{0},{1},{2},{3}", extent.MinX, extent.MinY, extent.MaxX, extent.MaxY));

            for (var i = 0; i < kvp.Length - 1; i++)
                dict.Add(kvp[i++], kvp[i]);
        }

#if !(SILVERLIGHT || WINDOWS_PHONE)

        internal static void Create(string connectionString, IDictionary<string, string> metadata)
        {
            var csb = new SqliteConnectionStringBuilder(connectionString);
            if (File.Exists(csb.DataSource))
                File.Delete(csb.DataSource);

            using (var cn = new SqliteConnection(connectionString))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText =
                        "CREATE TABLE metadata (name text, value text);" +
                        "CREATE TABLE tiles (zoom_level integer, tile_column integer, tile_row integer, tile_data blob);" +
                        "CREATE UNIQUE INDEX idx_tiles ON tiles (zoom_level, tile_colum, tile_row);";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO metadata VALUES (?, ?);";
                    var pName = new SqliteParameter("PName", DbType.String); cmd.Parameters.Add(pName);
                    var pValue = new SqliteParameter("PValue", DbType.String); cmd.Parameters.Add(pValue);

                    if (metadata == null || metadata.Count == 0)
                    {
                        metadata = new Dictionary<string, string>();
                    }
                    if (!metadata.ContainsKey("bounds"))
                        metadata.Add("bounds", "-180,-85,180,85");

                    foreach (var kvp in metadata)
                    {
                        pName.Value = kvp.Key;
                        pValue.Value = kvp.Value;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

#endif

        protected override bool IsTileIndexValid(TileIndex index)
        {
            int[] range;
            if (_tileRange.TryGetValue(int.Parse(index.LevelId), out range))
            {
                return ((range[0] <= index.Col) && (index.Col <= range[1]) &&
                        (range[2] <= index.Row) && (index.Row <= range[3]));
            }
            return false;
        }

        protected override byte[] GetBytes(IDataReader reader)
        {
            byte[] ret = null;
            if (reader.Read())
            {
                if (!reader.IsDBNull(0))
                    ret = (byte[])reader.GetValue(0);
            }
            return ret;
        }

        internal TileSchema TileSchema
        {
            get { return _schema; }
        }

        private readonly MbTilesFormat _format;
        private readonly MbTilesType _type;
        private readonly Extent _extent;

        public MbTilesFormat Format
        {
            get { return _format; }
        }

        public MbTilesType Type
        {
            get { return _type; }
        }

        public Extent Extent
        {
            get { return _extent; }
        }
    }
}