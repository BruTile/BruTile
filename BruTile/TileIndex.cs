#region License

// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//  
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

using System;

namespace BruTile
{
    public struct TileIndex : IComparable
    {
        private readonly int _col;
        private readonly int _row;
        private readonly int _levelId;

        public int Col
        {
            get { return this._col; }
        }

        public int Row
        {
            get { return this._row; }
        }

        public int LevelId
        {
            get { return this._levelId; }
        }

        public TileIndex(int col, int row, int level)
        {
            this._col = col;
            this._row = row;
            this._levelId = level;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is TileIndex))
            {
                throw new ArgumentException("object of type TileIndex was expected");
            }
            return CompareTo((TileIndex) obj);
        }

        public int CompareTo(TileIndex index)
        {
            if (this._col < index._col) return -1;
            if (this._col > index._col) return 1;
            if (this._row < index._row) return -1;
            if (this._row > index._row) return 1;
            if (this._levelId < index._levelId) return -1;
            if (this._levelId > index._levelId) return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TileIndex))
                return false;

            return Equals((TileIndex) obj);
        }

        public bool Equals(TileIndex index)
        {
            return this._col == index._col && this._row == index._row && this._levelId == index._levelId;
        }

        public override int GetHashCode()
        {
            return this._col ^ this._row ^ this._levelId;
        }

        public static bool operator ==(TileIndex key1, TileIndex key2)
        {
            return Equals(key1, key2);
        }

        public static bool operator !=(TileIndex key1, TileIndex key2)
        {
            return !Equals(key1, key2);
        }

        public static bool operator <(TileIndex key1, TileIndex key2)
        {
            return (key1.CompareTo(key2) < 0);
        }

        public static bool operator >(TileIndex key1, TileIndex key2)
        {
            return (key1.CompareTo(key2) > 0);
        }
    }
}