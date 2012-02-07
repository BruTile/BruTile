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
        public MbTilesTileSource(string file)
            : this(new SqliteConnection(string.Format("Data Source={0}", new Uri(file))))
        {
        }

        internal MbTilesTileSource(SqliteConnection connection)
        {
            _tileSource = new MbTilesProvider(connection);
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

        public Extent Extent
        {
            get
            {
                var tmp = _tileSource.Cache.Extent;

                var schemaExtent = Schema.Extent;

                var ll = ToMercator(tmp.MinX, tmp.MinY, schemaExtent.MaxX);
                var ur = ToMercator(tmp.MaxX, tmp.MaxY, schemaExtent.MaxX);

                return new Extent(ll[0], ll[1], ur[0], ur[1]);
            }
        }

        private static double[] ToMercator(double lon, double lat, double scale)
        {
            var x = lon * scale / 180;
            var y = Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180);
            y = y * scale / 180;
            return new[] { x, y };
        }

        #endregion Implementation of ITileSource
    }
}