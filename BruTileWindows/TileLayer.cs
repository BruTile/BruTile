using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;
using System.IO;

namespace BruTile.UI.Windows
{
    public class TileLayer : TileLayer<MemoryStream>
    {
        public TileLayer(ITileSource source)
            : base(source, new TileFactory())
        {
        }
    }
}
