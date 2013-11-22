// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class DataURL : XmlObject
    {
        private OnlineResource _onlineResourceField;

        public DataURL()
        { }

        public DataURL(XElement node, string ns)
        {
            var element = node.Element(XName.Get("Format", ns));
            if (element == null)
                throw WmsParsingException.ElementNotFound("Format");
            Format = element.Value;

            element = node.Element(XName.Get("OnlineResource", ns));
            if (element == null)
                throw WmsParsingException.ElementNotFound("OnlineResource");
            OnlineResource = new OnlineResource(element, ns);
        }

        public string Format { get; set; }

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
            if (CheckEmptyNode(reader, "DataURL", Namespace))
                return;

            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "Format":
                            Format = reader.ReadElementContentAsString();
                            break;
                        case "OnlineResource":
                            OnlineResource = new OnlineResource();
                            OnlineResource.ReadXml(reader);
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
                else
                {
                    reader.Read();
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Format", Namespace, Format);
            writer.WriteStartElement("OnlineResource", Namespace);
            OnlineResource.WriteXml(writer);
            writer.WriteEndElement();
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("DataURL", @namespace),
                new XElement(XName.Get("Format", @namespace), Format),
                OnlineResource.ToXElement(@namespace));
        }

        #endregion Overrides of XmlObject
    }
}