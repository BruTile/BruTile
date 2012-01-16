using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Post : DCPTypeElement
    {
        public Post()
            : base("Post")
        {
        }

        public Post(XElement node, string @namespace)
            : base("Post", node, @namespace)
        { }
    }
}