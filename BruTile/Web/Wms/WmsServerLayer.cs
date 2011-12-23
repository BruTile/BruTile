using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    #region Nested type: WmsServerLayer

    /// <summary>
    /// Structure for holding information about a WMS Layer 
    /// </summary>
    public class WmsServerLayer
    {
        /// <summary>
        /// Abstract
        /// </summary>
        public string Abstract;

        /// <summary>
        /// Collection of child layers
        /// </summary>
        public IList<WmsServerLayer> ChildLayers;

        /// <summary>
        /// Coordinate Reference Systems supported by layer
        /// </summary>
        public IList<string> Crs;

        /// <summary>
        /// Keywords
        /// </summary>
        public IList<string> Keywords;

        /// <summary>
        /// Latitudal/longitudal extent of this layer
        /// </summary>
        public Extent LatLonBoundingBox;

        /// <summary>
        /// Unique name of this layer used for requesting layer
        /// </summary>
        public string Name;

        /// <summary>
        /// Specifies whether this layer is queryable using GetFeatureInfo requests
        /// </summary>
        public bool Queryable;

        /// <summary>
        /// Specifies whether this layer is opaque
        /// </summary>
        public bool Opaque;

        /// <summary>
        /// Specifies whether this layer is cascaded
        /// </summary>
        public bool Cascaded;

        /// <summary>
        /// List of styles supported by layer
        /// </summary>
        public IList<WmsLayerStyle> Style;

        /// <summary>
        /// Layer title
        /// </summary>
        public string Title;

        /// <summary>
        /// Bounding Boxes
        /// </summary>
        public IList<WmsLayerBoundingBox> BoundingBoxes;

        public WmsServerLayer(XElement xmlLayerNode, WmsNamespaces namespaces)
        {
            var node = xmlLayerNode.Element(XName.Get("Name", namespaces.Wms));
            Name = (node != null ? node.Value : string.Empty);

            node = xmlLayerNode.Element(XName.Get("Title", namespaces.Wms));
            Title = (node != null ? node.Value : string.Empty);

            node = xmlLayerNode.Element(XName.Get("Abstract", namespaces.Wms));
            Abstract = (node != null ? node.Value : string.Empty);

            var attr = xmlLayerNode.Attribute("queryable");
            Queryable = (attr != null && attr.Value == "1");

            attr = xmlLayerNode.Attribute("opaque");
            Opaque = (attr != null && attr.Value == "1");

            attr = xmlLayerNode.Attribute("cascaded");
            Cascaded = (attr != null && attr.Value == "1");

            var xmlKeywords = xmlLayerNode.Elements(XName.Get("KeywordList", namespaces.Wms));
            if (xmlKeywords != null)
            {
                Keywords = new List<string>();
                foreach (var xmlKeyword in xmlKeywords.Elements(XName.Get("Keyword", namespaces.Wms)))
                {
                    Keywords.Add(xmlKeyword.Value);
                }
            }

            var xmlCrs = xmlLayerNode.Elements(XName.Get("CRS", namespaces.Wms));
            Crs = new List<string>();
            foreach (var crs in xmlCrs)
                Crs.Add(crs.Value);

            if (Crs.Count == 0)
            {
                xmlCrs = xmlLayerNode.Elements(XName.Get("SRS", namespaces.Wms));
                foreach (var crs in xmlCrs)
                        Crs.Add(crs.Value);
            }

            var xmlStyles = xmlLayerNode.Elements(XName.Get("Style", namespaces.Wms));
            if (xmlStyles != null)
            {
                Style = new List<WmsLayerStyle>();
                foreach (var xmlStyle in xmlStyles)
                {
                    Style.Add(new WmsLayerStyle(xmlStyle, namespaces));
                }
            }

            var xmlLayers = xmlLayerNode.Elements(XName.Get("Layer", namespaces.Wms));
            if (xmlLayers != null)
            {
                ChildLayers = new List<WmsServerLayer>();
                foreach (var xmlLayer in xmlLayers)
                {
                    ChildLayers.Add(new WmsServerLayer(xmlLayer, namespaces));
                }
            }

            node = xmlLayerNode.Element(XName.Get("LatLonBoundingBox", namespaces.Wms));
            if (node != null)
            {
                double minx, miny, maxx, maxy;

                if (!double.TryParse(node.Attribute("minx").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out minx) &
                    !double.TryParse(node.Attribute("miny").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out miny) &
                    !double.TryParse(node.Attribute("maxx").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out maxx) &
                    !double.TryParse(node.Attribute("maxy").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out maxy))
                    throw new ArgumentException("Invalid LatLonBoundingBox on layer '" + Name + "'");
                LatLonBoundingBox = new Extent(minx, miny, maxx, maxy);
            }

            var xmlBoundingBoxes = xmlLayerNode.Elements(XName.Get("BoundingBox", namespaces.Wms));
            if (xmlBoundingBoxes != null)
            {
                BoundingBoxes = new List<WmsLayerBoundingBox>();
                foreach (var xmlBoundingBox in xmlBoundingBoxes)
                {
                    BoundingBoxes.Add(new WmsLayerBoundingBox(xmlBoundingBox));
                }
            }
        }

        public WmsServerLayer()
        {
        }
    }

    #endregion
}