// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace BruTile
{
    public interface ITileProvider
    {
        /// <summary>
        /// May return null
        /// </summary>
        /// <param name="tileInfo"></param>
        /// <returns></returns>
        Task<byte[]> GetTileAsync(TileInfo tileInfo);
    }
}