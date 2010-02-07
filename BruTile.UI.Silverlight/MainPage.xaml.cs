using System;
using System.Windows;
using System.Windows.Controls;

namespace BruTile.UI.Silverlight
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            App.Current.Host.Content.Resized += new EventHandler(Content_Resized);
            App.Current.Host.Content.FullScreenChanged += new EventHandler(Content_FullScreenChanged);
            GUI.SetMap(map);
        }

        void Content_FullScreenChanged(object sender, EventArgs e)
        {
            this.Width = App.Current.Host.Content.ActualWidth;
            this.Height = App.Current.Host.Content.ActualHeight;
        }

        void Content_Resized(object sender, EventArgs e)
        {
            this.Width = App.Current.Host.Content.ActualWidth;
            this.Height = App.Current.Host.Content.ActualHeight;
        }
    }
}
