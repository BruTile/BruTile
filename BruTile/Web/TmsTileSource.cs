// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;
using BruTile.Web.TmsService;

namespace BruTile.Web
{
    public class TmsTileSource : TileSource
    {
        public TmsTileSource(string serviceUrl, ITileSchema tileSchema)
             : this (new Uri(serviceUrl), tileSchema)
        {
        }

        public TmsTileSource(Uri serviceUri, ITileSchema tileSchema) : 
            base(new WebTileProvider(new TmsRequest(serviceUri, tileSchema.Format)), tileSchema)
        {
        }

        public static ITileSource CreateFromTileMapResource(Stream tileMapResource)
        {
            return TileMapParser.CreateTileSource(tileMapResource);
        }
    }
}