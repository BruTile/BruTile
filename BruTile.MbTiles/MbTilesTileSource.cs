// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System;
using System.Data.SQLite;
using BruTile.FileSystem;
using BruTile.Predefined;

namespace BruTile
{
    /// <summary>
    /// An <see cref="ITileSource"/> implementation for MapBox Tiles files
    /// </summary>
    /// <seealso href="https://www.mapbox.com/developers/mbtiles/"/>
    [Serializable]
    public class MbTilesTileSource : ITileSource //, System.Runtime.Serialization.ISerializable
    {
        private readonly MbTilesProvider _provider;

        #region Implementation of ITileSource

        public ITileProvider Provider
        {
            get { return _provider; }
        }

        public MbTilesFormat Format
        {
            get { return _provider.Cache.Format; }
        }

        public MbTilesType Type { get { return _provider.Cache.Type; } }

        public ITileSchema Schema
        {
            get { return _provider.Schema; }
        }

        public string Name { get; private set; }

        /// <summary>
        /// Gets the actual image content of the tile as byte array
        /// </summary>
        public byte[] GetTile(TileInfo tileInfo)
        {
            return _provider.GetTile(tileInfo);
        }

        #endregion Implementation of ITileSource

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="file">The MapBox tiles file</param>
        /// <param name="schema">The tile schema (should be of <see cref="GlobalMercator"/></param>
        /// <param name="type">The type of the MapBox tiles file</param>
        public MbTilesTileSource(string file, ITileSchema schema = null, MbTilesType type = MbTilesType.None)
            : this(new SQLiteConnection(string.Format("Data Source={0}", file)), schema, type)
        {
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="connection">The connection to the MapBox tiles file</param>
        /// <param name="schema">The tile schema (should be of <see cref="GlobalMercator"/></param>
        /// <param name="type">The type of the MapBox tiles file</param>
        public MbTilesTileSource(SQLiteConnection connection, ITileSchema schema = null, MbTilesType type = MbTilesType.None)
            : this(new MbTilesProvider(connection, schema, type))
        {
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="provider">The MapBox Tiles provider</param>
        public MbTilesTileSource(MbTilesProvider provider)
        {
            _provider = provider;
        }
    }
}