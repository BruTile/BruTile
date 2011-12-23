using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    #region Nested type: WmsLayerStyle

    /// <summary>
    /// Structure for storing information about a WMS Layer Style
    /// </summary>
    public class WmsLayerStyle
    {
        /// <summary>
        /// Abstract
        /// </summary>
        public string Abstract;

        /// <summary>
        /// Legend
        /// </summary>
        public WmsStyleLegend LegendUrl;

        /// <summary>
        /// Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Style Sheet Url
        /// </summary>
        public WmsOnlineResource StyleSheetUrl;

        /// <summary>
        /// Title
        /// </summary>
        public string Title;

        public WmsLayerStyle(XElement xmlLayerStyle, WmsNamespaces namespaces)
        {
            var node = xmlLayerStyle.Element(XName.Get("Name", namespaces.Wms));
            Name = (node != null ? node.Value : string.Empty);

            node = xmlLayerStyle.Element(XName.Get("Title", namespaces.Wms));
            Title = (node != null ? node.Value : string.Empty);

            node = xmlLayerStyle.Element(XName.Get("Abstract", namespaces.Wms));
            Abstract = (node != null ? node.Value : string.Empty);

            node = xmlLayerStyle.Element(XName.Get("LegendUrl", namespaces.Wms));
            if (node != null)
            {
                LegendUrl = new WmsStyleLegend(node, namespaces);
            }

            node = xmlLayerStyle.Element(XName.Get("StyleSheetURL", namespaces.Wms));
            if (node != null)
            {
                StyleSheetUrl = new WmsOnlineResource(node, namespaces);
            }
        }
    }

    #endregion
}