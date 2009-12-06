﻿// Copyright 2008 - Paul den Dulk (Geodan)
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;
using Microsoft.Surface.Presentation;
using BruTile;
using DemoConfig;
using BruTileMap;
using BruTileWindows;
using System.IO;
using BruTile.Web;

namespace BruTileSurface
{
    partial class MapControl : SurfaceUserControl
    {
        #region Fields

        const double step = 1.1;
        TileLayer<MemoryStream> tileLayer;
        SurfaceTransform transform = new SurfaceTransform();
        Point previous = new Point();
        bool update = true;
        string errorMessage;
        FpsCounter fpsCounter = new FpsCounter();
        public event EventHandler ErrorMessageChanged;
        private Affine2DManipulationProcessor manipulationProcessor;
        private Affine2DInertiaProcessor inertiaProcessor;

        // 100 rotations/second squared, specified in radians (200 pi / (1000ms/s)^2)
        private const double expansionDeceleration = 0.02;

        // 100 rotations/second squared, specified in radians (200 pi / (1000ms/s)^2)
        private const double angularDeceleration = 200 * Math.PI / (1000.0 * 1000.0);

        // 24 inches/second squared (24 inches * 96 pixels per inch / (1000ms/s )^2)
        private const double deceleration = 24.0 * 96.0 / (1000.0 * 1000.0);

        // When inertia delta values get scaled by more than this amount, stop the inertia early
        private const double inertiaScaleStop = 0.8;

        Renderer renderer = new Renderer();
        
        #endregion

        #region Properties

        internal TileLayer<MemoryStream> TileLayer
        {
            get { return tileLayer; }
        }

        internal FpsCounter FpsCounter
        {
            get { return fpsCounter; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        #endregion

        #region Public methods

        public MapControl()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MapControl_Loaded);
        }

        #endregion

        #region ManipulationProcessor

        private void InitializeManipulationProcessors()
        {
            manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.Rotate | Affine2DManipulations.Scale | Affine2DManipulations.TranslateX | Affine2DManipulations.TranslateY, canvas);
            
            manipulationProcessor.Affine2DManipulationStarted += OnManipulationStarted;
            manipulationProcessor.Affine2DManipulationDelta += OnManipulationDelta;
            manipulationProcessor.Affine2DManipulationCompleted += OnManipulationCompleted;
        }

        protected override void OnContactDown(ContactEventArgs e)
        {
            base.OnContactDown(e);
            CaptureContact(e.Contact);
            e.Handled = true;
        }

        private void OnManipulationStarted(object sender, Affine2DOperationStartedEventArgs e)
        {
            if (InertiaProcessor.IsRunning)
            {
                InertiaProcessor.End();
            }
        }

        private void OnManipulationDelta(object sender, Affine2DOperationDeltaEventArgs e)
        {
            transform.ScaleAt(e.ScaleDelta, e.ManipulationOrigin);
            transform.Pan(e.Delta);
               
            tileLayer.UpdateData(transform.Extent, transform.Resolution);
            update = true;
        }

        private void OnManipulationCompleted(object sender, Affine2DOperationCompletedEventArgs e)
        {
            InertiaProcessor.InitialOrigin = e.ManipulationOrigin;

            // Set the deceleration rates
            InertiaProcessor.DesiredDeceleration = deceleration;
            InertiaProcessor.DesiredExpansionDeceleration = expansionDeceleration;
            InertiaProcessor.DesiredAngularDeceleration = angularDeceleration;

            // Set the inital values
            InertiaProcessor.InitialVelocity = e.Velocity;
            InertiaProcessor.InitialExpansionVelocity = e.ExpansionVelocity;
            InertiaProcessor.InitialAngularVelocity = e.AngularVelocity;

            // Initial Radius should be the average radius (width / 2 + height / 2) / 2 = width+height / 4, but Inertia Processor says value must be at least 1.
            //InertiaProcessor.InitialRadius = Math.Max(1, (Fractal.Viewport.RotatedWidth + Fractal.Viewport.RotatedHeight) / 4);

            // Translation constraints should ensure content stays in bounds, no need to use boundary
            //InertiaProcessor.Bounds = new Thickness(double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity, double.PositiveInfinity);

            // Start the inertia
            InertiaProcessor.Begin();
        }

        private Affine2DManipulationProcessor ManipulationProcessor
        {
            get
            {
                if (manipulationProcessor == null)
                {
                    InitializeManipulationProcessors();
                }
                return manipulationProcessor;
            }
        }

        protected override void OnGotContactCapture(ContactEventArgs e)
        {
            base.OnGotContactCapture(e);
            ManipulationProcessor.BeginTrack(e.Contact);
        }

        protected override void OnLostContactCapture(ContactEventArgs e)
        {
            base.OnLostContactCapture(e);

            // Stop tracking this contact in the manipulation processor
            ManipulationProcessor.EndTrack(e.Contact);
        }
        
        #endregion

        #region InertiaProcessor
        
        private void InitializeInertiaProcessor()
        {
            inertiaProcessor = new Affine2DInertiaProcessor();
            inertiaProcessor.Affine2DInertiaDelta += OnManipulationDelta;
        }

        private Affine2DInertiaProcessor InertiaProcessor
        {
            get
            {
                if (inertiaProcessor == null)
                {
                    InitializeInertiaProcessor();
                }
                return inertiaProcessor;
            }
        }


        #endregion 

        #region Private methods

        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitTransform();
            IConfig config = new ConfigVE();
            tileLayer = new TileLayer<MemoryStream>(config.CreateTileSource(), new TileFactory());  
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            this.MouseDown += new MouseButtonEventHandler(MapControl_MouseDown);
            this.MouseMove += new System.Windows.Input.MouseEventHandler(MapControl_MouseMove);
            this.MouseLeave += new MouseEventHandler(MapControl_MouseLeave);
            this.MouseUp += new MouseButtonEventHandler(MapControl_MouseUp);
            this.MouseWheel += new MouseWheelEventHandler(MapControl_MouseWheel);

            tileLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
            tileLayer.UpdateData(transform.Extent, transform.Resolution);

            canvas.Width = this.Width;
            canvas.Height = this.Height;
        }

        private void MapControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            previous = new Point();
        }

        private void MapControl_MouseLeave(object sender, MouseEventArgs e)
        {
            previous = new Point(); ;
        }

        private void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomIn(e.GetPosition(this));
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }

            tileLayer.UpdateData(transform.Extent, transform.Resolution);
            update = true;
            this.InvalidateVisual();
        }

        private void ZoomIn(Point mousePosition)
        {
            // When zooming in we want the mouse position to stay above the same world coordinate.
            // We do that in 3 steps.

            // 1) Temporarily center on where the mouse is
            transform.Center = transform.MapToWorld(mousePosition.X, mousePosition.Y);

            // 2) Then zoom 
            transform.Resolution /= step;

            // 3) Then move the temporary center back to the mouse position
            transform.Center = transform.MapToWorld(
              transform.Width - mousePosition.X,
              transform.Height - mousePosition.Y);
        }

        private void ZoomOut()
        {
            transform.Resolution *= step;
        }

        private void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            previous = e.GetPosition(this);
        }

        private void MapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (previous == new Point()) return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
                Point current = e.GetPosition(this);
                transform.Pan(current, previous);
                previous = current;
                tileLayer.UpdateData(transform.Extent, transform.Resolution);
                update = true;
                this.InvalidateVisual();
            }
        }

        private void InitTransform()
        {
            transform.Center = new Point(629816, 6805085);
            transform.Resolution = 1222.992452344;
            transform.Width = this.ActualWidth;
            transform.Height = this.ActualHeight;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            fpsCounter.FramePlusOne();
            if (update)
            {
                renderer.Render(this.canvas, tileLayer.Schema, transform, tileLayer.MemoryCache);
                update = false;
            }
        }

        private void tileLayer_DataUpdated(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                    new AsyncCompletedEventHandler(tileLayer_DataUpdated), sender, new object[] { e });
            }
            else
            {
                if (e.Error == null && e.Cancelled == false)
                {
                    update = true;
                    this.InvalidateVisual();
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
                else if (e.Error is System.IO.FileFormatException)
                {
                    errorMessage = "FileFormatException: " + e.Error.Message;
                    OnErrorMessageChanged();
                }
                else
                {
                    errorMessage = "Exception: " + e.Error.Message;
                    OnErrorMessageChanged();
                }
            }
        }

        protected void OnErrorMessageChanged()
        {
            if (ErrorMessageChanged != null) ErrorMessageChanged(this, null);
        }

        private void rb1_Click(object sender, RoutedEventArgs e)
        {
            IConfig config = new ConfigOsm();
            tileLayer = new TileLayer<MemoryStream>(config.CreateTileSource(), new TileFactory());
            tileLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
            tileLayer.UpdateData(transform.Extent, transform.Resolution);
            update = true;
            this.InvalidateVisual();
        }

        private void rb2_Click(object sender, RoutedEventArgs e)
        {
            IConfig config = new ConfigVE();
            tileLayer = new TileLayer<MemoryStream>(config.CreateTileSource(), new TileFactory());
            tileLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
            tileLayer.UpdateData(transform.Extent, transform.Resolution);
            update = true;
            this.InvalidateVisual();
        }

        #endregion

    }
}
