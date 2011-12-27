using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Get : DCPTypeElement
    {
        public Get()
            : base("Get")
        {
        }

        public Get(XElement node, string @namespace)
            : base("Get", node, @namespace)
        { }
    }
}