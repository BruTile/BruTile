using System;
// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using BruTile.Wmts;

namespace BruTile.Wms
{
    /// <summary>
    /// This class creates a WMS schema from a WMS capabilites document, using a preferred CRC code if provided as first choice.
    /// </summary>
    public class WmsSchemaFromCapabilities : TileSchema
    {
        private readonly string _onlineResource = null;
        private int _tileSize = 256;
        private IList<string> _crsSupported;
        private IDictionary<string, string> _customParameters;
        private bool _transparent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="layerNames"></param>
        /// <param name="minScale"></param>
        /// <param name="maxScale"></param>
        /// <param name="preferredSrs"></param>
        /// <param name="tileSize"></param>
        /// <param name="boundingBoxOrderHonoursCrs">Set to false if provider is using WMS spec 1.3 but ignores CRS axis ordering</param>
        public WmsSchemaFromCapabilities(Uri uri, IList<string> layerNames, double minScale = 50, double maxScale = 2000, string preferredSrs = "", int tileSize = 256,
            bool boundingBoxOrderHonoursCrs = true  )
        {
            _tileSize = tileSize;
            LayerNames = layerNames.ToList();
            var req = WebRequest.Create(uri);
            var resp = req.GetResponseAsync();
            WmsCapabilities wmsCapabilities;
            try
            {
                using (var stream = resp.Result.GetResponseStream())
                {
                    wmsCapabilities = new WmsCapabilities(stream);
                }
            }
            catch (System.Exception ex)
            {
                Message = ex.InnerException.Message;
                return;
            }

            Version = wmsCapabilities.Version;
            bool isVersion_1_3 = wmsCapabilities.Version.Version == WmsVersionEnum.Version_1_3_0;
            Layer mainlayer = wmsCapabilities.Capability.Layer;
            Title = mainlayer.Title;

            var selectedLayers = new List<Layer>();
            getLayers(layerNames, mainlayer, selectedLayers);
            var firstLayer = selectedLayers.First();

            Srs = getSrs(mainlayer,firstLayer, preferredSrs, isVersion_1_3);
            if (!string.IsNullOrWhiteSpace(preferredSrs) && !CrsIsSupported(preferredSrs))
            {
                Message = "Crs " + preferredSrs + " is not supported by the map server.";
                return;
            }

            var axisOrder = CrsHelpers.GetOrdinateOrderFromCrs(Srs, null);
            if (axisOrder != null && axisOrder[0] == 1)
                CrsAxisOrder = CrsAxisOrder.Reversed;

            Format = "image/png";
            if (isVersion_1_3)
            {
                var exBbox = firstLayer.ExGeographicBoundingBox;
                Wgs84BoundingBox = new Extent(exBbox.WestBoundLongitude, exBbox.SouthBoundLatitude, exBbox.EastBoundLongitude, exBbox.NorthBoundLatitude);
            }
            else
            {
                var exBbox = firstLayer.LatLonBoundingBox;
                Wgs84BoundingBox = new Extent(exBbox.MinX, exBbox.MinY, exBbox.MaxX, exBbox.MaxY);
            }

            var bbox = firstLayer.BoundingBox.First(s => s.CRS == Srs); //[bboxIndex];

            bool swapAxis = CrsAxisOrder == CrsAxisOrder.Reversed && isVersion_1_3 && boundingBoxOrderHonoursCrs;
            if (swapAxis)
                BoundingBoxAxisOrderInterpretation = BoundingBoxAxisOrderInterpretation.Geographic;

            OriginX = swapAxis ? bbox.MinY : bbox.MinX;  
            OriginY = swapAxis ? bbox.MinX : bbox.MinY;

            Extent = swapAxis 
                ? new Extent(bbox.MinY, bbox.MinX, bbox.MaxY, bbox.MaxX) 
                : new Extent(bbox.MinX, bbox.MinY, bbox.MaxX, bbox.MaxY);

            Styles = getStyles(layerNames, selectedLayers);
            LayerTitles = getLayerTitles(layerNames, selectedLayers);
           
            YAxis = YAxis.TMS;  
            _onlineResource = wmsCapabilities.Capability.Request.GetCapabilities.DCPType[0].Http.Get.OnlineResource.Href; ; //info.WmsCapabilities.Capability.Request.GetCapabilities.DCPType[0].Http.Get.OnlineResource.Href;
            if (!_onlineResource.EndsWith("?"))
                _onlineResource = _onlineResource + "?";

            setResolutions(minScale, maxScale);

            var a = firstLayer.Abstract;

        }

        private string getSrs(Layer mainLayer, Layer firstLayer, string preferredSrs, bool isVersion_1_3)
        {
            IList<string> srs;
            if (isVersion_1_3 && (firstLayer.CRS != null && firstLayer.CRS.Count > 0))
                srs = firstLayer.CRS;
            else if (isVersion_1_3)
                srs = mainLayer.CRS;
            else
                srs = mainLayer.SRS;

            _crsSupported = srs;

            int srsIndex = 0;
            if (!string.IsNullOrWhiteSpace(preferredSrs))
            {
                for (int i = 0; i < srs.Count; i++)
                {
                    if (srs[i].ToUpper() == preferredSrs)
                    {
                        srsIndex = i;
                        break;
                    }
                }
            }
            return srs[srsIndex];
        }

        public IList<string> CrsSupported
        {
            get { return _crsSupported; }
        }

        private IList<object> getStyles(IList<string> layerNames, List<Layer> selectedLayers)
        {
            var styles = new List<Style>();
            for (int i = 0; i < layerNames.Count; i++)
            {
                var lyr = selectedLayers.First(s => s.Name == layerNames[i]);
                if (lyr.Style.Any(s => s.Title == lyr.Name))
                    styles.Add(lyr.Style.First(s => s.Title == lyr.Name));
                else
                    styles.Add(lyr.Style.First());
            }
            return styles.Cast<object>().ToList();
        }

        private IList<string> getLayerTitles(IList<string> layerNames, List<Layer> selectedLayers)
        {
            var titles = new List<string>();
            for (int i = 0; i < layerNames.Count; i++)
            {
                var lyr = selectedLayers.First(s => s.Name == layerNames[i]);
                if (!string.IsNullOrEmpty(lyr.Title))
                    titles.Add(lyr.Title);
                else
                    titles.Add(lyr.Name);
            }
            return titles;
        }

        private IList<string> getAbstracts(IList<string> layerNames, List<Layer> selectedLayers)
        {
            var abstracts = new List<string>();
            for (int i = 0; i < layerNames.Count; i++)
            {
                var lyr = selectedLayers.First(s => s.Name == layerNames[i]);
                if (!string.IsNullOrEmpty(lyr.Abstract))
                    abstracts.Add(lyr.Abstract);
                else
                    abstracts.Add(lyr.Name);

            }
            return abstracts;
        }



        public string Title { get; set; }

        public string Message { get; private set; }

        public string OnlineResource
        {
            get { return _onlineResource; }
        }

        public IList<string> StyleNames
        {
            get
            {
                if (Styles == null)
                    return null;
                if (Styles.Count == 0)
                    return new List<string>();
                return Styles.Cast<Style>().Select(s => s.Name).ToList();
            }
        }

        public IList<object> Images { get; set; }

        public WmsVersion Version { get; set; }

        public IDictionary<string, string> CustomParameters
        {
            get { return _customParameters; }
        }

        private void setResolutions(double min, double max)
        {

            var unitsPerPixel = max;
            int counter = 0;
            while (unitsPerPixel >= min)
            {
                var levelId = counter.ToString(CultureInfo.InvariantCulture);
                Resolutions[levelId] = new Resolution
                    (
                    levelId,
                    unitsPerPixel,
                    _tileSize,
                    _tileSize,
                    OriginX,
                    OriginY
                    );
                counter++;
                unitsPerPixel /= 2;
            }
        }

        /// <summary>
        /// Add layer recursively until desired layers have been found (each layer may contain childlayers)
        /// </summary>
        /// <param name="layerNames"></param>
        /// <param name="layer"></param>
        /// <param name="selectedLayers"></param>
        private void getLayers(IList<string> layerNames, Layer layer, IList<Layer> selectedLayers  )
        {
            if (layer.ChildLayers == null || layer.ChildLayers.Count == 0)
                return;
            foreach (var childLyr in layer.ChildLayers)
            {
                if (layerNames.Contains(childLyr.Name))
                    selectedLayers.Add(childLyr);

                if (selectedLayers.Count == layerNames.Count) // No need to search further
                    return;

                getLayers(layerNames, childLyr, selectedLayers);
            }
        }

        public bool CrsIsSupported(string authorityCode)
        {
            return _crsSupported.Any(s => s.Equals(authorityCode, StringComparison.CurrentCultureIgnoreCase));
        }

        public IList<string> LayerNames { get; set; }

        public bool Transparent
        {
            get { return _transparent; }
            set
            {
                if (value != _transparent)
                {
                    _transparent = value;
                    if (_customParameters == null)
                        _customParameters = new Dictionary<string, string>();

                    if (!_customParameters.ContainsKey("transparent"))
                        _customParameters.Add("transparent", value.ToString().ToLower());
                    else
                        _customParameters["transparent"] = value.ToString().ToLower();

                }
            }
        }

        public WmsSchemaFromCapabilities Clone()
        {
            var copy = (WmsSchemaFromCapabilities)this.MemberwiseClone();
            LayerNames = LayerNames.ToList();
            LayerTitles = LayerTitles.ToList();
            Styles = Styles.ToList();
            return (WmsSchemaFromCapabilities) copy;
        }
    }
}