using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BruTileSilverlight;
using System.Text;
using System.IO;
using System.Net;
using System.Globalization;

namespace BruTile.UI.Silverlight
{
	public partial class GoTo : UserControl
	{
        GUIOverlay gui;

		public GoTo()
		{
			InitializeComponent();
            HideGoTo.Completed += new EventHandler(HideGoTo_Completed);
		}

        public void SetGui(GUIOverlay gui)
        {
            this.gui = gui;
        }

        void HideGoTo_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            HideGoTo.Begin();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            String city = cityBox.Text;
            String country = countryBox.Text;
            String street = streetBox.Text;
            String requestString = "http://tinygeocoder.com/create-api.php?q=";

            if (!street.Equals(""))
                requestString += street;
            if (!street.Equals("") && !city.Equals(""))
                requestString += ", ";
            if (!city.Equals(""))
                requestString += city;
            if (!city.Equals("") && !country.Equals(""))
                requestString += ", ";
            if (!country.Equals(""))
                requestString += country;

            Uri uri = new Uri(requestString);

            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(uri);
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            String result = e.Result;
            if (result.Equals(""))
            {
                MessageBox.Show("Could not find location");
            }
            else
            {
                String[] lonLat = result.Split(',');

                if (lonLat.Length == 2)
                {
                    if (!streetBox.Text.Equals(""))
                        gui.map.Transform.Resolution = 0.597164283;
                    else if (!cityBox.Text.Equals(""))
                        gui.map.Transform.Resolution = 9.554628534;
                    else if (!countryBox.Equals(""))
                        gui.map.Transform.Resolution = 611.496226172;

                    Point sphericalLocation = SphericalMercator.FromLonLat(Double.Parse(lonLat[1], CultureInfo.InvariantCulture), Double.Parse(lonLat[0], CultureInfo.InvariantCulture));
                    gui.map.Transform.Center = sphericalLocation;
                    //Toresolution has to be set somehow
                    gui.map.Refresh();
                    HideGoTo.Begin();
                }
                else
                {
                    MessageBox.Show("Can not use the returned values");
                }
            }
        }
	}

    public class SphericalMercator
    {
        private readonly static double radius = 6378137;
        private static double D2R = Math.PI / 180;
        private static double HALF_PI = Math.PI / 2;

        public static Point FromLonLat(double lon, double lat)
        {
            double lonRadians = (D2R * (double)lon);
            double latRadians = (D2R * (double)lat);

            double x = radius * lonRadians;
            double y = radius * Math.Log(Math.Tan(Math.PI * 0.25 + latRadians * 0.5));

            return new Point((float)x, (float)y);
        }

        public static Point ToLonLat(double x, double y)
        {
            double ts;
            ts = Math.Exp(-y / (radius));
            double latRadians = HALF_PI - 2 * Math.Atan(ts);

            double lonRadians = x / (radius);

            double lon = (lonRadians / D2R);
            double lat = (latRadians / D2R);

            return new Point((float)lon, (float)lat);
        }
    }
}