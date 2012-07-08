// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class LegendURL : XmlObject
    {
        private OnlineResource _onlineResourceField;

        public LegendURL()
        {
        }

        public LegendURL(XElement node, string nameSpace)
        {
            var att = node.Attribute(XName.Get("width"));
            Width = int.Parse(att.Value, NumberFormatInfo.InvariantInfo);
            att = node.Attribute(XName.Get("height"));
            Height = int.Parse(att.Value, NumberFormatInfo.InvariantInfo);

            var element = node.Element(XName.Get("Format", nameSpace));
            Format = element == null ? "png" : element.Value;

            element = node.Element(XName.Get("OnlineResource", nameSpace));
            if (element != null)
                OnlineResource = new OnlineResource(element, nameSpace);
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("LegendURL", @namespace),
                                new XAttribute(XName.Get("width"), Width.ToString(NumberFormatInfo.InvariantInfo)),
                                new XAttribute(XName.Get("height"), Height.ToString(NumberFormatInfo.InvariantInfo)),
                                new XElement(XName.Get("Format", @namespace), Format),
                                OnlineResource.ToXElement(@namespace));
        }

        public string Format { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

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
            var val = reader.GetAttribute("width");
            if (val != null) Width = int.Parse(val, NumberFormatInfo.InvariantInfo);
            val = reader.GetAttribute("height");
            if (val != null) Height = int.Parse(val, NumberFormatInfo.InvariantInfo);

            var isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                reader.ReadStartElement("Format");
                Format = reader.ReadContentAsString();
                reader.ReadEndElement();
                OnlineResource.ReadXml(reader);
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("width", Width.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteAttributeString("height", Height.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteElementString("Format", Format);
            writer.WriteStartElement("OnlineResource", Namespace);
            OnlineResource.WriteXml(writer);
            writer.WriteEndElement();
        }

        #endregion Overrides of XmlObject
    }
}