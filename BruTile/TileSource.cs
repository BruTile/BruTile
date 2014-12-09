// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile
{
    /// <summary>
    /// The default implementation of a <see cref="ITileSource"/>.
    /// </summary>
    public class TileSource : ITileSource
    {
        private readonly ITileProvider _provider;
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="tileProvider">The tile provider</param>
        /// <param name="tileSchema">The tile schema</param>
        public TileSource(ITileProvider tileProvider, ITileSchema tileSchema)
        {
            _provider = tileProvider;
            Schema = tileSchema;
        }
        
        /// <summary>
        /// Gets a the Name of the tile source
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the image content of the tile 
        /// </summary>
        public byte[] GetTile(TileInfo tileInfo)
        {
            return _provider.GetTile(tileInfo);
        }

        /// <summary>
        /// Gets a value indicating the tile schema
        /// </summary>
        public ITileSchema Schema { get; private set; }

        public override string ToString()
        {
            return string.Format("[TileSource:{0}]", Name);
        }
    }
}