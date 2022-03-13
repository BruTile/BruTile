// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Wms
{
    public class Attribution : XmlObject
    {
        private OnlineResource _onlineResourceField;
        private LogoURL _logoUrlField;

        public Attribution()
        {
            Title = string.Empty;
        }

        public Attribution(XElement node, string @namespace)
            : this()
        {
            var element = node.Element(XName.Get("Title", @namespace));
            if (element != null) Title = element.Value;

            element = node.Element(XName.Get("OnlineResource", @namespace));
            if (element != null) OnlineResource = new OnlineResource(element, @namespace);

            element = node.Element(XName.Get("LogoURL", @namespace));
            if (element != null) LogoURL = new LogoURL(element, @namespace);
        }

        public string Title { get; set; }

        public OnlineResource OnlineResource
        {
            get => _onlineResourceField ?? (_onlineResourceField = new OnlineResource());
            set => _onlineResourceField = value;
        }

        public LogoURL LogoURL
        {
            get => _logoUrlField ?? (_logoUrlField = new LogoURL());
            set => _logoUrlField = value;
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmpty)
            {
                reader.ReadStartElement("Title");
                Title = reader.ReadContentAsString();
                reader.ReadEndElement();
                OnlineResource.ReadXml(reader);
                LogoURL.ReadXml(reader);
                reader.ReadEndElement();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Title", Namespace, Title);
            writer.WriteStartElement("OnlineResource", Namespace);
            OnlineResource.WriteXml(writer);
            writer.WriteEndElement();
            writer.WriteStartElement("LogoURL", Namespace);
            LogoURL.WriteXml(writer);
            writer.WriteEndElement();
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("Attribution", @namespace),
                new XElement(XName.Get("Title", @namespace), Title),
                OnlineResource.ToXElement(@namespace),
                LogoURL.ToXElement(@namespace));
        }
    }
}