﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using BruTile;
using SharpMap;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Rendering;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;
using SharpMapProvider;

namespace DemoConfig
{
    public class ConfigSharpMap : IConfig
    {
        string format = "jpg";
        string name = "SharpMap";

        private static double[] ScalesOsm = new double[] { 
            156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 
            9783.939618750, 4891.969809375, 2445.984904688, 1222.992452344, 
            611.496226172, 305.748113086, 152.874056543, 76.437028271, 
            38.218514136, 19.109257068, 9.554628534, 4.777314267, 
            2.388657133, 1.194328567, 0.597164283};


        public ITileProvider TileProvider
        {
            get
            {
                return new SharpMapTileProvider(CreateMap());
            }
        }

        public ITileSchema TileSchema
        {
            get
            {
                TileSchema schema = new TileSchema();
                foreach (double resolution in ScalesOsm) schema.Resolutions.Add(resolution);
                schema.Height = 256;
                schema.Width = 256;
                schema.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
                schema.OriginX = -20037508.342789;
                schema.OriginY = 20037508.342789;
                schema.Name = name;
                schema.Format = format;
                schema.Axis = AxisDirection.InvertedY;
                schema.Srs = "EPSG:3785";
                return schema;
            }
        }

        private Map CreateMap1()
        {
            Map map = new Map(256, 256);
            map.BackColor = Color.White;
            VectorLayer vectorLayer = new VectorLayer("polyline");
            VectorStyle style = new VectorStyle();
            style.EnableOutline = true;
            style.Line = new Pen(Color.Black);
            vectorLayer.Style = style;
            ShapeFile shapeFile = new ShapeFile(GetAppDir() + "\\Resources\\GeoData\\countries_out.shp");
            shapeFile.Open();
            vectorLayer.DataSource = shapeFile;
            map.Layers.Add(vectorLayer);
            return map;
        }

        public static Map CreateMap()
        {
            //Initialize a new map based on the simple map
            Map map = new Map(256, 256);

            //Set up countries layer
            VectorLayer layCountries = new VectorLayer("Countries");
            //Set the datasource to a shapefile in the App_data folder
            layCountries.DataSource = new ShapeFile(GetAppDir() + "\\Resources\\GeoData\\countries.shp", true);
            //Set fill-style to green
            layCountries.Style.Fill = new SolidBrush(Color.Green);
            //Set the polygons to have a black outline
            layCountries.Style.Outline = Pens.Black;
            layCountries.Style.EnableOutline = true;
            layCountries.SRID = 3785;
            map.Layers.Add(layCountries);

            //set up cities layer
            VectorLayer layCities = new VectorLayer("Cities");
            //Set the datasource to a shapefile in the App_data folder
            layCities.DataSource = new ShapeFile(GetAppDir() + "\\Resources\\GeoData\\cities.shp", true);
            layCities.Style.SymbolScale = 0.8f;
            layCities.MaxVisible = 10000000.0;
            layCities.SRID = 3785;
            map.Layers.Add(layCities);

            //Set up a country label layer
            LabelLayer layLabel = new LabelLayer("Country labels");
            layLabel.DataSource = layCountries.DataSource;
            layLabel.Enabled = true;
            layLabel.LabelColumn = "Name";
            layLabel.Style = new LabelStyle();
            layLabel.Style.ForeColor = Color.White;
            layLabel.Style.Font = new Font(FontFamily.GenericSerif, 12);
            layLabel.Style.BackColor = new SolidBrush(Color.FromArgb(128, 255, 0, 0));
            layLabel.MaxVisible = 20000000.0; ;
            layLabel.MinVisible = 20000000.0;
            layLabel.Style.HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center;
            layLabel.SRID = 3785;
            layLabel.MultipartGeometryBehaviour = LabelLayer.MultipartGeometryBehaviourEnum.Largest;
            map.Layers.Add(layLabel);

            //Set up a city label layer
            LabelLayer layCityLabel = new LabelLayer("City labels");
            layCityLabel.DataSource = layCities.DataSource;
            layCityLabel.Enabled = true;
            layCityLabel.LabelColumn = "Name";
            layCityLabel.Style = new LabelStyle();
            layCityLabel.Style.ForeColor = Color.Black;
            layCityLabel.Style.Font = new Font(FontFamily.GenericSerif, 11);
            layCityLabel.MaxVisible = layLabel.MinVisible;
            layCityLabel.Style.HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Left;
            layCityLabel.Style.VerticalAlignment = LabelStyle.VerticalAlignmentEnum.Bottom;
            layCityLabel.Style.Offset = new PointF(3, 3);
            layCityLabel.Style.Halo = new Pen(Color.Yellow, 2);
            layCityLabel.TextRenderingHint = TextRenderingHint.AntiAlias;
            layCityLabel.SmoothingMode = SmoothingMode.AntiAlias;
            layCityLabel.SRID = 3785;
            layCityLabel.LabelFilter = LabelCollisionDetection.ThoroughCollisionDetection;
            layCityLabel.Style.CollisionDetection = true;
            map.Layers.Add(layCityLabel);


            //Set a gradient theme on the countries layer, based on Population density
            //First create two styles that specify min and max styles
            //In this case we will just use the default values and override the fill-colors
            //using a colorblender. If different line-widths, line- and fill-colors where used
            //in the min and max styles, these would automatically get linearly interpolated.
            VectorStyle min = new VectorStyle();
            VectorStyle max = new VectorStyle();
            //Create theme using a density from 0 (min) to 400 (max)
            GradientTheme popdens = new GradientTheme("PopDens", 0, 400, min, max);
            //We can make more advanced coloring using the ColorBlend'er.
            //Setting the FillColorBlend will override any fill-style in the min and max fills.
            //In this case we just use the predefined Rainbow colorscale
            popdens.FillColorBlend = SharpMap.Rendering.Thematics.ColorBlend.Rainbow5;
            layCountries.Theme = popdens;

            //Lets scale the labels so that big countries have larger texts as well
            LabelStyle lblMin = new LabelStyle();
            LabelStyle lblMax = new LabelStyle();
            lblMin.ForeColor = Color.Black;
            lblMin.Font = new Font(FontFamily.GenericSerif, 6);
            lblMax.ForeColor = Color.Blue;
            lblMax.BackColor = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
            lblMin.BackColor = lblMax.BackColor;
            lblMax.Font = new Font(FontFamily.GenericSerif, 9);
            layLabel.Theme = new GradientTheme("PopDens", 0, 400, lblMin, lblMax);

            //Lets scale city icons based on city population
            //cities below 1.000.000 gets the smallest symbol, and cities with more than 5.000.000 the largest symbol
            VectorStyle citymin = new VectorStyle();
            VectorStyle citymax = new VectorStyle();
            string iconPath = "Resources\\Images\\icon.png";
            if (!File.Exists(iconPath))
            {
                throw new Exception(
                    String.Format("Error file '{0}' could not be found, make sure it is at the expected location",
                                  iconPath));
            }

            citymin.Symbol = new Bitmap(iconPath);
            citymin.SymbolScale = 0.5f;
            citymax.Symbol = new Bitmap(iconPath);
            citymax.SymbolScale = 1f;
            layCities.Theme = new GradientTheme("Population", 1000000, 5000000, citymin, citymax);

             map.BackColor = Color.LightBlue;

            return map;
        }

        private static string GetAppDir()
        {
            return System.IO.Path.GetDirectoryName(
              System.Reflection.Assembly.GetEntryAssembly().GetModules()[0].FullyQualifiedName);
        }
    }
}
