using System;
using System.Collections.Generic;
using System.Text;
using BruTile;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using BruTile.PreDefined;

namespace BruTile.Web
{
    public class OsmTileSource : TmsTileSource
    {
        public OsmTileSource()
            : base(new Uri("http://b.tile.openstreetmap.org"), new SphericalMercatorInvertedWorldSchema())
        {

        }
    }
}
