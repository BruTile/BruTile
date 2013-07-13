// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2009.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace BruTile.Cache
{
    /// <summary>
    /// Sql command for inserting tiles in database.
    /// </summary>
    /// <param name="connection">the connection to the database</param>
    /// <param name="qualifier">a delegate to decorate schema-table and table-column pairs</param>
    /// <param name="schema">the schema in which the table resides, usually 'public'</param>
    /// <param name="table">the name of the table</param>
    /// <param name="parameterPrefix">Prefix to use for decorating parameters in the query</param>
    /// <returns>the <see cref="DbCommand"/> to insert a tile</returns>
    public delegate DbCommand AddTileCommand(DbConnection connection, DecorateDbObjects qualifier, String schema, String table, char parameterPrefix = '@');

    /// <summary>
    /// Sql command for deleting tiles from database
    /// </summary>
    /// <param name="connection">the connection to the database</param>
    /// <param name="qualifier">a delegate to decorate schema-table and table-column pairs</param>
    /// <param name="schema">the schema in which the table resides, usually 'public'</param>
    /// <param name="table">the name of the table</param>
    /// <param name="parameterPrefix">Prefix to use for decorating parameters in the query</param>
    /// <returns>the <see cref="DbCommand"/> to insert a tile</returns>
    public delegate DbCommand RemoveTileCommand(DbConnection connection, DecorateDbObjects qualifier, String schema, String table, char parameterPrefix = '@');

    /// <summary>
    /// Sql command for finding a tiles
    /// </summary>
    /// <param name="connection">the connection to the database</param>
    /// <param name="qualifier">a delegate to decorate schema-table and table-column pairs</param>
    /// <param name="schema">the schema in which the table resides, usually 'public'</param>
    /// <param name="table">the name of the table</param>
    /// <param name="parameterPrefix">Prefix to use for decorating parameters in the query</param>
    /// <returns>the <see cref="IDbCommand"/> to find a tile</returns>
    public delegate DbCommand FindTileCommand(DbConnection connection, DecorateDbObjects qualifier, String schema, String table, char parameterPrefix = '@');

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
    /// Therefore you should take care not to insert more than one item with the same TileIndex.
    /// </summary>
    /// <typeparam name="TConnection">A Connection class derived from <see cref="DbConnection"/></typeparam>
    [Serializable]
    public class DbCache<TConnection> : IPersistentCache<byte[]>, ISerializable
        where TConnection : DbConnection, new()
    {
        private static DbCommand BasicAddTileCommand(DbConnection connection,
            DecorateDbObjects qualifier, String schema, String table, char parameterPrefix = '@')
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format(
                "INSERT INTO {0} VALUES({1}Level, {1}Row, {1}Col, {1}Size, {1}Image);", qualifier(schema, table), parameterPrefix);

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
            par.DbType = DbType.Binary;
            par.ParameterName = "Image";
            cmd.Parameters.Add(par);

            return cmd;
        }

        private static DbCommand BasicRemoveTileCommand(DbConnection connection,
            DecorateDbObjects qualifier, String schema, String table, char parameterPrefix = '@')
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = string.Format("DELETE FROM {0} WHERE ({1}={4}Level AND {2}={4}Row AND {3}={4}Col);",
                qualifier(schema, table), qualifier(table, "Level"), qualifier(table, "Row"),
                qualifier(table, "Col"), parameterPrefix);

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
            DecorateDbObjects qualifier, String schema, String table, char parameterPrefix = '@')
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format("SELECT [SIZE], [IMAGE] FROM {0} WHERE ({1}={4}Level AND {2}={4}Row AND {3}={4}Col);",
                qualifier(schema, table), qualifier(table, "Level"), qualifier(table, "Row"),
                qualifier(table, "Col"), parameterPrefix);

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

        [NonSerialized]
        private readonly DecorateDbObjects _decorator = (parent, child) => string.Format("\"{0}\".\"{1}\"", parent, child);

        private readonly IDbCommand _addTileCommand;
        private readonly IDbCommand _removeTileCommand;
        private readonly IDbCommand _findTileCommand;

        public readonly String Schema;
        public readonly String Table;

        [NonSerialized]
        private readonly object _addLock = new object();
        [NonSerialized]
        private readonly object _removeLock = new object();

        [NonSerialized]
        private readonly BankOfSelectTileCommands _bank;

        public DbCache(TConnection connection)
            : this(connection, (parent, child) => string.Format("[{0}].[{1}]", parent, child), "public", "Tiles")
        {
        }

        public DbCache(TConnection connection, DecorateDbObjects qualifier, String schema, String table)
            : this(connection, qualifier, schema, table, BasicAddTileCommand, BasicRemoveTileCommand, BasicFindTileCommand)
        {
        }

        public DbCache(TConnection connection, DecorateDbObjects decorator, String schema, String table, AddTileCommand atc, RemoveTileCommand rtc, FindTileCommand ftc)
        {
            Connection = connection;
            Schema = schema;
            Table = table;

            //_decorator = decorator;

            if (atc != null)
            {
                _addTileCommand = atc(connection, decorator, schema, table);
            }

            if (rtc != null)
            {
                _removeTileCommand = rtc(connection, decorator, schema, table);
            }

            if (ftc == null)
                ftc = BasicFindTileCommand;

            _findTileCommand = ftc(connection, decorator, schema, table);

            _bank = new BankOfSelectTileCommands(_findTileCommand);

            if (Connection.State == ConnectionState.Open)
                Connection.Close();
        }

        public DbCache(SerializationInfo info, StreamingContext context)
        {
            Connection = (TConnection) info.GetValue("connection", typeof (TConnection));
            Schema = info.GetString("schema");
            Table = info.GetString("table");

            _addTileCommand = (IDbCommand) info.GetValue("add", typeof (IDbCommand));
            _removeTileCommand = (IDbCommand)info.GetValue("remove", typeof(IDbCommand));
            _findTileCommand = (IDbCommand)info.GetValue("find", typeof(IDbCommand));

            _bank = new BankOfSelectTileCommands(_findTileCommand);
        }

        public virtual void Clear(Int32 level)
        {
            DbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = String.Format("DELETE FROM {0} WHERE {1}={2};", _decorator(Schema, Table),
                                            _decorator(Table, "Level"), level);

            Boolean wasClosed = OpenConnectionIfClosed();
            cmd.ExecuteNonQuery();

            if (wasClosed) Connection.Close();
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

        public void Add(TileIndex index, byte[] image)
        {
            if (_addTileCommand == null)
                throw new InvalidOperationException("Cache is readonly");

            lock (_addLock)
            {
                ((IDataParameter)_addTileCommand.Parameters[0]).Value = index.Level;
                ((IDataParameter)_addTileCommand.Parameters[1]).Value = index.Row;
                ((IDataParameter)_addTileCommand.Parameters[2]).Value = index.Col;
                ((IDataParameter)_addTileCommand.Parameters[3]).Value = image.Length;
                ((IDataParameter)_addTileCommand.Parameters[4]).Value = image;

                Boolean wasClosed = OpenConnectionIfClosed();

                _addTileCommand.ExecuteNonQuery();

                if (wasClosed) Connection.Close();
            }
        }

        public void Remove(TileIndex index)
        {
            if (_removeTileCommand == null)
                throw new InvalidOperationException("Cache is readonly");

            lock (_removeLock)
            {
                ((IDataParameter)_removeTileCommand.Parameters[0]).Value = index.Level;
                ((IDataParameter)_removeTileCommand.Parameters[1]).Value = index.Row;
                ((IDataParameter)_removeTileCommand.Parameters[2]).Value = index.Col;

                Boolean wasClosed = OpenConnectionIfClosed();

                _removeTileCommand.ExecuteNonQuery();

                if (wasClosed) Connection.Close();
            }
        }

        protected virtual byte[] GetBytes(IDataReader reader)
        {
            byte[] ret = null;
            if (reader.Read())
            {
                Int32 size = reader.GetInt32(0);
                ret = new byte[size];
                reader.GetBytes(1, 0, ret, 0, size);
            }
            return ret;
        }

        public byte[] Find(TileIndex index)
        {
            if (!IsTileIndexValid(index))
                return null;

            IDbCommand cmd = _bank.Borrow();

            ((IDataParameter)cmd.Parameters[0]).Value = index.Level;
            ((IDataParameter)cmd.Parameters[1]).Value = index.Row;
            ((IDataParameter)cmd.Parameters[2]).Value = index.Col;

            //Boolean wasClosed = OpenConnectionIfClosed();

            IDataReader dr = cmd.ExecuteReader();
            byte[] ret = GetBytes(dr);
            dr.Close();

            cmd.Connection.Close();

            _bank.Return(cmd);

            return ret;
        }

        protected virtual bool IsTileIndexValid(TileIndex index)
        {
            return true;
        }

        #endregion Implementation of ITileCache<byte[]>

        #region Private Helpers

        private bool OpenConnectionIfClosed()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
                return true;
            }
            return false;
        }

        #endregion Private Helpers

        internal class BankOfSelectTileCommands
        {
            private int _maxStore;
            private int _maxBorrowed;

            public BankOfSelectTileCommands(IDbCommand template)
            {
                _template = template;
                for (int i = 0; i < 5; i++)
                {
                    _store.Enqueue(CreateNew());
                }
            }

            private IDbCommand CreateNew()
            {
                var conn = new TConnection { ConnectionString = _template.Connection.ConnectionString };

                var newItem = conn.CreateCommand();
                newItem.CommandText = _template.CommandText;

                foreach (IDbDataParameter parameter in _template.Parameters)
                {
                    IDbDataParameter pNew = newItem.CreateParameter();

                    pNew.DbType = parameter.DbType;
                    pNew.Direction = parameter.Direction;
                    //pNew.IsNullable = parameter.IsNullable;
                    pNew.ParameterName = parameter.ParameterName;
                    pNew.Precision = parameter.Precision;
                    pNew.Scale = parameter.Scale;
                    pNew.Size = parameter.Size;
                    pNew.SourceColumn = parameter.SourceColumn;
#if !(SILVERLIGHT || WINDOWS_PHONE)
                    pNew.SourceVersion = parameter.SourceVersion;
#endif
                    //pNew.Value = ()parameter.Value.

                    newItem.Parameters.Add(pNew);
                }
                _maxStore++;
                return newItem;
            }

            private readonly Queue<IDbCommand> _store = new Queue<IDbCommand>(20);
            private readonly IDbCommand _template;
            private readonly object _lock = new object();

            public IDbCommand Borrow()
            {
                IDbCommand command;
                lock (_lock)
                {
                    if (_store.Count == 0)
                        _store.Enqueue(CreateNew());

                    var borrowed = _maxStore - _store.Count;
                    _maxBorrowed = Math.Max(borrowed, _maxBorrowed);

                    command = _store.Dequeue();
                }

                command.Connection.Open();
                return command;
            }

            public void Return(IDbCommand command)
            {
                lock (_lock)
                {
                    _store.Enqueue(command);
                }
            }
#if DEBUG
            public int CommandsInStore
            {
                get { return _store.Count; }
            }

            public int MaxBorrowed
            {
                get { return _maxBorrowed; }
            }
#endif
        }
#if DEBUG
        public int CommandsInStore
        {
            get { return _bank.CommandsInStore; }
        }

        public int MaxBorrowed
        {
            get { return _bank.MaxBorrowed; }
        }
#endif

        #region Implementation of ISerializable

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("connection", Connection);
            info.AddValue("schema", Schema);
            info.AddValue("table", Table);

            info.AddValue("add", _addTileCommand);
            info.AddValue("remove", _removeTileCommand);
            info.AddValue("find", _findTileCommand);
        }

        #endregion
    }
}