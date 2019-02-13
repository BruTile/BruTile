// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile
{
    public struct TileIndex : IComparable
    {
        public int Col { get; }
        public int Row { get; }
        public string Level { get; }  // Note: TileIndex is a struct but Level is a class which needs GC. It would be nice if we could avoid GC.

        public TileIndex(int col, int row, string level)
        {
            Col = col;
            Row = row;
            Level = level;
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
            if (Col < index.Col) return -1;
            if (Col > index.Col) return 1;
            if (Row < index.Row) return -1;
            if (Row > index.Row) return 1;
            return String.Compare(Level, index.Level, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TileIndex))
                return false;

            return Equals((TileIndex) obj);
        }

        public bool Equals(TileIndex index)
        {
            return Col == index.Col && Row == index.Row && Level == index.Level;
        }

        public override int GetHashCode()
        {
            return Col ^ Row ^ ((Level == null) ? 0 : Level.GetHashCode());
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