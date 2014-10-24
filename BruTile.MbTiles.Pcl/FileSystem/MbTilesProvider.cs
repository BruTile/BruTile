// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2010.

using BruTile.Cache;
using SQLite.Net;

namespace BruTile.FileSystem
{
    internal class MbTilesProvider : ITileProvider
    {
        private readonly MbTilesCache _cache;

        internal MbTilesProvider(SQLiteConnectionString connectionString, ITileSchema schema = null, MbTilesType type = MbTilesType.None)
        {
            _cache = new MbTilesCache(connectionString, schema, type);
        }

        internal ITileSchema Schema { get { return _cache.TileSchema; } }

        internal MbTilesCache Cache
        {
            get
            {
                return _cache;
            }
        }

        #region Implementation of ITileProvider

        public byte[] GetTile(TileInfo tileInfo)
        {
            return _cache.Find(tileInfo.Index);
        }

        #endregion Implementation of ITileProvider
    }
}