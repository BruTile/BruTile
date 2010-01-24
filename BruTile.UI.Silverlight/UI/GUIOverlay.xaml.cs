using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DemoConfig;
using System.IO;
using BruTile;
using System.Windows.Threading;
using BruTile.UI.Windows;

namespace BruTileSilverlight
{
    public partial class GUIOverlay : UserControl
    {
        MapControl map;
        bool isMenuDown = false;

        public GUIOverlay()
        {
            InitializeComponent();
            hideMenu.Completed += new EventHandler(hideMenu_Completed);
            showMenu.Completed += new EventHandler(showMenu_Completed);
            this.Loaded += new RoutedEventHandler(GUIOverlay_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(GUIOverlay_SizeChanged);
        }

        internal void SetMap(MapControl map)
        {
            this.map = map;
        }

        void SetClip()
        {
            RectangleGeometry geom = new RectangleGeometry();
            geom.Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

            this.Clip = geom;
        }

        #region layer handling

        private void Osm_Click(object sender, RoutedEventArgs e)
        {
            ITileSource source = new ConfigOsm().CreateTileSource();
            map.RootLayer = new TileLayer(source);
        }

        private void BingMaps_Click(object sender, RoutedEventArgs e)
        {
            ITileSource source = new ConfigVE().CreateTileSource();
            map.RootLayer = new TileLayer(source);
        }

        #endregion

        #region menu animation events

        void showMenu_Completed(object sender, EventArgs e)
        {
            isMenuDown = true;
        }

        private void showBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            menuShowHideOn.Visibility = Visibility.Collapsed;
        }

        private void showBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            menuShowHideOn.Visibility = Visibility.Visible;
        }

        private void ShowMenuStart()
        {
            showBtn.Visibility = Visibility.Collapsed;
            showMenu.Begin();
        }

        private void showBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowMenuStart();
        }

        void hideMenu_Completed(object sender, EventArgs e)
        {
            menuShowHideOn.Visibility = Visibility.Visible;
            showBtn.Visibility = Visibility.Visible;
            isMenuDown = false;
        }

        private void hideBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            menuShowHideOff.Visibility = Visibility.Collapsed;
        }

        private void hideBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            menuShowHideOff.Visibility = Visibility.Visible;
        }

        private void hideBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            hideMenu.Begin();
        }

        # endregion

        #region event handlers

        void GUIOverlay_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetClip();
        }

        void GUIOverlay_Loaded(object sender, RoutedEventArgs e)
        {
            SetClip();
        }

        private void buttonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            map.ZoomIn();
        }

        private void buttonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            map.ZoomOut();
        }

        private void btnLayers_Click(object sender, RoutedEventArgs e)
        {
            if (!isMenuDown)
                ShowMenuStart();
        }

        #endregion

        #region TopTooltip

        private void buttonZoomIn_MouseEnter(object sender, MouseEventArgs e)
        {
            txtTooltipTop.Text = "Zoom In";
            ShowTopTooltip.Begin();
        }

        private void buttonZoomIn_MouseLeave(object sender, MouseEventArgs e)
        {
            HideTopTooltip.Begin();
        }

        private void buttonZoomOut_MouseEnter(object sender, MouseEventArgs e)
        {
            txtTooltipTop.Text = "Zoom Out";
            ShowTopTooltip.Begin();
        }

        private void buttonZoomOut_MouseLeave(object sender, MouseEventArgs e)
        {
            HideTopTooltip.Begin();
        }

        private void buttonMaxExtend_MouseEnter(object sender, MouseEventArgs e)
        {
            txtTooltipTop.Text = "Not Implemented Yet";
            ShowTopTooltip.Begin();
        }

        private void buttonMaxExtend_MouseLeave(object sender, MouseEventArgs e)
        {
            HideTopTooltip.Begin();
        }

        #endregion

        #region lowertooltip

        private void btnLayers_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowLowerTooltip.Begin();
            txtTooltipBottom.Text = "Layers";
        }

        private void btnLayers_MouseLeave(object sender, MouseEventArgs e)
        {
            HideLowerTooltip.Begin();
        }

        #endregion
    }
}