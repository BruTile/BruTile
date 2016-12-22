// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

// This file was created by Felix Obermaier (www.ivv-aachen.de) 2011.

using System;

namespace BruTile.Web
{
    [Obsolete("Replaced with KnownTileSources", true)]
    public class OsmTileServerConfig
    {
    }

    [Obsolete("Replaced with KnownTileSources", true)]
    internal class OsmTileServerConfigWithApiKey : OsmTileServerConfig
    {
    }

    [Obsolete("Replaced with BasicRequest", true)]
    public class OsmRequest : IRequest
    {
        public Uri GetUri(TileInfo info)
        {
            throw new NotImplementedException();
        }
    }
}