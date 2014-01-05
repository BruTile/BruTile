using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class AuthorityURL : XmlObject
    {
        private OnlineResource _onlineResourceField;

        public AuthorityURL()
        {
        }

        public AuthorityURL(XElement node, string @namespace)
        {
            var att = node.Attribute("name");
            if (att == null)
                throw WmsParsingException.AttributeNotFound("name");

            Name = att.Value;

            var element = node.Element(XName.Get("OnlineResource", @namespace));
            if (element != null)
                OnlineResource = new OnlineResource(element, @namespace);
        }

        public string Name { get; set; }

        public OnlineResource OnlineResource
        {
            get
            {
                if ((_onlineResourceField == null))
                {
                    _onlineResourceField = new OnlineResource();
                }
                return _onlineResourceField;
            }
            set
            {
                _onlineResourceField = value;
            }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Name = reader.GetAttribute("name");

            var isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                OnlineResource.ReadXml(reader);
                reader.ReadEndElement();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", Name);
            writer.WriteStartElement("OnlineResource", Namespace);
            OnlineResource.WriteXml(writer);
            writer.WriteEndElement();
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("AuthorityURL", @namespace),
                                new XAttribute(XName.Get("name"), Name),
                                OnlineResource.ToXElement(@namespace));
        }

        #endregion Overrides of XmlObject
    }
}