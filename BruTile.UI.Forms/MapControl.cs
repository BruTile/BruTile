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
using System.Drawing;
using System.Windows.Forms;
using BruTile;
using BruTile.UI;
using BruTile.Web;
using BruTile.UI.Fetcher;

namespace BruTile.UI.Forms
{
    public class MapControl : System.Windows.Forms.Control
    {
        #region Fields

        private TileLayer rootLayer;
        private MapTransform transform = new MapTransform();
        private string errorMessage;
        private Bitmap buffer;
        private Graphics bufferGraphics;
        private Brush whiteBrush = new SolidBrush(Color.White);
        private PointF mousePosition;

        #endregion

        #region Events

        public event EventHandler ErrorMessageChanged;

        #endregion

        #region Properties

        public MapTransform Transform
        {
            get { return transform; }
        }

        public TileLayer RootLayer
        {
            get { return rootLayer; }
            set
            {
                if (RootLayer != null)
                {
                    rootLayer.DataChanged -= new DataChangedEventHandler(rootLayer_DataChanged);
                    rootLayer.AbortFetch();
                }
                rootLayer = value;
                if (rootLayer != null)
                    rootLayer.DataChanged += new DataChangedEventHandler(rootLayer_DataChanged);
                Refresh();
            }
        }

        #endregion

        #region Public methods

        public MapControl()
        {
            this.Resize += new EventHandler(MapControl_Resize);
            this.MouseDown += new MouseEventHandler(MapControl_MouseDown);
            this.MouseMove += new MouseEventHandler(MapControl_MouseMove);
            this.Disposed += new EventHandler(MapControl_Disposed);
        }

        void MapControl_Disposed(object sender, EventArgs e)
        {
            if (rootLayer != null) rootLayer.AbortFetch();
        }

        public void ZoomIn()
        {
            transform.Resolution = ZoomHelper.ZoomIn(rootLayer.Schema.Resolutions, transform.Resolution);
            Refresh();
        }

        public void ZoomIn(PointF mapPosition)
        {
            // When zooming in we want the mouse position to stay above the same world coordinate.
            // We do that in 3 steps.

            // 1) Temporarily center on where the mouse is
            transform.Center = transform.MapToWorld(mapPosition.X, mapPosition.Y);

            // 2) Then zoom 
            transform.Resolution = ZoomHelper.ZoomIn(rootLayer.Schema.Resolutions, transform.Resolution);

            // 3) Then move the temporary center back to the mouse position
            transform.Center = transform.MapToWorld(
              transform.Width - mapPosition.X,
              transform.Height - mapPosition.Y);

            Refresh();
        }

        public void ZoomOut()
        {
            transform.Resolution = ZoomHelper.ZoomOut(rootLayer.Schema.Resolutions, transform.Resolution);

            Refresh();
        }

        #endregion

        #region Private and Protected methods

        protected override void OnPaint(PaintEventArgs e)
        {
            //Reset background
            bufferGraphics.FillRectangle(whiteBrush, 0, 0, buffer.Width, buffer.Height);
            //Render to the buffer
            if (rootLayer != null)
                Renderer.Render(bufferGraphics, rootLayer.Schema, transform, rootLayer.MemoryCache);
            //Render the buffer to the control
            e.Graphics.DrawImage(buffer, 0, 0);

            base.OnPaint(e);
        }

        protected void OnErrorMessageChanged()
        {
            if (ErrorMessageChanged != null) ErrorMessageChanged(this, null);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //by overriding this method and not calling the base class implementation 
            //we prevent flickering.
        }

        protected override void Dispose(bool disposing)
        {
            rootLayer.DataChanged -= new DataChangedEventHandler(rootLayer_DataChanged);
            base.Dispose(disposing);
        }

        public override void Refresh()
        {
            if (rootLayer != null)
                rootLayer.ViewChanged(transform.Extent, transform.Resolution);
            this.Invalidate();
        }

        private void rootLayer_DataChanged(object sender, DataChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new DataChangedEventHandler(rootLayer_DataChanged), new object[] { sender, e });
                }
                catch (ObjectDisposedException ex)
                {
                    //sorry about this, but i could not prevent this exception on application close in CF.
                    //Removing the EventHandlers and if (!this.IsDisposed) seems to have no effect
                }
            }
            else
            {
                if (e.Error == null && e.Cancelled == false)
                {
                    this.Invalidate();
                }
                else if (e.Cancelled == true)
                {
                    errorMessage = "Cancelled";
                    OnErrorMessageChanged();
                }
                else if (e.Error is WebResponseFormatException)
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

        private void MapControl_MouseDown(object sender, MouseEventArgs e)
        {
            mousePosition = new PointF(e.X, e.Y);
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (mousePosition == null) return;
                PointF newMousePosition = new PointF(e.X, e.Y);
                MapTransformHelpers.Pan(transform, newMousePosition, mousePosition);
                mousePosition = newMousePosition;

                Refresh();
            }
        }

        private void MapControl_Resize(object sender, EventArgs e)
        {
            if (this.Width == 0) return;
            if (this.Height == 0) return;

            transform.Width = (float)this.Width;
            transform.Height = (float)this.Height;

            if (buffer == null || buffer.Width != this.Width || buffer.Height != this.Height)
            {
                buffer = new Bitmap(this.Width, this.Height);
                bufferGraphics = Graphics.FromImage(buffer);
            }
        }

        #endregion
    }
}
