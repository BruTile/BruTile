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
using System.Collections.Generic;
using System.Text;

namespace BruTile
{
  public class TileInfo
  {
    private TileKey key;
    private Extent extent;
    private double priority;
    private int retries;

    public TileKey Key
    {
      get { return key; }
      set { key = value; }
    }

    public Extent Extent
    {
      get { return extent; }
      set { extent = value; }
    }

    public double Priority
    {
      get { return priority; }
      set { priority = value; }
    }    

    public int Retries
    {
      get { return retries; }
      set { retries = value; }
    }
  }
}
