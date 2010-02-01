using System;
using System.Collections.Generic;
using System.Text;
using BruTile;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using System.Net;

namespace BruTile.Web
{
    public class TileSourceBing : ITileSource
    {
        TileSchema tileSchema;
        WebTileProvider tileProvider;

        public TileSourceBing(String url)
        {
            tileSchema = CreateSchema();
            tileProvider = new WebTileProvider(CreateRequestBuilder(new Uri(url)));
        }

        private static TileSchema CreateSchema()
        {
            double[] resolutions = new double[] { 
                    78271.516950000, 39135.758475000, 19567.879237500, 
                    9783.939618750, 4891.969809375, 2445.984904688, 1222.992452344, 
                    611.496226172, 305.748113086, 152.874056543, 76.437028271, 
                    38.218514136, 19.109257068, 9.554628534, 4.777314267, 
                    2.388657133, 1.194328567, 0.597164283, 0.298582142};

            TileSchema schema = new TileSchema();
            foreach (double resolution in resolutions) schema.Resolutions.Add(resolution);
            schema.Height = 256;
            schema.Width = 256;
            schema.Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789);
            schema.OriginX = -20037508.342789;
            schema.OriginY = 20037508.342789;
            schema.Name = "Bing";
            schema.Format = "jpg";
            schema.Axis = AxisDirection.InvertedY;
            schema.Srs = "EPSG:3785";
            return schema;
        }

        private IRequestBuilder CreateRequestBuilder(Uri url)
        {
            IRequestBuilder request;
            request = new RequestVE(url.ToString(), null);

            return request;
        }

        #region ITileSource Members

        public ITileProvider Provider
        {
            get { return tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return tileSchema; }
        }

        #endregion
    }
}
