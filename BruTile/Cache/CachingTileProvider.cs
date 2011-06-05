#region License

// Copyright 2011 - Paul den Dulk (Geodan)
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

using System;

namespace BruTile.Cache
{
    internal class CachingTileProvider : ITileProvider
    {
        private readonly ITileProvider _provider;
        private readonly ITileCache<byte[]> _cache;

        public CachingTileProvider(ITileProvider provider, ITileCache<byte[]> cache)
        {
            _provider = provider;
            _cache = cache;
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            byte[] bytes = _cache.Find(tileInfo.Index);

            if (bytes == null)
            {
                bytes = _provider.GetTile(tileInfo);
                if (bytes != null) _cache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }
    }
}