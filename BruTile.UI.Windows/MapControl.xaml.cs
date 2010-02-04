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
using BruTile.Web;

namespace BruTile.UI.Windows
{
    public partial class MapControl : UserControl
    {
        #region Fields

        private const double step = 1.1;
        private TileLayer rootLayer;
        private MapTransform transform = new MapTransform();
        private Point previousMousePosition = new Point();
        private Point currentMousePosition = new Point();
        private bool _update = true;
        private string errorMessage;
        private FpsCounter fpsCounter = new FpsCounter();
        private DoubleAnimation zoomAnimation = new DoubleAnimation();
        private Storyboard zoomStoryBoard = new Storyboard();
        private double toResolution;
        private bool mouseDown = false;
        public bool isCtrlDown = false;
        private Renderer renderer = new Renderer();

        public event EventHandler ErrorMessageChanged;
        #endregion

        #region Properties

        public MapTransform Transform
        {
            get
            {
                return this.transform;
            }
        }

        public TileLayer RootLayer
        {
            get
            {
                return this.rootLayer;
            }
            set
            {
                this.renderer = new Renderer(); //todo reset instead of new.
                this.rootLayer = value;
                if (this.rootLayer != null)
                {
                    this.rootLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(rootLayer_DataUpdated);
                }
                this.Refresh();
            }
        }

        public FpsCounter FpsCounter
        {
            get
            {
                return this.fpsCounter;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
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
#if SILVERLIGHT
            bool httpResult = System.Net.WebRequest.RegisterPrefix("http://", System.Net.Browser.WebRequestCreator.ClientHttp);
#endif
            this.Loaded += new RoutedEventHandler(this.MapControl_Loaded);
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
            this.transform.Center = this.transform.MapToWorld(mousePosition.X, mousePosition.Y);

            // 2) Then zoom 
            this.transform.Resolution = resolution;

            // 3) Then move the temporary center of the map back to the mouse position
            this.transform.Center = this.transform.MapToWorld(
              this.transform.Width - mousePosition.X,
              this.transform.Height - mousePosition.Y);

            this.Refresh();
        }

        public void ZoomIn()
        {
            if (this.toResolution == 0)
                this.toResolution = this.transform.Resolution;

            this.toResolution = ZoomHelper.ZoomIn(this.rootLayer.Schema.Resolutions, this.toResolution);
            ZoomMiddle();
        }

        public void ZoomOut()
        {
            if (this.toResolution == 0)
                this.toResolution = this.transform.Resolution;

            this.toResolution = ZoomHelper.ZoomOut(this.rootLayer.Schema.Resolutions, this.toResolution);
            ZoomMiddle();
        }

        private void ZoomMiddle()
        {
            this.currentMousePosition = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            this.StartZoomAnimation(this.transform.Resolution, this.toResolution);
        }

        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(MapControl_MouseDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MapControl_MouseLeftButtonUp);
            this.MouseMove += new System.Windows.Input.MouseEventHandler(MapControl_MouseMove);
            this.MouseLeave += new MouseEventHandler(MapControl_MouseLeave);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MapControl_MouseUp);
            this.MouseWheel += new MouseWheelEventHandler(MapControl_MouseWheel);
            this.SizeChanged += new SizeChangedEventHandler(MapControl_SizeChanged);
            this.KeyDown += new KeyEventHandler(MapControl_KeyDown);
            this.KeyUp += new KeyEventHandler(MapControl_KeyUp);
            InitAnimation();

            UpdateSize();
            this.Refresh();

#if !SILVERLIGHT
                 this.Focusable = true;
#endif

#if SILVERLIGHT
            this.IsTabStop = true;
#endif

            this.Focus();
        }

        private void InitAnimation()
        {
            this.zoomAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 1000));
#if SILVERLIGHT
            zoomAnimation.EasingFunction = new QuarticEase();
#endif
            Storyboard.SetTarget(this.zoomAnimation, this);
            Storyboard.SetTargetProperty(this.zoomAnimation, new PropertyPath("Resolution"));
            this.zoomStoryBoard.Children.Add(this.zoomAnimation);
        }

        void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event for mousewheel event

            if (this.toResolution == 0)
            {
                this.toResolution = this.transform.Resolution;
            }

            if (e.Delta > 0)
            {
                this.toResolution = ZoomHelper.ZoomIn(this.rootLayer.Schema.Resolutions, this.toResolution);
            }
            else if (e.Delta < 0)
            {
                this.toResolution = ZoomHelper.ZoomOut(this.rootLayer.Schema.Resolutions, this.toResolution);
            }

            e.Handled = true; //so that the scroll event is not sent to the html page.

            this.StartZoomAnimation(this.transform.Resolution, this.toResolution);
        }

        public void StartZoomAnimation(double begin, double end)
        {
            this.zoomStoryBoard.Pause(); //using Stop() here causes unexpected results while zooming very fast.
            this.zoomAnimation.From = begin;
            this.zoomAnimation.To = end;
            this.zoomStoryBoard.Begin();
        }

        void MapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSize();
            this.Refresh();
        }

        private void UpdateSize()
        {
            RectangleGeometry rect = new RectangleGeometry();
            rect.Rect = new Rect(0f, 0f, this.ActualWidth, this.ActualHeight);
            this.canvas.Clip = rect;
            this.canvas.Width = this.ActualWidth;
            this.canvas.Height = this.ActualHeight;
            if (this.Transform != null)
            {
                this.Transform.Width = (float)this.ActualWidth;
                this.Transform.Height = (float)this.ActualHeight;
            }
        }

        private void MapControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.previousMousePosition = new Point();
        }

        private void MapControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.previousMousePosition = new Point(); ;
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
                    this.Refresh();
                }
                else if (e.Cancelled == true)
                {
                    this.errorMessage = "Cancelled";
                    this.OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Error is WebResponseFormatException)
                {
                    this.errorMessage = "UnexpectedTileFormat: " + e.Error.Message;
                    this.OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Error is System.Net.WebException)
                {
                    this.errorMessage = "WebException: " + e.Error.Message;
                    this.OnErrorMessageChanged(EventArgs.Empty);
                }
                else
                {
                    this.errorMessage = "Exception: " + e.Error.Message;
                    this.OnErrorMessageChanged(EventArgs.Empty);
                }
            }
        }

        private void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.previousMousePosition = e.GetPosition(this);
            this.mouseDown = true;
            this.Focus();
        }

        void MapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isCtrlDown)
            {
                Point min = this.Transform.MapToWorld(previousMousePosition.X, previousMousePosition.Y);
                Point max = this.Transform.MapToWorld(e.GetPosition(this).X, e.GetPosition(this).Y);
                this.ZoomToBbox(min, max);
            }
            this.mouseDown = false;
        }

        private void MapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isCtrlDown)
            {
                DrawBbox(e.GetPosition(this));
                return;
            }
            if (!this.mouseDown)
            {
                return;
            }
            if (this.previousMousePosition == new Point())
            {
                return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
            }

            this.currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event
            MapTransformHelper.Pan(this.transform, this.currentMousePosition, this.previousMousePosition);
            this.previousMousePosition = this.currentMousePosition;
            this.Refresh();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this.fpsCounter.FramePlusOne();
            if (this._update)
            {
                if ((this.renderer != null) && (this.rootLayer != null))
                {
                    this.renderer.Render(this.canvas, this.rootLayer.Schema, this.transform, this.rootLayer.MemoryCache);
                }
                this._update = false;
            }
        }

        protected virtual void OnErrorMessageChanged(EventArgs e)
        {
            if (this.ErrorMessageChanged != null)
            {
                this.ErrorMessageChanged(this, e);
            }
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            //TODO: this method should be private and any refresh to the control should be authomatic.
            //In this version, this method is public to allow users to perfrom actions like "Pan to"; those operations can
            //be performed in two steps, by setting the mapControl.Transform.Center's property, and then calling Refresh();
            if (this.rootLayer != null)
            {
                this.rootLayer.UpdateData(transform.Extent, transform.Resolution);
            }
            this._update = true;
#if !SILVERLIGHT
            this.InvalidateVisual();
#endif
        }

        #region BBOX zoom

        void MapControl_KeyUp(object sender, KeyEventArgs e)
        {
            String keyName = e.Key.ToString().ToLower();
            if (keyName.Equals("ctrl") || keyName.Equals("leftctrl") || keyName.Equals("rightctrl"))
            {
                isCtrlDown = false;
            }
        }

        void MapControl_KeyDown(object sender, KeyEventArgs e)
        {
            String keyName = e.Key.ToString().ToLower();
            if (keyName.Equals("ctrl") || keyName.Equals("leftctrl") || keyName.Equals("rightctrl"))
            {
                isCtrlDown = true;
            }
        }

        public void ZoomToBbox(Point min, Point max)
        {
            double x, y, resolution;
            ZoomHelper.ZoomToBoudingbox(min.X, min.Y, max.X, max.Y, this.ActualWidth, out x, out y, out resolution);

            this.Transform.Center = new Point(x, y);
            this.Transform.Resolution = resolution;
            this.toResolution = resolution;

            this.Refresh();
            ClearBBoxDrawing();
        }

        void ClearBBoxDrawing()
        {
            bboxRect.Margin = new Thickness(0, 0, 0, 0);
            bboxRect.Width = 0;
            bboxRect.Height = 0;
        }

        void DrawBbox(Point newPos)
        {
            if (mouseDown)
            {
                Point from = previousMousePosition;
                Point to = newPos;

                if (from.X > to.X)
                {
                    Point temp = from;
                    from.X = to.X;
                    to.X = temp.X;
                }

                if (from.Y > to.Y)
                {
                    Point temp = from;
                    from.Y = to.Y;
                    to.Y = temp.Y;
                }

                bboxRect.Width = to.X - from.X;
                bboxRect.Height = to.Y - from.Y;
                bboxRect.Margin = new Thickness(from.X, from.Y, 0, 0);
            }
        }

        #endregion
    }
}