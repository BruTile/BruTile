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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BruTileMap;
using DemoConfig;

namespace BruTileWindows
{
  public partial class MapControl : UserControl
  {
    #region Fields

    const double step = 1.1;
    TileLayer<Image> tileLayer;
    MapTransform transform = new MapTransform();
    Point previousMousePosition = new Point();
    Point currentMousePosition = new Point();
    bool update = true;
    string errorMessage;
    FpsCounter fpsCounter = new FpsCounter();
    public event EventHandler ErrorMessageChanged;
    
    #endregion
    
    #region Properties
    
    public TileLayer<Image> TileLayer
    {
      get { return tileLayer; }
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

    public MapControl()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(MapControl_Loaded);
    }
 
    void MapControl_Loaded(object sender, RoutedEventArgs e)
    {
      InitTransform();

      bool httpResult = System.Net.WebRequest.RegisterPrefix("http://", System.Net.Browser.WebRequestCreator.ClientHttp);

      IConfig config = new ConfigVE();
      tileLayer = new TileLayer<Image>(new FetchTileWeb(config.RequestBuilder), config.TileSchema, new TileFactory());
      CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

      this.MouseLeftButtonDown += new MouseButtonEventHandler(MapControl_MouseDown); 
      this.MouseMove += new System.Windows.Input.MouseEventHandler(MapControl_MouseMove);
      this.MouseLeave += new MouseEventHandler(MapControl_MouseLeave);
      this.MouseLeftButtonUp += new MouseButtonEventHandler(MapControl_MouseUp);
      this.MouseWheel += new MouseWheelEventHandler(MapControl_MouseWheel);
      this.SizeChanged += new SizeChangedEventHandler(TestUserControl_SizeChanged);
      
      tileLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
      tileLayer.UpdateData(transform.Extent, transform.Resolution);
      update = true;
     
    }

    void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
        {
            ZoomIn(currentMousePosition);
        }
        else if (e.Delta < 0)
        {
            ZoomOut();
        }

        e.Handled = true;
        tileLayer.UpdateData(transform.Extent, transform.Resolution);
        update = true;
    }

    void TestUserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {      
      RectangleGeometry rect = new RectangleGeometry();
      rect.Rect = new Rect(0f, 0f, this.Width, this.Height);
      canvas.Clip = rect;
      canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
      canvas.VerticalAlignment = VerticalAlignment.Stretch;
      canvas.Width = this.Width;
      canvas.Height = this.Height;
    }
      
    void MapControl_MouseUp(object sender, MouseButtonEventArgs e)
    {
      previousMousePosition = new Point();
    }

    void MapControl_MouseLeave(object sender, MouseEventArgs e)
    {
      previousMousePosition = new Point(); ;
    }

    void tileLayer_DataUpdated(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
      if (!this.Dispatcher.CheckAccess())
      {
        this.Dispatcher.BeginInvoke(new AsyncCompletedEventHandler(tileLayer_DataUpdated), new object[] { sender, e });
      }
      else
      {
        if (e.Error == null && e.Cancelled == false)
        {
          update = true;
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

    private void ZoomIn(Point mousePosition)
    {
      // While zooming we want to keep the mouse at the same world location.
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

    void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
      previousMousePosition = e.GetPosition(this);
    }

    void MapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event for mousewheel event
      
      if (previousMousePosition == new Point()) return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
      MapTransformHelpers.Pan(transform, currentMousePosition, previousMousePosition);
      previousMousePosition = currentMousePosition;
      tileLayer.UpdateData(transform.Extent, transform.Resolution);
      update = true;
    }

    private void InitTransform()
    {
      transform.Center = new PointF(629816, 6805085);
      transform.Resolution = 1222.992452344;
      transform.Width = (float)this.Width;
      transform.Height = (float)this.Height;
    }

    void CompositionTarget_Rendering(object sender, EventArgs e)
    {
      fpsCounter.FramePlusOne();
      if (update)
      {
        Graphics.Render(canvas, tileLayer.Schema, transform, tileLayer.MemoryCache);
        update = false;
      }
    }

    protected void OnErrorMessageChanged()
    {
      if (ErrorMessageChanged != null) ErrorMessageChanged(this, null);
    }

  }
}
