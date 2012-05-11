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
using BruTile.FileSystem;
using Community.CsharpSqlite.SQLiteClient;

namespace BruTile
{
    public class MbTilesTileSource : ITileSource
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