// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile;

public readonly struct TileIndex : IComparable
{
    public int Col { get; }
    public int Row { get; }
    public int Level { get; }

    public TileIndex(int col, int row, int level)
    {
        Col = col;
        Row = row;
        Level = level;
    }

    public int CompareTo(object obj)
    {
        if (obj is not TileIndex index)
        {
            throw new ArgumentException("object of type TileIndex was expected");
        }
        return CompareTo(index);
    }

    public int CompareTo(TileIndex index)
    {
        if (Col < index.Col) return -1;
        if (Col > index.Col) return 1;
        if (Row < index.Row) return -1;
        if (Row > index.Row) return 1;
        if (Level < index.Level) return -1;
        if (Level > index.Level) return 1;
        return 0;
    }

    public override bool Equals(object obj)
    {
        if (obj is not TileIndex index)
            return false;

        return Equals(index);
    }

    public bool Equals(TileIndex index)
    {
        return Col == index.Col && Row == index.Row && Level == index.Level;
    }

    public override int GetHashCode()
    {
        return Col ^ Row ^ Level;
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
