#region License

/*
 *  The attached / following is part of SharpMap.Layers.BruTile.
 *  
 *  SharpMap.Layers.BruTile is free software © 2009 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/.
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2009
 *
 *  This work is inspired by FileCache.cs by Paul den Dulk for Brutile
 *  
 */

#endregion
using System;
using System.Data;
using System.Data.Common;

namespace BruTile.Cache
{
    /// <summary>
    /// Sql command for inserting tiles in database.
    /// </summary>
    /// <param name="connection">the connection to the database</param>
    /// <param name="qualifier">a delegate to decorate schema-table and table-column pairs</param>
    /// <param name="schema">the schema in which the table resides, usually 'public'</param>
    /// <param name="table">the name of the table</param>
    /// <returns>the <see cref="DbCommand"/> to insert a tile</returns>
    public delegate DbCommand AddTileCommand(DbConnection connection, DecorateDbObjects qualifier, String schema, String table);

    /// <summary>
    /// Sql command for deleting tiles from database
    /// </summary>
    /// <param name="connection">the connection to the database</param>
    /// <param name="qualifier">a delegate to decorate schema-table and table-column pairs</param>
    /// <param name="schema">the schema in which the table resides, usually 'public'</param>
    /// <param name="table">the name of the table</param>
    /// <returns>the <see cref="DbCommand"/> to insert a tile</returns>
    public delegate DbCommand RemoveTileCommand(DbConnection connection, DecorateDbObjects qualifier, String schema, String table);

    /// <summary>
    /// Sql command for finding a tiles
    /// </summary>
    /// <param name="connection">the connection to the database</param>
    /// <param name="qualifier">a delegate to decorate schema-table and table-column pairs</param>
    /// <param name="schema">the schema in which the table resides, usually 'public'</param>
    /// <param name="table">the name of the table</param>
    /// <returns>the <see cref="IDbCommand"/> to find a tile</returns>
    public delegate DbCommand FindTileCommand(DbConnection connection, DecorateDbObjects qualifier, String schema, String table);

    /// <summary>
    /// Declaration of schema-table and table-column pairs decorator
    /// </summary>
    /// <example>
    /// This function may return for schema = public and table = Data [public].[Data]
    /// </example>
    /// <param name="parent">the high value</param>
    /// <param name="child">the low value</param>
    /// <returns>the decorated pair</returns>
    /// <example>
    /// This function may return for schema = public and table = Data [public].[Data]
    /// </example>
    public delegate String DecorateDbObjects(String parent, String child);

    /// <summary>
    /// Database based Cache for Brutile
    /// The table must have at least the following layout (regard case):
    /// <list>
    /// <item>Level   integer</item>
    /// <item>Row     integer</item>
    /// <item>Col     integer</item>
    /// <item>Size    integer</item>
    /// <item>Image   blob/object</item>
    /// </list>
    /// Furthermore the table should have a primary/unique key on the columns Level, Row and Col.
    /// Therefore you should take care not to insert more than one item with the same TileKey.
    /// </summary>
    /// <typeparam name="TConnection">A Connection class derived from <see cref="DbConnection"/></typeparam>
    public class DbCache<TConnection> : ITileCache<byte[]>
        where TConnection: DbConnection
    {
        private static DbCommand BasicAddTileCommand(DbConnection connection, 
            DecorateDbObjects qualifier, String schema, String table)
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format(
                "INSERT INTO {0} VALUES(@Level, @Row, @Col, @Size, @Image);", qualifier(schema, table));

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

            par = cmd.CreateParameter();
            par.DbType = DbType.Int32;
            par.ParameterName = "Size";
            cmd.Parameters.Add(par);

            par = cmd.CreateParameter();
            par.DbType = DbType.Object;
            par.ParameterName = "Image";
            cmd.Parameters.Add(par);

            return cmd;
        }

        private static DbCommand BasicRemoveTileCommand(DbConnection connection,
            DecorateDbObjects qualifier, String schema, String table)
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = string.Format("DELETE FROM {0} WHERE ({1}=@Level AND {2}=@Row AND {3}=@Col);",
                qualifier(schema, table), qualifier(table, "Level"), qualifier(table, "Row"),
                qualifier(table, "Col"));

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

        private static DbCommand BasicFindTileCommand(DbConnection connection, 
            DecorateDbObjects qualifier, String schema, String table)
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format("SELECT [SIZE], [IMAGE] FROM {0} WHERE ({1}=@Level AND {2}=@Row AND {3}=@Col);",
                qualifier(schema, table), qualifier(table, "Level"), qualifier(table, "Row"),
                qualifier(table, "Col"));

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

        public readonly TConnection Connection;

        private readonly DecorateDbObjects _decorator;

        private readonly IDbCommand _addTileCommand;
        private readonly IDbCommand _removeTileCommand;
        private readonly IDbCommand _findTileCommand;

        public readonly String Schema;
        public readonly String Table;

        public DbCache(TConnection connection)
            :this(connection, delegate(string parent, string child){ return string.Format( "[{0}].[{1}]", parent, child);}, "public", "Tiles")
        {
        }

        public DbCache(TConnection connection, DecorateDbObjects qualifier, String schema, String table)
            :this(connection, qualifier, schema, table, BasicAddTileCommand, BasicRemoveTileCommand, BasicFindTileCommand)
        {
        }

        public DbCache(TConnection connection, DecorateDbObjects decorator, String schema, String table, AddTileCommand atc, RemoveTileCommand rtc, FindTileCommand ftc)
        {
            Connection = connection;
            Schema = schema;
            Table = table;

            _decorator = decorator;

            _addTileCommand = atc(connection, decorator, schema, table);
            _removeTileCommand = rtc(connection, decorator, schema, table);
            _findTileCommand = ftc(connection, decorator, schema, table);

            if (Connection.State == ConnectionState.Open)
                Connection.Close();
        }

        public virtual void Clear(Int32 level)
        {
            DbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = String.Format("DELETE FROM {0} WHERE {1}={2};", _decorator(Schema, Table),
                                            _decorator(Table, "Level"), level);

            Boolean wasClosed = OpenConnectionIfClosed();
            cmd.ExecuteNonQuery();

            if(wasClosed) Connection.Close();
        }

        public void Clear()
        {
            DbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = String.Format("DELETE FROM {0};", _decorator(Schema, Table));

            Boolean wasClosed = OpenConnectionIfClosed();
            cmd.ExecuteNonQuery();
            if (wasClosed) Connection.Close();
        }

        #region Implementation of ITileCache<byte[]>

        public void Add(TileKey key, byte[] image)
        {
            ((IDataParameter)_addTileCommand.Parameters[0]).Value = key.Level;
            ((IDataParameter)_addTileCommand.Parameters[1]).Value = key.Row;
            ((IDataParameter)_addTileCommand.Parameters[2]).Value = key.Col;
            ((IDataParameter)_addTileCommand.Parameters[3]).Value = image.Length;
            ((IDataParameter)_addTileCommand.Parameters[4]).Value = image;

            Boolean wasClosed = OpenConnectionIfClosed();

            _addTileCommand.ExecuteNonQuery();

            if ( wasClosed ) Connection.Close();
        }

        public void Remove(TileKey key)
        {
            ((IDataParameter)_removeTileCommand.Parameters[0]).Value = key.Level;
            ((IDataParameter)_removeTileCommand.Parameters[1]).Value = key.Row;
            ((IDataParameter)_removeTileCommand.Parameters[2]).Value = key.Col;

            Boolean wasClosed = OpenConnectionIfClosed();

            _removeTileCommand.ExecuteNonQuery();

            if (wasClosed) Connection.Close();
        }

        public byte[] Find(TileKey key)
        {
            ((IDataParameter)_findTileCommand.Parameters[0]).Value = key.Level;
            ((IDataParameter)_findTileCommand.Parameters[1]).Value = key.Row;
            ((IDataParameter)_findTileCommand.Parameters[2]).Value = key.Col;

            Boolean wasClosed = OpenConnectionIfClosed();

            IDataReader dr = _findTileCommand.ExecuteReader();
            byte[] ret = null;
            if (dr.Read())
            {
                Int32 size = dr.GetInt32(0);
                ret = new byte[size];
                dr.GetBytes(1, 0, ret, 0, size);
            }
            dr.Close();

            if ( wasClosed )Connection.Close();

            return ret;

        }

        #endregion

        #region Private Helpers
        private Boolean OpenConnectionIfClosed()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
                return true;
            }
            return false;
        }
        #endregion
    }
}