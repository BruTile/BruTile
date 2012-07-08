﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

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
