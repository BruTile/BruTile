using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class WmsNamespaces
    {
        private readonly string _wms;

        public WmsNamespaces()
            :this(string.Empty)
        {
        }
        public WmsNamespaces(string sm)
        {
            _wms = sm;
        }

        public string Wms
        {
            get { return _wms; }
        }

        public static string Xlink
        {
            get { return "http://www.w3.org/1999/xlink"; }
        }

        public static XNamespace Xsi
        {
            get { return "http://www.w3.org/2001/XMLSchema-instance"; }
        }
    }
}