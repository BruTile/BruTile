#region License

// Copyright 2009 - Paul den Dulk (Geodan)
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

using System.IO;
using BruTile.Cache;

namespace BruTile.FileSystem
{
    public class FileTileProvider : ITileProvider
    {
        readonly FileCache _fileCache;

        public FileTileProvider(FileCache fileCache)
        {
            _fileCache = fileCache;
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = _fileCache.Find(tileInfo.Index);
            if (bytes == null) throw new FileNotFoundException("The tile was not found at it's expected location");
            return bytes;
        }

    }
}
