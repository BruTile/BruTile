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
using BruTile.Web;

namespace BruTileWindows
{
    public partial class MapControl : UserControl
    {
        #region Fields

        private const double step = 1.1;
        private TileLayer<MemoryStream> _rootLayer;
        private MapTransform _transform = new MapTransform();
        private Point _previousMousePosition = new Point();
        private Point _currentMousePosition = new Point();
        private bool _update = true;
        private string _errorMessage;
        private FpsCounter _fpsCounter = new FpsCounter();
        private DoubleAnimation _zoomAnimation = new DoubleAnimation();
        private Storyboard _zoomStoryBoard = new Storyboard();
        private double _toResolution;
        private bool _mouseDown = false;
        private Renderer _renderer = new Renderer();

        public event EventHandler ErrorMessageChanged;
        #endregion

        #region Properties

        public MapTransform Transform
        {
            get
            {
                return this._transform;
            }
        }

        public TileLayer<MemoryStream> RootLayer
        {
            get
            {
                return this._rootLayer;
            }
            set
            {
                this._renderer = new Renderer(); //todo reset instead of new.
                this._rootLayer = value;
                if (this._rootLayer != null)
                {
                    this._rootLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(rootLayer_DataUpdated);
                }
                this.Refresh();
            }
        }

        public FpsCounter FpsCounter
        {
            get
            {
                return this._fpsCounter;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this._errorMessage;
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
            Point mousePosition = this._currentMousePosition;
            // When zooming we want the mouse position to stay above the same world coordinate.
            // We calcultate that in 3 steps.

            // 1) Temporarily center on the mouse position
            this._transform.Center = this._transform.MapToWorld(mousePosition.X, mousePosition.Y);

            // 2) Then zoom 
            this._transform.Resolution = resolution;

            // 3) Then move the temporary center of the map back to the mouse position
            this._transform.Center = this._transform.MapToWorld(
              this._transform.Width - mousePosition.X,
              this._transform.Height - mousePosition.Y);

            this.Refresh();
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

            this.InitAnimation();
        }

        private void InitAnimation()
        {
            this._zoomAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 1000));
#if SILVERLIGHT
      _zoomAnimation.EasingFunction = new QuadraticEase();
#endif
            Storyboard.SetTarget(this._zoomAnimation, this);
            Storyboard.SetTargetProperty(this._zoomAnimation, new PropertyPath("Resolution"));
            this._zoomStoryBoard.Children.Add(this._zoomAnimation);
        }

        void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this._currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event for mousewheel event

            if (this._toResolution == 0)
            {
                this._toResolution = this._transform.Resolution;
            }

            if (e.Delta > 0)
            {
                this._toResolution = ZoomHelper.ZoomIn(this._rootLayer.Schema.Resolutions, this._toResolution);
            }
            else if (e.Delta < 0)
            {
                this._toResolution = ZoomHelper.ZoomOut(this._rootLayer.Schema.Resolutions, this._toResolution);
            }

            e.Handled = true; //so that the scroll event is not sent to the html page.

            this.StartZoomAnimation(this._transform.Resolution, this._toResolution);
        }

        public void StartZoomAnimation(double begin, double end)
        {
            this._zoomStoryBoard.Pause(); //using Stop() here causes unexpected results while zooming very fast.
            this._zoomAnimation.From = begin;
            this._zoomAnimation.To = end;
            this._zoomStoryBoard.Begin();
        }

        void MapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RectangleGeometry rect = new RectangleGeometry();
            rect.Rect = new Rect(0f, 0f, this.Width, this.Height);
            this.canvas.Clip = rect;
            this.canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.canvas.VerticalAlignment = VerticalAlignment.Stretch;
            this.canvas.Width = this.Width;
            this.canvas.Height = this.Height;
        }

        private void MapControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this._previousMousePosition = new Point();
        }

        private void MapControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this._previousMousePosition = new Point(); ;
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
                    this._errorMessage = "Cancelled";
                    this.OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Error is WebResponseFormatException)
                {
                    this._errorMessage = "UnexpectedTileFormat: " + e.Error.Message;
                    this.OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Error is System.Net.WebException)
                {
                    this._errorMessage = "WebException: " + e.Error.Message;
                    this.OnErrorMessageChanged(EventArgs.Empty);
                }
                else
                {
                    this._errorMessage = "Exception: " + e.Error.Message;
                    this.OnErrorMessageChanged(EventArgs.Empty);
                }
            }
        }

        private void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._previousMousePosition = e.GetPosition(this);
            this._mouseDown = true;
        }

        void MapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this._mouseDown = false;
        }

        private void MapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!this._mouseDown)
            {
                return;
            }
            if (this._previousMousePosition == new Point())
            {
                return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
            }

            this._currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event
            MapTransformHelpers.Pan(this._transform, this._currentMousePosition, this._previousMousePosition);
            this._previousMousePosition = this._currentMousePosition;
            this.Refresh();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this._fpsCounter.FramePlusOne();
            if (this._update)
            {
                if ((this._renderer != null) && (this._rootLayer != null))
                {
                    this._renderer.Render(this.canvas, this._rootLayer.Schema, this._transform, this._rootLayer.MemoryCache);
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

        private void Refresh()
        {
            if (this._rootLayer != null)
            {
                this._rootLayer.UpdateData(_transform.Extent, _transform.Resolution);
            }
            this._update = true;
#if !SILVERLIGHT
            this.InvalidateVisual();
#endif
        }
    }
}