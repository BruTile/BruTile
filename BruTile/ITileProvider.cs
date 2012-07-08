﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile
{
    public interface ITileProvider
    {
        byte[] GetTile(TileInfo tileInfo);
    }
}