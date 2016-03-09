// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2010.

using System;
using System.Data.SQLite;
using BruTile.Cache;

namespace BruTile.FileSystem
{
    [Serializable]
    public class MbTilesProvider : ITileProvider
    {
        private readonly MbTilesCache _cache;

        public MbTilesProvider(string file)
            : this(new SQLiteConnection(string.Format("Data Source={0}", file)))
        {
        }

        public MbTilesProvider(SQLiteConnection connection, ITileSchema schema = null, MbTilesType type = MbTilesType.None)
        {
            _cache = new MbTilesCache(connection, schema, type);
        }

        public ITileSchema Schema => _cache.TileSchema;

        internal MbTilesCache Cache
        {
            get
            {
                return _cache;
            }
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            return _cache.Find(tileInfo.Index);
        }
    }
}