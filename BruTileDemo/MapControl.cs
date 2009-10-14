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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
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
using BruTile;
using BruTileMap;
using BruTileWindows;
using System.IO;

namespace BruTileDemo
{
  class MapControl : Canvas
  {
    #region Fields

    const double step = 1.1;
    TileLayer<MemoryStream> rootLayer;
    MapTransform transform = new MapTransform();
    Point previous = new Point();
    bool update = true;
    string errorMessage;
    FpsCounter fpsCounter = new FpsCounter();
    public event EventHandler ErrorMessageChanged;
    Renderer renderer = new Renderer();

    #endregion
    
    #region Properties

    public FpsCounter FpsCounter
    {
      get { return fpsCounter; }
    }

    public string ErrorMessage
    {
      get { return errorMessage; }
    }

    public TileLayer<MemoryStream> RootLayer
    {
      get { return rootLayer; }
      set 
      { 
        rootLayer = value;
        rootLayer.DataUpdated += new System.ComponentModel.AsyncCompletedEventHandler(tileLayer_DataUpdated);
        rootLayer.UpdateData(transform.Extent, transform.Resolution);
        update = true;
        this.InvalidateVisual();
      }
    }

    #endregion

    public MapControl()
    {
      this.Loaded += new RoutedEventHandler(MapControl_Loaded);
    }
    
    void MapControl_Loaded(object sender, RoutedEventArgs e)
    {
      InitTransform();
      
      this.Background = new SolidColorBrush(Colors.Transparent);
      
      CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
      
      this.MouseDown += new MouseButtonEventHandler(MapControl_MouseDown);
      this.MouseMove += new System.Windows.Input.MouseEventHandler(MapControl_MouseMove);
      this.MouseLeave += new MouseEventHandler(MapControl_MouseLeave);
      this.MouseUp += new MouseButtonEventHandler(MapControl_MouseUp);
      this.MouseWheel += new MouseWheelEventHandler(MapControl_MouseWheel);
   }

    void MapControl_MouseUp(object sender, MouseButtonEventArgs e)
    {
      previous = new Point();
    }

    void MapControl_MouseLeave(object sender, MouseEventArgs e)
    {
      previous = new Point(); ;
    }

    void tileLayer_DataUpdated(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
      if (!this.Dispatcher.CheckAccess())
      {
        this.Dispatcher.Invoke(new AsyncCompletedEventHandler(tileLayer_DataUpdated), new object[] { sender, e });
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

    void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (e.Delta > 0)
      {
        ZoomIn(e.GetPosition(this));
      }
      else if (e.Delta < 0)
      {
        ZoomOut();
      }      
      
      rootLayer.UpdateData(transform.Extent, transform.Resolution);
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

    void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
      previous = e.GetPosition(this);
    }

    void MapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (previous == new Point()) return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
        Point current = e.GetPosition(this);
        MapTransformHelpers.Pan(transform, current, previous);
        previous = current;
        rootLayer.UpdateData(transform.Extent, transform.Resolution);
        update = true;
        this.InvalidateVisual();
      }
    }

    private void InitTransform()
    {
      transform.Center = new PointF(0, 0);
      transform.Resolution = 78271.516950000;
      transform.Width = (float)this.ActualWidth;
      transform.Height = (float)this.ActualHeight;
    }

    void CompositionTarget_Rendering(object sender, EventArgs e)
    {
      fpsCounter.FramePlusOne();
      if (update)
      {
        renderer.Render(this, rootLayer.Schema, transform, rootLayer.MemoryCache);
        update = false;
      }
    }

    protected void OnErrorMessageChanged()
    {
      if (ErrorMessageChanged != null) ErrorMessageChanged(this, null);
    }
  }
}
