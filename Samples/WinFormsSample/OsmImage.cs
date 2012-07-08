using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using BruTile;
using BruTile.Web;

namespace WinFormsSample
{
    public class OsmImage : Control
    {
        public OsmImage()
        {
            OsmServer = KnownOsmTileServers.Mapnic;
        }
        
        private Bitmap _buffer;
        private MapTransform _mapTransform;
        private OsmTileSource _source;

        private string _apiKey;

        /// <summary>
        /// Gets or sets the Api key
        /// </summary>
        [DefaultValue("")]
        public string ApiKey
        {
            get { return _apiKey; }
            set { _apiKey = value; }
        }

        private KnownOsmTileServers _osmRenderer;

        /// <summary>
        /// Gets or sets the default OpenStreetmap renderer
        /// </summary>
        [DefaultValue(KnownOsmTileServers.Mapnic)]
        public KnownOsmTileServers OsmServer
        {
            get { return _osmRenderer; }
            set
            {
                if (_osmRenderer == value)
                    return;

                if (value == KnownOsmTileServers.Custom)
                    return;

                _osmRenderer = value;
                _source = new OsmTileSource(new OsmRequest(OsmTileServerConfig.Create(value, ApiKey)));
                RenderToBuffer();
            }
        }

        private bool _showGrid;
        private bool _showExtent;

        [DefaultValue(true)]
        public bool ShowGrid
        {
            get { return _showGrid; }
            set
            {
                _showGrid = value;
                RenderToBuffer();
            }
        }

        [DefaultValue(true)]
        public bool ShowExtent
        {
            get { return _showExtent; }
            set
            {
                _showExtent = value;
                RenderToBuffer();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_buffer, 0, 0);
            base.OnPaint(e);
        }

        protected override void OnResize(System.EventArgs e)
        {
            _buffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            if (_mapTransform == null)
                _mapTransform = new MapTransform(new PointF(0, 0), 50000f, ClientSize.Width,ClientSize.Height);
            else
                _mapTransform = new MapTransform(_mapTransform.Center, _mapTransform.Resolution, ClientSize.Width, ClientSize.Height);
            RenderToBuffer();
            base.OnResize(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var pt = _mapTransform.MapToWorld(e.X, e.Y);
                _mapTransform = new MapTransform(pt, _mapTransform.Resolution, Width, Height);
                RenderToBuffer();
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var res = _mapTransform.Resolution;
            float factor;
            if (e.Delta < 0)
                factor = 1.2f;
            else if (e.Delta > 0)
                factor = 1 / 1.2f;
            else
                return;

            var center = new PointF(Width * 0.5f, Height * 0.5f);
            var point = new PointF(e.X, e.Y);

            var dx = (center.X - point.X) * factor;
            var dy = (center.Y - point.Y) * factor;

            var newCenter = _mapTransform.MapToWorld(point.X + dx, point.Y + dy);

            var transform = new MapTransform(newCenter, res * factor, Width, Height);
            _mapTransform = transform;
            RenderToBuffer();

            base.OnMouseWheel(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Select();
            base.OnMouseEnter(e);
        }

        private void RenderToBuffer()
        {
            if (_mapTransform == null)
                return;

            var levelIndex = Utilities.GetNearestLevel(_source.Schema.Resolutions, _mapTransform.Resolution);

            using (var g = Graphics.FromImage(_buffer))
            {
                g.Clear(Color.White);
                foreach (var tileInfo in _source.Schema.GetTilesInView(_mapTransform.Extent, levelIndex))
                {
                    byte[] res;
                    try
                    {
                        res = _source.Provider.GetTile(tileInfo);
                    }
                    catch
                    {
                        continue;
                    }

                    var extent = _mapTransform.WorldToMap(tileInfo.Extent.MinX, tileInfo.Extent.MinY,
                                                          tileInfo.Extent.MaxX, tileInfo.Extent.MaxY);
                    if (res != null)
                    {
                        var tileStream = new MemoryStream(res);
                        var tile = (Bitmap) Image.FromStream(tileStream);

                        DrawTile(_source.Schema, g, tile, extent);
                    }
                    if (_showGrid)
                    {
                        var roundedExtent = RoundToPixel(extent);
                        g.DrawRectangle(Pens.Black, roundedExtent);
                        g.DrawString(
                            string.Format("({2}:{0},{1})", tileInfo.Index.Col, tileInfo.Index.Row,
                                          tileInfo.Index.LevelId),
                            new Font("Arial", 8, FontStyle.Regular), Brushes.OrangeRed, roundedExtent,
                            new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center,
                                    Trimming = StringTrimming.None
                                });
                    }
                }

                if (_showExtent)
                    g.DrawRectangle(new Pen(Color.Tomato, 2), RoundToPixel(_mapTransform.WorldToMap(_source.Extent)));
            }
            Invalidate();
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

        private void GetTileOnThread(object parameter)
        {
            //object[] parameters = (object[])parameter;
            //if (parameters.Length != 4) throw new ArgumentException("Three parameters expected");
            //ITileProvider tileProvider = (ITileProvider)parameters[0];
            //TileInfo tileInfo = (TileInfo)parameters[1];
            //MemoryCache<Bitmap> bitmaps = (MemoryCache<Bitmap>)parameters[2];
            //AutoResetEvent autoResetEvent = (AutoResetEvent)parameters[3];


            //byte[] bytes;
            //try
            //{
            //    bytes = tileProvider.GetTile(tileInfo);
            //    Bitmap bitmap = new Bitmap(new MemoryStream(bytes));
            //    bitmaps.Add(tileInfo.Index, bitmap);
            //    if (_fileCache != null)
            //    {
            //        AddImageToFileCache(tileInfo, bitmap);
            //    }
            //}
            //catch (WebException ex)
            //{
            //    if (_showErrorInTile)
            //    {
            //        //an issue with this method is that one an error tile is in the memory cache it will stay even
            //        //if the error is resolved. PDD.
            //        Bitmap bitmap = new Bitmap(_source.Schema.Width, _source.Schema.Height);
            //        Graphics graphics = Graphics.FromImage(bitmap);
            //        graphics.DrawString(ex.Message, new Font(FontFamily.GenericSansSerif, 12), new SolidBrush(Color.Black),
            //            new RectangleF(0, 0, _source.Schema.Width, _source.Schema.Height));
            //        bitmaps.Add(tileInfo.Index, bitmap);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //todo: log and use other ways to report to user.
            //}
            //finally
            //{
            //    autoResetEvent.Set();
            //}
        }

    }
}