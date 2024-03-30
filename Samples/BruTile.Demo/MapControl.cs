// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Samples.Common;

namespace BruTile.Demo;

internal class MapControl : Grid
{
    private Fetcher<Image> _fetcher;
    private readonly Renderer _renderer;
    private readonly MemoryCache<Tile<Image>> _tileCache = new(200, 300);
    private ITileSource _tileSource;
    private bool _invalid = true;
    private Point _previousMousePosition;
    private Viewport _viewport;

    public MapControl()
    {
        var canvas = new Canvas
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = new SolidColorBrush(Colors.Transparent),
        };

        Children.Add(canvas);
        _renderer = new Renderer(canvas);

        _tileSource = KnownTileSources.Create();
        CompositionTarget.Rendering += CompositionTargetRendering;
        SizeChanged += MapControlSizeChanged;
        MouseWheel += MapControlMouseWheel;
        MouseMove += MapControlMouseMove;
        MouseUp += OnMouseUp;
        MouseLeave += OnMouseLeave;

        ClipToBounds = true;
        _fetcher = new Fetcher<Image>(_tileSource, _tileCache);
        _fetcher.DataChanged += FetcherOnDataChanged;
    }

    public void SetTileSource(ITileSource source)
    {
        _fetcher.DataChanged -= FetcherOnDataChanged;
        _fetcher.AbortFetch();

        _tileSource = source;
        _viewport.CenterX = source.Schema.Extent.CenterX;
        _viewport.CenterY = source.Schema.Extent.CenterY;
        _viewport.UnitsPerPixel = Math.Max(source.Schema.Extent.Width / ActualWidth, source.Schema.Extent.Height / ActualHeight);
        _tileCache.Clear();
        _fetcher = new Fetcher<Image>(_tileSource, _tileCache);
        _fetcher.DataChanged += FetcherOnDataChanged;
        _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel);
        _invalid = true;
    }

    private void OnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
    {
        _previousMousePosition = new Point();
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        _previousMousePosition = new Point();
    }

    private void MapControlMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        if (_previousMousePosition == new Point())
        {
            _previousMousePosition = e.GetPosition(this);
            return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
        }

        var currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event
        _viewport.Transform(currentMousePosition.X, currentMousePosition.Y, _previousMousePosition.X, _previousMousePosition.Y);
        _previousMousePosition = currentMousePosition;
        _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel);
        _invalid = true;
    }

    private void MapControlSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_viewport == null) return;
        _viewport.Width = ActualWidth;
        _viewport.Height = ActualHeight;
        _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel);
        _invalid = true;
    }

    private void FetcherOnDataChanged(object sender, DataChangedEventArgs<Image> e)
    {
        if (!Dispatcher.CheckAccess())
            Dispatcher.Invoke(() => FetcherOnDataChanged(sender, e));
        else
        {
            if (e.Error == null && e.Tile != null)
            {
                e.Tile.Image = TileToImage(e.Tile.Data);
                _tileCache.Add(e.Tile.Info.Index, e.Tile);
                _invalid = true;
            }
        }
    }

    private static Image TileToImage(byte[] tile)
    {
        var stream = new MemoryStream(tile);

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.EndInit();

        var image = new Image();
        image.BeginInit();
        image.Source = bitmapImage;
        image.EndInit();
        return image;
    }

    private void MapControlMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
        {
            _viewport.UnitsPerPixel = ZoomHelper.ZoomIn(_tileSource.Schema.Resolutions.Select(r => r.Value.UnitsPerPixel).ToList(), _viewport.UnitsPerPixel);
        }
        else if (e.Delta < 0)
        {
            _viewport.UnitsPerPixel = ZoomHelper.ZoomOut(_tileSource.Schema.Resolutions.Select(r => r.Value.UnitsPerPixel).ToList(), _viewport.UnitsPerPixel);
        }

        _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel);
        e.Handled = true; //so that the scroll event is not sent to the html page.
        _invalid = true;
    }

    private void CompositionTargetRendering(object sender, EventArgs e)
    {
        if (!_invalid) return;
        if (_renderer == null) return;

        if (_viewport == null)
        {
            if (!TryInitializeViewport(ref _viewport, ActualWidth, ActualHeight, _tileSource.Schema)) return;
            _fetcher.ViewChanged(_viewport.Extent, _viewport.UnitsPerPixel); // start fetching when viewport is first initialized
        }

        _renderer.Render(_viewport, _tileSource, _tileCache);
        _invalid = false;
    }

    private static bool TryInitializeViewport(ref Viewport viewport, double actualWidth, double actualHeight, ITileSchema schema)
    {
        if (double.IsNaN(actualWidth)) return false;
        if (actualWidth <= 0) return false;

        var nearestLevel = Utilities.GetNearestLevel(schema.Resolutions, schema.Extent.Width / actualWidth);

        viewport = new Viewport
        {
            Width = actualWidth,
            Height = actualHeight,
            UnitsPerPixel = schema.Resolutions[nearestLevel].UnitsPerPixel,
            Center = new Samples.Common.Geometries.Point(schema.Extent.CenterX, schema.Extent.CenterY)
        };
        return true;
    }
}
