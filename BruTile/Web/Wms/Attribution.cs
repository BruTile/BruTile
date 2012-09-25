// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
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

        public LogoURL LogoURL
        {
            get
            {
                if ((_logoUrlField == null))
                {
                    _logoUrlField = new LogoURL();
                }
                return _logoUrlField;
            }
            set
            {
                _logoUrlField = value;
            }
        }

        #region Overrides of XmlObject

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

        #endregion Overrides of XmlObject
    }
}