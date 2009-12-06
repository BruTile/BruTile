using System;
using System.Collections.Generic;
using System.Text;

namespace BruTile
{
    public class TileSource : ITileSource
    {
        ITileProvider tileProvider;
        ITileSchema tileSchema;

        public TileSource(ITileProvider tileProvider, ITileSchema tileSchema)
        {
            this.tileProvider = tileProvider;
            this.tileSchema = tileSchema;
        }

        #region ITileSource Members

        public ITileProvider Provider
        {
            get { return tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return tileSchema; }
        }

        #endregion
    }
}
