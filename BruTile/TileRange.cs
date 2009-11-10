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
        int firstCol;
        int lastCol;
        int firstRow;
        int lastRow;

        public TileRange(int col, int row) : this(col, row, col, row) { }

        public TileRange(int firstCol, int firstRow, int lastCol, int lastRow)
        {
            this.firstCol = firstCol;
            this.lastCol = lastCol;
            this.firstRow = firstRow;
            this.lastRow = lastRow;
        }

        public int FirstCol
        {
            get { return firstCol; }
            set { firstCol = value; }
        }

        public int LastCol
        {
            get { return lastCol; }
            set { lastCol = value; }
        }

        public int FirstRow
        {
            get { return firstRow; }
            set { firstRow = value; }
        }

        public int LastRow
        {
            get { return lastRow; }
            set { lastRow = value; }
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
              this.firstCol == tileRange.firstCol &&
              this.lastCol == tileRange.lastCol &&
              this.firstRow == tileRange.firstRow &&
              this.lastRow == tileRange.lastRow;
        }

        public override int GetHashCode()
        {
            return firstCol ^ lastCol ^ firstRow ^ lastRow;
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
