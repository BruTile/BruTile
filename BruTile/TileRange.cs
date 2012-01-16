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

namespace BruTile
{
    public struct TileRange
    {
        public int FirstCol { get; private set; }
        public int FirstRow { get; private set; }
        public int ColCount { get; private set; }
        public int RowCount { get; private set; }

        public TileRange(int col, int row) : this(col, row, 1, 1) { }

        public TileRange(int firstCol, int firstRow, int colCount, int rowCount) : this()
        {
            FirstCol = firstCol;
            FirstRow = firstRow;
            ColCount = colCount;
            RowCount = rowCount;
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
              FirstCol == tileRange.FirstCol &&
              ColCount == tileRange.ColCount &&
              FirstRow == tileRange.FirstRow &&
              RowCount == tileRange.RowCount;
        }

        public override int GetHashCode()
        {
            return FirstCol ^ ColCount ^ FirstRow ^ RowCount;
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
