// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile;

public readonly struct TileRange
{
    public int FirstCol { get; }
    public int FirstRow { get; }
    public int ColCount { get; }
    public int RowCount { get; }
    public int LastCol => FirstCol + ColCount - 1;
    public int LastRow => FirstRow + RowCount - 1;

    public TileRange(int col, int row) : this(col, row, 1, 1)
    { }

    public TileRange(int firstCol, int firstRow, int colCount, int rowCount) : this()
    {
        FirstCol = firstCol;
        FirstRow = firstRow;
        ColCount = colCount;
        RowCount = rowCount;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not TileRange range)
            return false;

        return Equals(range);
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
