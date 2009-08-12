// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston,min MA  02111-1307  USA 

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using Tiling;
using Microsoft.Win32;
using System.Collections;
using System.IO;
using BruTileMap;
using DemoConfig;
using BruTileDemo;

namespace BruTileClient
{
    class MapControl : System.Windows.Forms.Control
    {
        #region private variables
        private TileLayer<Bitmap> rootLayer;
        private MapTransform transform;
        private PointF previous = new PointF();
        private bool update = true;
        private string errorMessage;
        private Bitmap buffer;
        private Graphics graphics;
        private Brush brush = new SolidBrush(Color.White);
        private double[] resolutions;
        bool firstTime = true;
        #endregion

        #region Events
        public event EventHandler ErrorMessageChanged;
        #endregion

        #region Properties
        public TileLayer<Bitmap> RootLayer
        {
            get { return rootLayer; }
        }
        #endregion

        public MapControl()
        {
            this.Resize += new EventHandler(MapControl_Resize);
        }

        void MapControl_Resize(object sender, EventArgs e)
        {
            if (this.Width == 0) return;
            if (this.Height == 0) return;
            if (buffer == null || buffer.Width != this.Width || buffer.Height != this.Height)
            {
                buffer = new Bitmap(this.Width, this.Height);
                graphics = Graphics.FromImage(buffer);
            }
        }

        void MapControl_MouseUp(object sender, MouseEventArgs e)
        {
            previous = new PointF();
        }

        void MapControl_MouseLeave(object sender, MouseEventArgs e)
        {
            previous = new PointF(); ;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (firstTime)
            {
                Initialize();
                firstTime = false;
            }

            if (update)
            {
                graphics.FillRectangle(brush, 0, 0, buffer.Width, buffer.Height);
                Renderer.Render(graphics, rootLayer.Schema, transform, rootLayer.MemoryCache);

                e.Graphics.DrawImage(buffer, 0, 0);

                update = false;
            }
            base.OnPaint(e);
        }

        public override void Refresh()
        {
            UpdateTiles();
        }

        void tileLayer_DataUpdated(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new AsyncCompletedEventHandler(tileLayer_DataUpdated), new object[] { sender, e });
                }
                catch (ObjectDisposedException ex)
                {
                    //sorry about this, but i could not prevent this exception on application close.
                    //Removing the EventHandlers and if (!this.IsDisposed) seem to have no effect. 
                }
            }
            else
            {
                if (e.Error == null && e.Cancelled == false)
                {
                    update = true;
                    this.Invalidate();
                }
                else if (e.Cancelled == true)
                {
                    errorMessage = "Cancelled";
                    OnErrorMessageChanged();
                }
                else if (e.Error is Tiling.WebResponseFormatException)
                {
                    errorMessage = "UnexpectedTileFormat: " + e.Error.Message;
                    OnErrorMessageChanged();
                }
                else if (e.Error is System.Net.WebException)
                {
                    errorMessage = "WebException: " + e.Error.Message;
                    OnErrorMessageChanged();
                }
                else
                {
                    errorMessage = "Exception: " + e.Error.Message;
                    OnErrorMessageChanged();
                }
            }
        }

        public void ZoomIn()
        {
            transform.Resolution = ZoomHelper.ZoomIn(resolutions, transform.Resolution);

            UpdateTiles();
        }

        public void ZoomIn(PointF mousePosition)
        {
            // When zooming in we want the mouse position to stay above the same world coordinate.
            // We do that in 3 steps.

            // 1) Temporarily center on where the mouse is
            transform.Center = transform.MapToWorld(mousePosition.X, mousePosition.Y);

            // 2) Then zoom 
            transform.Resolution = ZoomHelper.ZoomIn(resolutions, transform.Resolution);

            // 3) Then move the temporary center back to the mouse position
            transform.Center = transform.MapToWorld(
              transform.Width - mousePosition.X,
              transform.Height - mousePosition.Y);

            UpdateTiles();
        }

        public void ZoomOut()
        {
            transform.Resolution = ZoomHelper.ZoomOut(resolutions, transform.Resolution);

            UpdateTiles();
        }

        void MapControl_MouseDown(object sender, MouseEventArgs e)
        {
            previous = new PointF(e.X, e.Y);
        }

        void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (previous == new PointF()) return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
                PointF current = new PointF(e.X, e.Y);
                transform.Pan(current, previous);
                previous = current;

                UpdateTiles();
            }
        }

        protected void OnErrorMessageChanged()
        {
            if (ErrorMessageChanged != null) ErrorMessageChanged(this, null);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //do nothing to prevent flickering
        }

        private void Initialize()
        {
            transform = new MapTransform(new PointF(629816f, 6805085f), 1222.992452344f, this.Width, this.Height);

            IConfig config = new ConfigOsm();

            resolutions = new double[config.TileSchema.Resolutions.Count];
            config.TileSchema.Resolutions.CopyTo(resolutions, 0);

            rootLayer = new TileLayer<Bitmap>(new WebTileProvider(config.RequestBuilder), config.TileSchema, new TileFactory());

            this.MouseDown += new MouseEventHandler(MapControl_MouseDown);
            this.MouseMove += new MouseEventHandler(MapControl_MouseMove);
            this.MouseUp += new MouseEventHandler(MapControl_MouseUp);

            rootLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
            rootLayer.UpdateData(transform.Extent, transform.Resolution);
        }

        private void UpdateTiles()
        {
            rootLayer.UpdateData(transform.Extent, transform.Resolution);
            update = true;
            this.Invalidate();
        }

        public void LoadOsmLayer()
        {
            IConfig config = new ConfigOsm();
            rootLayer = new TileLayer<Bitmap>(new WebTileProvider(config.RequestBuilder), config.TileSchema, new TileFactory());
            rootLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
            rootLayer.UpdateData(transform.Extent, transform.Resolution);
            update = true;
            this.Invalidate();
        }

        public void LoadVELayer()
        {
            IConfig config = new ConfigVE();
            rootLayer = new TileLayer<Bitmap>(new WebTileProvider(config.RequestBuilder), config.TileSchema, new TileFactory());
            rootLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
            rootLayer.UpdateData(transform.Extent, transform.Resolution);
            update = true;
            this.Invalidate();
        }

        public void LoadGeodanLayer()
        {
            IConfig config = new ConfigTms();
            rootLayer = new TileLayer<Bitmap>(new WebTileProvider(config.RequestBuilder), config.TileSchema, new TileFactory());
            rootLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
            rootLayer.UpdateData(transform.Extent, transform.Resolution);
            update = true;
            this.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            rootLayer.DataUpdated -= new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
            rootLayer.Dispose();
            base.Dispose(disposing);
        }
    }
}
