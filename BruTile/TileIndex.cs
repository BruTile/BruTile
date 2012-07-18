﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace BruTile
{
    public struct TileIndex : IComparable
    {
        private readonly int _col;
        private readonly int _row;
        private readonly string _levelId;

        public int Col
        {
            get { return _col; }
        }

        public int Row
        {
            get { return _row; }
        }

        public string LevelId
        {
            get { return _levelId; }
        }

        public TileIndex(int col, int row, int level)
        {
            _col = col;
            _row = row;
            _levelId = level.ToString(CultureInfo.InvariantCulture);
        }

        public TileIndex(int col, int row, string levelId)
        {
            _col = col;
            _row = row;
            _levelId = levelId;
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
            return string.CompareOrdinal(_levelId, index._levelId);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TileIndex))
                return false;

            return Equals((TileIndex) obj);
        }

        public bool Equals(TileIndex index)
        {
            return _col == index._col && _row == index._row && _levelId == index._levelId;
        }

        public override int GetHashCode()
        {
            return _col ^ _row ^ _levelId.GetHashCode();
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