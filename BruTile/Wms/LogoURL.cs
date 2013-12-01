// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class LogoURL : XmlObject
    {
        private OnlineResource _onlineResourceField;
        private int _width;
        private int _height;

        public LogoURL()
        {
        }

        public LogoURL(XElement node, string nameSpace)
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
            return new XElement(XName.Get("StyleURL", @namespace),
                                new XAttribute(XName.Get("width"), Width.ToString(NumberFormatInfo.InvariantInfo)),
                                new XAttribute(XName.Get("height"), Height.ToString(NumberFormatInfo.InvariantInfo)),
                                new XElement(XName.Get("Format", @namespace), Format),
                                OnlineResource.ToXElement(@namespace));
        }

        public string Format { get; set; }

        public int Width
        {
            get { return _width; }
            set
            {
                if (value < 1)
                    throw WmsPropertyException.PositiveInteger("Width", value);
                _width = value;
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                if (value < 1)
                    throw WmsPropertyException.PositiveInteger("Height", value);
                _height = value;
            }
        }

        public OnlineResource OnlineResource
        {
            get { return _onlineResourceField ?? (_onlineResourceField = new OnlineResource()); }
            set { _onlineResourceField = value; }
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