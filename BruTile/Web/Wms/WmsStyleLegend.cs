using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    #region Nested type: WmsStyleLegend

    /// <summary>
    /// Structure for storing WMS Legend information
    /// </summary>
    public class WmsStyleLegend
    {
        /// <summary>
        /// Online resource for legend style 
        /// </summary>
        public readonly WmsOnlineResource OnlineResource;

        /// <summary>
        /// Size of legend
        /// </summary>
        public int Width;
        public int Height;

        public WmsStyleLegend(XElement xmlStyleLegend, WmsNamespaces namespaces)
        {
            Width = int.Parse(xmlStyleLegend.Attribute(XName.Get("width")).Value);
            Height = int.Parse(xmlStyleLegend.Attribute(XName.Get("height")).Value);

            OnlineResource = new WmsOnlineResource(xmlStyleLegend, namespaces);
        }
    }

    #endregion
}