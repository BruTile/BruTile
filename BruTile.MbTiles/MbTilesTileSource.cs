// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System;
using System.Data.SQLite;
using BruTile.FileSystem;


namespace BruTile
{
    [Serializable]
    public class MbTilesTileSource : ITileSource //, System.Runtime.Serialization.ISerializable
    {
        public MbTilesTileSource(string file, ITileSchema schema = null, MbTilesType type = MbTilesType.None)
            : this(new SQLiteConnection(string.Format("Data Source={0}", file)), schema, type)
        {
        }

        internal MbTilesTileSource(SQLiteConnection connection, ITileSchema schema = null, MbTilesType type = MbTilesType.None)
        {
            _tileProvider = new MbTilesProvider(connection, schema, type);
        }

        private readonly MbTilesProvider _tileProvider;

        #region Implementation of ITileSource

        public ITileProvider Provider
        {
            get { return _tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return _tileProvider.Schema; }
        }

        public Extent Extent { get { return _tileProvider.Cache.Extent; } }

        public MbTilesFormat Format
        {
            get { return _tileProvider.Cache.Format; }
        }

        public MbTilesType Type { get { return _tileProvider.Cache.Type; } }

        #endregion Implementation of ITileSource
    }
}