using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BruTile.UI.Forms
{
    public class TileLayer : TileLayer<Bitmap>
    {
        public TileLayer(ITileSource source)
            : base(source, new TileFactory())
        {
        }
    }
}
