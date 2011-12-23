using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    #region Nested type: WmsOnlineResource

    /// <summary>
    /// Structure for storing info on an Online Resource
    /// </summary>
    public struct WmsOnlineResource
    {
        /// <summary>
        /// URI of online resource
        /// </summary>
        public string OnlineResource;

        /// <summary>
        /// Type of online resource (Ex. request method 'Get' or 'Post')
        /// </summary>
        public string Type;

        public WmsOnlineResource(XElement xmlStyleLegend, WmsNamespaces namespaces)
        {
            OnlineResource = xmlStyleLegend.Element(XName.Get("OnlineResource", namespaces.Wms)).Attribute(XName.Get("href", WmsNamespaces.Xlink)).ToString();
            Type = xmlStyleLegend.Element(XName.Get("Format", namespaces.Wms)).Value;
        }
    }

    #endregion
}