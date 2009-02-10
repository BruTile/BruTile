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

using System;

namespace Tiling
{
  public struct TileKey : IComparable
  {
    private int col;
    private int row;
    private int level;

    public int Col
    {
      get { return col; }
    }

    public int Row
    {
      get { return row; }
    }

    public int Level
    {
      get { return level; }
    }

    public TileKey(int col, int row, int level)
    {
      this.col = col;
      this.row = row;
      this.level = level;
    }

    public int CompareTo(object obj)
    {
      if (!(obj is TileKey))
      {
        throw new ArgumentException("object of type TileKey was expected");
      }
      return CompareTo((TileKey)obj);
    }

    public int CompareTo(TileKey key)
    {
      if (col < key.col) return -1;
      if (col > key.col) return 1;
      if (row < key.row) return -1;
      if (row > key.row) return 1;
      if (level < key.level) return -1;
      if (level > key.level) return 1;
      return 0;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is TileKey))
        return false;
      
      return Equals((TileKey)obj);
    }

    public bool Equals(TileKey key)
    {
      return col == key.col && row == key.row && level == key.level;
    }

    public override int GetHashCode()
    {
      return col ^ row ^ level;
    }

    public static bool operator ==(TileKey key1, TileKey key2)
    {
      return Equals(key1, key2);
    }

    public static bool operator !=(TileKey key1, TileKey key2)
    {
      return !Equals(key1, key2);
    }

    public static bool operator <(TileKey key1, TileKey key2)
    {
      return (key1.CompareTo(key2) < 0);
    }

    public static bool operator >(TileKey key1, TileKey key2)
    {
      return (key1.CompareTo(key2) > 0);
    }  
  }
}
