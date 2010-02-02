using System;
using System.Collections.Generic;
using System.Text;
using BruTile;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using System.Net;

namespace BruTile.Web
{
    public class TileSourceOsm : TileSourceTms
    {
        Uri url = new Uri(""); //todo!!!

        public TileSourceOsm()
          //  : base(url, new SchemaBasicSphericalMercator())
            : base(new MemoryStream()) //!!! TEMP
        {

        }
    }
}
