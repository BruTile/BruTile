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

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

namespace BruTile
{
    internal struct TileRange
    {
        int _firstCol;
        int _lastCol;
        int _firstRow;
        int _lastRow;

        public TileRange(int col, int row) : this(col, row, col, row) { }

        public TileRange(int firstCol, int firstRow, int lastCol, int lastRow)
        {
            _firstCol = firstCol;
            _lastCol = lastCol;
            _firstRow = firstRow;
            _lastRow = lastRow;
        }

        public int FirstCol
        {
            get { return _firstCol; }
            set { _firstCol = value; }
        }

        public int LastCol
        {
            get { return _lastCol; }
            set { _lastCol = value; }
        }

        public int FirstRow
        {
            get { return _firstRow; }
            set { _firstRow = value; }
        }

        public int LastRow
        {
            get { return _lastRow; }
            set { _lastRow = value; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TileRange))
                return false;

            return Equals((TileRange)obj);
        }

        public bool Equals(TileRange tileRange)
        {
            return
              _firstCol == tileRange._firstCol &&
              _lastCol == tileRange._lastCol &&
              _firstRow == tileRange._firstRow &&
              _lastRow == tileRange._lastRow;
        }

        public override int GetHashCode()
        {
            return _firstCol ^ _lastCol ^ _firstRow ^ _lastRow;
        }

        public static bool operator ==(TileRange tileRange1, TileRange tileRange2)
        {
            return Equals(tileRange1, tileRange2);
        }

        public static bool operator !=(TileRange tileRange1, TileRange tileRange2)
        {
            return !Equals(tileRange1, tileRange2);
        }
    }
}
