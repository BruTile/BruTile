// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System;
using System.Runtime.Serialization;
using BruTile.FileSystem;
using Community.CsharpSqlite.SQLiteClient;

namespace BruTile
{
    [Serializable]
    public class MbTilesTileSource : ITileSource //, System.Runtime.Serialization.ISerializable
    {
        public MbTilesTileSource(string file, ITileSchema schema = null, MbTilesType type = MbTilesType.None)
            : this(new SqliteConnection(string.Format("Data Source={0}", new Uri(file))), schema, type)
        {
        }

        internal MbTilesTileSource(SqliteConnection connection, ITileSchema schema = null, MbTilesType type = MbTilesType.None)
        {
            _tileSource = new MbTilesProvider(connection, schema, type);
        }

        private readonly MbTilesProvider _tileSource;

        #region Implementation of ITileSource

        public ITileProvider Provider
        {
            get { return _tileSource; }
        }

        public ITileSchema Schema
        {
            get { return _tileSource.Schema; }
        }

        public MbTilesFormat Format
        {
            get { return _tileSource.Cache.Format; }
        }

        public MbTilesType Type { get { return _tileSource.Cache.Type; } }

        #endregion Implementation of ITileSource
    }
}