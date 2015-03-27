using BruTile.Tms;
using BruTile.Web;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace BruTile.Samples.SimpleStaticMap
{
    public partial class Form1 : Form
    {
        readonly Bitmap _buffer;

        //a list of resolutions in which the tiles are stored
        readonly double[] _unitsPerPixelArray =
        { 
            156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 9783.939618750, 
            4891.969809375, 2445.984904688, 1222.992452344, 611.496226172, 305.748113086, 
            152.874056543, 76.437028271, 38.218514136, 19.109257068, 9.554628534, 4.777314267,
            2.388657133, 1.194328567, 0.597164283};

        public Form1()
        {
            InitializeComponent();

            _buffer = new Bitmap(Width, Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_buffer, 0, 0);
        }

        private void Form1Load(object sender, EventArgs e)
        {
            var viewport = new Viewport(new PointF(629816f, 6805085f), 1222.992452344f, Width, Height);

            var schema = CreateTileSchema();
            var tiles = schema.GetTileInfos(viewport.Extent, Utilities.GetNearestLevel(schema.Resolutions, viewport.UnitsPerPixel));

            var requestBuilder = new TmsRequest(new Uri("http://a.tile.openstreetmap.org"), "png");

            var graphics = Graphics.FromImage(_buffer);
            foreach (var tile in tiles)
            {
                var url = requestBuilder.GetUri(tile);
                byte[] bytes = RequestHelper.FetchImage(url);
                var bitmap = new Bitmap(new MemoryStream(bytes));
                var destination = viewport.WorldToScreen(tile.Extent.MinX, tile.Extent.MinY, tile.Extent.MaxX, tile.Extent.MaxY);
                graphics.DrawImage(bitmap, RoundToPixel(destination));
            }
            Invalidate();
        }

        private ITileSchema CreateTileSchema()
        {
            var schema = new TileSchema
            {
                Name = "OpenStreetMap",
                OriginX = -20037508.342789,
                OriginY = 20037508.342789,
                YAxis = YAxis.OSM,
                Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789),
                Height = 256,
                Width = 256,
                Format = "png",
                Srs = "EPSG:900913"
            };

            var i = 0;
            foreach (var unitsPerPixel in _unitsPerPixelArray)
            {
                var levelId = i++.ToString(CultureInfo.InvariantCulture);
                schema.Resolutions[levelId] = new Resolution { UnitsPerPixel = unitsPerPixel, Id = levelId};
            }
            return schema;
        }

        private static Rectangle RoundToPixel(RectangleF dest)
        {
            // To get seamless aligning you need to round the locations
            // not the width and height
            var result = new Rectangle(
                (int)Math.Round(dest.Left),
                (int)Math.Round(dest.Top),
                (int)(Math.Round(dest.Right) - Math.Round(dest.Left)),
                (int)(Math.Round(dest.Bottom) - Math.Round(dest.Top)));
            return result;
        }

    }
}
