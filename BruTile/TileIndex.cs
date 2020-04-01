// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile
{
    public struct TileIndex : IComparable
    {
        public int Col { get; }
        public int Row { get; }
        public int Zoom { get; private set; }
        public string Level     // Note: TileIndex is a struct but Level is a class which needs GC. It would be nice if we could avoid GC.
        {
            get => Zoom.ToString();
            set
            {
                Zoom = (int)float.Parse(value);
            }
        }

        public TileIndex(int col, int row, string level) : this (col, row, (int)float.Parse(level))
        { 
        }

        public TileIndex(int col, int row, int zoom)
        {
            Col = col;
            Row = row;
            Zoom = zoom;
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
            if (Zoom < index.Zoom) return -1;
            if (Zoom > index.Zoom) return 1;
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
            return Col == index.Col && Row == index.Row && Zoom == index.Zoom;
        }

        public override int GetHashCode()
        {
            return Col ^ Row ^ Zoom;
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