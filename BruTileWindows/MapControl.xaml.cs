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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BruTileMap;

namespace BruTileWindows
{
    public partial class MapControl : UserControl
    {
        #region Fields

        const double step = 1.1;
        TileLayer<MemoryStream> rootLayer;
        MapTransform transform = new MapTransform();
        Point previousMousePosition = new Point();
        Point currentMousePosition = new Point();
        bool update = true;
        string errorMessage;
        FpsCounter fpsCounter = new FpsCounter();
        public event EventHandler ErrorMessageChanged;
        DoubleAnimation zoomAnimation = new DoubleAnimation();
        Storyboard zoomStoryBoard = new Storyboard();
        double toResolution;
        bool mouseDown = false;
        Renderer renderer = new Renderer();

        #endregion

        #region Properties

        public MapTransform Transform
        {
            get { return transform; }
        }

        public TileLayer<MemoryStream> RootLayer
        {
            get { return rootLayer; }
            set
            {
                renderer = new Renderer(); //todo reset instead of new.
                rootLayer = value;
                if (rootLayer != null)
                {
                    rootLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(rootLayer_DataUpdated);
                }
                Refresh();
            }
        }

        public FpsCounter FpsCounter
        {
            get { return fpsCounter; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty ResolutionProperty =
          System.Windows.DependencyProperty.Register(
          "Resolution", typeof(double), typeof(MapControl),
          new PropertyMetadata(new PropertyChangedCallback(OnResolutionChanged)));

        #endregion

        #region Constructors

        public MapControl()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MapControl_Loaded);
        }

        #endregion

        private static void OnResolutionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            double newResolution = (double)e.NewValue;
            ((MapControl)dependencyObject).ZoomIn(newResolution);
        }

        private void ZoomIn(double resolution)
        {
            Point mousePosition = this.currentMousePosition;
            // When zooming we want the mouse position to stay above the same world coordinate.
            // We calcultate that in 3 steps.

            // 1) Temporarily center on the mouse position
            transform.Center = transform.MapToWorld(mousePosition.X, mousePosition.Y);

            // 2) Then zoom 
            transform.Resolution = resolution;

            // 3) Then move the temporary center of the map back to the mouse position
            transform.Center = transform.MapToWorld(
              transform.Width - mousePosition.X,
              transform.Height - mousePosition.Y);

            Refresh();
        }

        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
      bool httpResult = System.Net.WebRequest.RegisterPrefix("http://", System.Net.Browser.WebRequestCreator.ClientHttp);
#endif
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(MapControl_MouseDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MapControl_MouseLeftButtonUp);
            this.MouseMove += new System.Windows.Input.MouseEventHandler(MapControl_MouseMove);
            this.MouseLeave += new MouseEventHandler(MapControl_MouseLeave);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MapControl_MouseUp);
            this.MouseWheel += new MouseWheelEventHandler(MapControl_MouseWheel);
            this.SizeChanged += new SizeChangedEventHandler(MapControl_SizeChanged);

            InitAnimation();
        }

        private void InitAnimation()
        {
            zoomAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 1000));
#if SILVERLIGHT
      zoomAnimation.EasingFunction = new QuadraticEase();
#endif
            Storyboard.SetTarget(zoomAnimation, this);
            Storyboard.SetTargetProperty(zoomAnimation, new PropertyPath("Resolution"));
            zoomStoryBoard.Children.Add(zoomAnimation);
        }

        void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event for mousewheel event

            if (toResolution == 0) toResolution = transform.Resolution;
            if (e.Delta > 0)
            {
                toResolution = ZoomHelper.ZoomIn(rootLayer.Schema.Resolutions, toResolution);
            }
            else if (e.Delta < 0)
            {
                toResolution = ZoomHelper.ZoomOut(rootLayer.Schema.Resolutions, toResolution);
            }

            e.Handled = true; //so that the scroll event is not sent to the html page.

            StartZoomAnimation(transform.Resolution, toResolution);
        }

        public void StartZoomAnimation(double begin, double end)
        {
            zoomStoryBoard.Pause(); //using Stop() here causes unexpected results while zooming very fast.
            zoomAnimation.From = begin;
            zoomAnimation.To = end;
            zoomStoryBoard.Begin();
        }

        void MapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RectangleGeometry rect = new RectangleGeometry();
            rect.Rect = new Rect(0f, 0f, this.Width, this.Height);
            canvas.Clip = rect;
            canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            canvas.VerticalAlignment = VerticalAlignment.Stretch;
            canvas.Width = this.Width;
            canvas.Height = this.Height;
        }

        private void MapControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            previousMousePosition = new Point();
        }

        private void MapControl_MouseLeave(object sender, MouseEventArgs e)
        {
            previousMousePosition = new Point(); ;
        }

        private void rootLayer_DataUpdated(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(new AsyncCompletedEventHandler(rootLayer_DataUpdated), new object[] { sender, e });
            }
            else
            {
                if (e.Error == null && e.Cancelled == false)
                {
                    Refresh();
                }
                else if (e.Cancelled == true)
                {
                    errorMessage = "Cancelled";
                    OnErrorMessageChanged();
                }
                else if (e.Error is BruTile.WebResponseFormatException)
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

        private void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            previousMousePosition = e.GetPosition(this);
            mouseDown = true;
        }

        void MapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }

        private void MapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!mouseDown) return;
            if (previousMousePosition == new Point()) return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
            currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event
            MapTransformHelpers.Pan(transform, currentMousePosition, previousMousePosition);
            previousMousePosition = currentMousePosition;
            Refresh();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            fpsCounter.FramePlusOne();
            if (update)
            {
                if ((renderer != null) && (rootLayer != null))
                {
                    renderer.Render(canvas, rootLayer.Schema, transform, rootLayer.MemoryCache);
                }
                update = false;
            }
        }

        private void OnErrorMessageChanged()
        {
            if (ErrorMessageChanged != null) ErrorMessageChanged(this, null);
        }

        private void Refresh()
        {
            if (rootLayer != null)
                rootLayer.UpdateData(transform.Extent, transform.Resolution);
            update = true;
#if !SILVERLIGHT
            this.InvalidateVisual();
#endif
        }

    }
}