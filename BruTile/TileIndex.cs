﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace BruTile
{
    public struct TileIndex : IComparable
    {
        private readonly int _col;
        private readonly int _row;
        private readonly string _level;
        
        public int Col
        {
            get { return _col; }
        }

        public int Row
        {
            get { return _row; }
        }

        public string Level
        {
            get { return _level; }
        }

        public TileIndex(int col, int row, string level)
        {
            _col = col;
            _row = row;
            _level = level;
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
            if (_col < index._col) return -1;
            if (_col > index._col) return 1;
            if (_row < index._row) return -1;
            if (_row > index._row) return 1;
            return String.Compare(_level, index._level, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TileIndex))
                return false;

            return Equals((TileIndex) obj);
        }

        public bool Equals(TileIndex index)
        {
            return _col == index._col && _row == index._row && _level == index._level;
        }

        public override int GetHashCode()
        {
            return _col ^ _row ^ _level.GetHashCode();
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