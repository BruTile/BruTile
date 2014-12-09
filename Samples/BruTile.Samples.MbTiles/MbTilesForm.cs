using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;
using SQLite.Net;

namespace BruTile.Samples.MbTiles
{
    public partial class MbTilesForm : Form
    {
        private Bitmap _buffer;
        private MapTransform _mapTransform;
        private MbTilesTileSource _source;

        public MbTilesForm()
        {
            InitializeComponent();
            _buffer = new Bitmap(Width, Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_buffer, 0, 0);
            base.OnPaint(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            /* http://a.tiles.mapbox.com/mapbox/download/haiti-terrain-grey.mbtiles */
            var path = Path.Combine(Path.GetTempPath(), "mapbox.haiti-terrain-grey.mbtiles");
            if (File.Exists(path))
            {
                _source = new MbTilesTileSource(new SQLiteConnectionString(path, false));
                var scale = (float)(1.1 * Math.Max(_source.Schema.Extent.Width / picMap.Width, _source.Schema.Extent.Height / picMap.Height));
                _mapTransform = new MapTransform(
                    new PointF((float)_source.Schema.Extent.CenterX, (float)_source.Schema.Extent.CenterY),
                    scale, picMap.Width, picMap.Height);

                RenderToBuffer();
            }
            base.OnLoad(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (_mapTransform == null)
                return;

            var res = _mapTransform.Resolution;
            float factor;
            if (e.Delta < 0)
                factor = 1.2f;
            else if (e.Delta > 0)
                factor = 1 / 1.2f;
            else
                return;

            var center = new PointF(picMap.Width * 0.5f, picMap.Height * 0.5f);
            var point = new PointF(e.X, e.Y);

            var dx = (center.X - point.X) * factor;
            var dy = (center.Y - point.Y) * factor;

            var newCenter = _mapTransform.MapToWorld(point.X + dx, point.Y + dy);

            var transform = new MapTransform(newCenter, res * factor, picMap.Width, picMap.Height);
            _mapTransform = transform;
            RenderToBuffer();

            base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_mapTransform == null) return;
            if (picMap.Width == 0 || picMap.Height == 0) return;

            _buffer = new Bitmap(picMap.Width, picMap.Height);
            _mapTransform = new MapTransform(_mapTransform.Center, _mapTransform.Resolution, picMap.Width, picMap.Height);
            RenderToBuffer();
        }

        private void RenderToBuffer()
        {
            var levelIndex = Utilities.GetNearestLevel(_source.Schema.Resolutions, _mapTransform.Resolution);

            using (var g = Graphics.FromImage(_buffer))
            {
                g.Clear(Color.White);
                foreach (var tileInfo in _source.Schema.GetTileInfos(_mapTransform.Extent, levelIndex))
                {
                    var res = _source.GetTile(tileInfo);
                    var extent = _mapTransform.WorldToMap(tileInfo.Extent.MinX, tileInfo.Extent.MinY,
                                                            tileInfo.Extent.MaxX, tileInfo.Extent.MaxY);
                    if (res != null)
                    {
                        var tileStream = new MemoryStream(res);
                        var tile = (Bitmap)Image.FromStream(tileStream);

                        DrawTile(_source.Schema, g, tile, extent, tileInfo.Index.Level);
                    }
                    var roundedExtent = RoundToPixel(extent);
                    g.DrawRectangle(Pens.Black, roundedExtent);
                    g.DrawString(string.Format("({2}:{0},{1})", tileInfo.Index.Col, tileInfo.Index.Row, tileInfo.Index.Level), new Font("Arial", 8, FontStyle.Regular), Brushes.OrangeRed, roundedExtent, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.None });
                }
                g.DrawRectangle(new Pen(Color.Tomato, 2), RoundToPixel(_mapTransform.WorldToMap(_source.Schema.Extent)));
            }

            tsslExtent.Text = string.Format("[({0:N}/{1:N})/({2:N}/{3:N})]", _mapTransform.Extent.MinX,
                                            _mapTransform.Extent.MinY, _mapTransform.Extent.MaxX,
                                            _mapTransform.Extent.MaxY);
            picMap.Image = _buffer;
        }

        private static void DrawTile(ITileSchema schema, Graphics graphics, Bitmap bitmap, RectangleF extent, string levelId)
        {
            // For drawing on WinForms there are two things to take into account
            // to prevent seams between tiles.
            // 1) The WrapMode should be set to TileFlipXY. This is related
            //    to how pixels are rounded by GDI+
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
            // 2) The rectangle should be rounded to actual pixels.
            var roundedExtent = RoundToPixel(extent);
            graphics.DrawImage(bitmap, roundedExtent, 0, 0, schema.GetTileWidth(levelId), schema.GetTileHeight(levelId), GraphicsUnit.Pixel, imageAttributes);
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofn = new OpenFileDialog())
            {
                ofn.Filter = @"MbTiles file|*.mbtiles";
                ofn.FilterIndex = 0;
                ofn.Multiselect = false;
                ofn.CheckFileExists = true;
                if (ofn.ShowDialog() == DialogResult.OK)
                {
                    _source = new MbTilesTileSource(new SQLiteConnectionString(ofn.FileName, false));
                    var extent = _source.Extent;
                    var scale = (float)(1.1 * Math.Max(extent.Width / picMap.Width, extent.Height / picMap.Height));
                    _mapTransform = new MapTransform(
                        new PointF((float)extent.CenterX, (float)extent.CenterY),
                        scale, picMap.Width, picMap.Height);

                    RenderToBuffer();
                }
            }
        }

        private void getSampleFileFromInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = Path.Combine(Path.GetTempPath(), "mapbox.haiti-terrain.mbtiles");
            var req = WebRequest.Create("http://a.tiles.mapbox.com/mapbox/download/haiti-terrain.mbtiles");
            var success = true;
            try
            {
                Enabled = false;

                var tmpFile = Path.GetTempFileName();
                using (var response = req.GetResponse())
                {
                    using (var streamWriter = new BinaryWriter(File.OpenWrite(tmpFile)))
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            var buffer = new byte[4 * 8192];
                            while (true)
                            {
                                var read = stream.Read(buffer, 0, buffer.Length);
                                if (read <= 0) break;
                                streamWriter.Write(buffer, 0, read);
                            }
                        }
                    }
                }
                _source = null;
                File.Copy(tmpFile, path, true);
                File.Delete(tmpFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                success = false;
            }
            finally
            {
                if (success)
                {
                    _source = new MbTilesTileSource(new SQLiteConnectionString(path, false));
                    var scale = (float)(1.1 * Math.Max(_source.Schema.Extent.Width / picMap.Width, _source.Schema.Extent.Height / picMap.Height));
                    _mapTransform = new MapTransform(
                        new PointF((float)_source.Schema.Extent.CenterX, (float)_source.Schema.Extent.CenterY),
                        scale, picMap.Width, picMap.Height);

                    RenderToBuffer();
                }
                Enabled = true;
            }
        }
    }
}