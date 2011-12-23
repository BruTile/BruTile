using System.Globalization;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    #region Nested type: WmsLayerBoundingBox
    /// <summary>
    /// Structure for holding information about a bounding box
    /// </summary>
    public class WmsLayerBoundingBox
    {
        /// <summary>
        /// Layer CRS that applies to this bounding box
        /// </summary>
        public string CRS;

        /// <summary>
        /// Limits of the bounding box using the axis units and order of the specified CRS
        /// </summary>
        public double minx, miny, maxx, maxy;

        /// <summary>
        /// Optional resx and resy attributes indicate the X and Y spatial resolution in the units of that CRS
        /// </summary>
        public double resx, resy;

        public WmsLayerBoundingBox(XElement xmlBoundingBox)
        {
            var crs = xmlBoundingBox.Attribute("CRS");
            if (crs == null)
                crs = xmlBoundingBox.Attribute("crs");
            if (crs == null)
                crs = xmlBoundingBox.Attribute("SRS");
            if (crs == null)
                crs = xmlBoundingBox.Attribute("srs");
            if (crs != null)
                CRS = crs.Value;

            double.TryParse(xmlBoundingBox.Attribute("minx").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out minx);
            double.TryParse(xmlBoundingBox.Attribute("miny").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out miny);
            double.TryParse(xmlBoundingBox.Attribute("maxx").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out maxx);
            double.TryParse(xmlBoundingBox.Attribute("maxy").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out maxy);

            var resxAtt = xmlBoundingBox.Attribute("resx");
            if (null != resxAtt)
                double.TryParse(resxAtt.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out resx);
            var resyAtt = xmlBoundingBox.Attribute("resy");
            if (null != resyAtt)
                double.TryParse(resyAtt.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out resy);
        }
    }
    #endregion
}