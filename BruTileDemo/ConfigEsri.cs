using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;

namespace BruTileDemo
{
    class ConfigEsri : IConfig
    {
        private static double[] resoltions = new double[] { 
            0.3515625,
            0.17578125,
            0.087890625,
            0.0439453125,
            0.02197265625,
            0.010986328125,
            0.0054931640625,
            0.00274658203125,
            0.001373291015625,
            0.0006866455078125,
            0.00034332275390625,
            0.000171661376953125,
            0.0000858306884765629,
            0.0000429153442382814,
            0.0000214576721191407,
            0.0000107288360595703 };

        private static Extent extent = new Extent(-180, -90, 180, 90);
        string name = "ESRI";
        string url = "http://server.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer/tile/{0}/{2}/{1}";
        string format = "jpeg";

        #region IConfig Members

        public ITileCache<byte[]> FileCache
        {
            get { return null; }
        }

        public IRequestBuilder RequestBuilder
        {
            get
            {
                return new RequestBasic(url);
            }
        }

        public ITileSchema TileSchema
        {
            get
            {
                TileSchema schema = new TileSchema();
                foreach (double resolution in resoltions) schema.Resolutions.Add(resolution);
                schema.Height = 512;
                schema.Width = 512;
                schema.Extent = extent;
                schema.OriginX = -180;
                schema.OriginY = 90;
                schema.Name = name;
                schema.Format = format;
                schema.Axis = AxisDirection.InvertedY;
                schema.Srs = "EPSG:4326";
                return schema;
            }
        }

        #endregion
    }
}
