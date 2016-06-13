// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System;
using BruTile.Cache;
using BruTile.FileSystem;
using SQLite.Net;
using SQLite.Net.Interop;

namespace BruTile
{
    /// <summary>
    /// An <see cref="ITileSource"/> implementation for MapBox Tiles files
    /// </summary>
    /// <seealso href="https://www.mapbox.com/developers/mbtiles/"/>
    public class MbTilesTileSource : ITileSource //, System.Runtime.Serialization.ISerializable
    {
        readonly MbTilesProvider _provider;
        const string DefaultName = nameof(MbTilesTileSource);

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="connectionString">The MapBox tiles file</param>
        /// <param name="name">The name of the TileSource</param>
        /// <param name="schema">The tile schema</param>
        /// <param name="type">The type of the MapBox tiles file</param>
        public MbTilesTileSource(SQLiteConnectionString connectionString, ITileSchema schema = null, 
            MbTilesType type = MbTilesType.None, string name = DefaultName)
            : this(new MbTilesProvider(connectionString, schema, type), name)
        {
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="provider">The MapBox Tiles provider</param>
        /// <param name="name">The name of the TileSource</param>
        internal MbTilesTileSource(MbTilesProvider provider, string name = DefaultName)
        {
            _provider = provider;
            Name = name;
        }
        
        /// <summary>
        /// Gets the actual image content of the tile as byte array
        /// </summary>
        public byte[] GetTile(TileInfo tileInfo)
        {
            return _provider.GetTile(tileInfo);
        }

        /// <summary>
        /// Gets a value indicating the schema of the tile source
        /// </summary>
        public ITileSchema Schema => _provider.Schema;

        /// <summary>
        /// Gets a value indicating the name of the tile source
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating the (image-)format of the tiles
        /// </summary>
        public MbTilesFormat Format => _provider.Cache.Format;

        /// <summary>
        /// Gets a value indicating the type of the tiles
        /// </summary>
        public MbTilesType Type => _provider.Cache.Type;

        /// <summary>
        /// Method to initialize SQLite.Net with the platform it is used with.
        /// </summary>
        /// <param name="platform"></param>
        public static void SetPlatform(ISQLitePlatform platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            MbTilesCache.SetPlatform(platform);
        }

        /// <summary>
        /// Gets a value indicating the covered extent
        /// </summary>
        public Extent Extent => _provider.Cache.Extent;
    }
}