﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile
{
    public class TileSource : ITileSource
    {
        public ITileProvider Provider { get; private set; }
        public ITileSchema Schema { get; private set; }

        public TileSource(ITileProvider tileProvider, ITileSchema tileSchema)
        {
            Provider = tileProvider;
            Schema = tileSchema;
        }

        public virtual Extent Extent { get { return Schema.Extent; }}
    }
}