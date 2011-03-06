using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace BruTile.Web
{
    /// <remarks>
    /// I don't like this class in its current form. It adds nothing except a more convenient constructor
    /// </remarks>
    public class TmsTileSource : ITileSource
    {
        public ITileProvider Provider { get; private set; }
        public ITileSchema Schema { get; private set; }
        
        public TmsTileSource(string serviceUrl, ITileSchema tileSchema)
             : this (new Uri(serviceUrl), tileSchema)
        {
        }

        public TmsTileSource(Uri serviceUri, ITileSchema tileSchema)
        {
            Provider = new WebTileProvider(new TmsRequest(serviceUri, tileSchema.Format));
            Schema = tileSchema;
        }
    }
}