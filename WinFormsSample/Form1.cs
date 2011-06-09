using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using BruTile;
using BruTile.Web;

namespace WinFormsSample
{
    public partial class Form1 : Form
    {
        readonly Bitmap buffer;

        public Form1()
        {
            InitializeComponent();

            buffer = new Bitmap(this.Width, this.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(buffer, 0, 0);
        }

        private void Form1Load(object sender, EventArgs e)
        {
            var mapTransform = new MapTransform(new PointF(0, 0), 50000f, this.Width, this.Height);

            // Here we use a tile schema that is defined in code. There are a few predefined 
            // tile schemas in the BruTile.dll. In the usual case the schema should be parsed
            // from a tile service description.
            ITileSchema schema = CreateTileSchema();

            int level = Utilities.GetNearestLevel(schema.Resolutions, mapTransform.Resolution);
            IList<TileInfo> infos = schema.GetTilesInView(mapTransform.Extent, level);

            IRequest requestBuilder = new TmsRequest(new Uri("http://a.tile.openstreetmap.org"), "png");

            Graphics graphics = Graphics.FromImage(buffer);
            foreach (TileInfo info in infos)
            {
                Uri url = requestBuilder.GetUri(info);
                byte[] bytes = RequestHelper.FetchImage(url);
                var bitmap = new Bitmap(new MemoryStream(bytes));
                RectangleF extent = mapTransform.WorldToMap(info.Extent.MinX, info.Extent.MinY, info.Extent.MaxX, info.Extent.MaxY);
                DrawTile(schema, graphics, bitmap, extent);
            }

            this.Invalidate();
        }

        private static void DrawTile(ITileSchema schema, Graphics graphics, Bitmap bitmap, RectangleF extent)
        {
            // For drawing on WinForms there are two things to take into account 
            // to prevent seams between tiles.
            // 1) The WrapMode should be set to TileFlipXY. This is related 
            //    to how pixels are rounded by GDI+
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
            // 2) The rectangle should be rounded to actual pixels. 
            Rectangle roundedExtent = RoundToPixel(extent);
            graphics.DrawImage(bitmap, roundedExtent, 0, 0, schema.Width, schema.Height, GraphicsUnit.Pixel, imageAttributes);
        }

        private static Rectangle RoundToPixel(RectangleF dest)
        {
            // To get seamless aligning you need to round the locations
            // not the width and height
            return new Rectangle(
                (int)Math.Round(dest.Left),
                (int)Math.Round(dest.Top),
                (int)(Math.Round(dest.Right) - Math.Round(dest.Left)),
                (int)(Math.Round(dest.Bottom) - Math.Round(dest.Top)));
        }

        private static ITileSchema CreateTileSchema()
        {
            var resolutions = new[] { 
                156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 9783.939618750, 
                4891.969809375, 2445.984904688, 1222.992452344, 611.496226172, 305.748113086, 
                152.874056543, 76.437028271, 38.218514136, 19.109257068, 9.554628534, 4.777314267,
                2.388657133, 1.194328567, 0.597164283};

            var schema = new TileSchema();
            schema.Name = "OpenStreetMap";
            int count = 0;
            foreach (float resolution in resolutions)
            {
                schema.Resolutions.Add(new Resolution {Id = count.ToString(), UnitsPerPixel = resolution});
                count++;
            }
            schema.OriginX = -20037508.342789;
            schema.OriginY = 20037508.342789;
            schema.Axis = AxisDirection.InvertedY;
            schema.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
            schema.Height = 256;
            schema.Width = 256;
            schema.Format = "png";
            schema.Srs = "EPSG:900913";
            return schema;
        }
    }
}
