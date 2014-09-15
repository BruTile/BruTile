// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Identifier : XmlObject
    {
        public Identifier()
        {
        }

        public Identifier(XElement el, string ns)
        {
            var att = el.Attribute("authority");
            Authority = att != null ? att.Value : string.Empty;

            Value = el.Value;
        }

        public string Authority { get; set; }

        [System.Xml.Serialization.XmlTextAttribute]
        public string Value { get; set; }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Authority = reader.GetAttribute("authority");
            var isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmpty)
            {
                Value = reader.ReadContentAsString();
                reader.ReadEndElement();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(Authority))
                writer.WriteAttributeString("authority", Authority);
            writer.WriteString(Value);
        }

        public override XElement ToXElement(string @namespace)
        {
            if (string.IsNullOrEmpty(Authority))
                return new XElement(XName.Get("Identifier", @namespace), Value);
            return new XElement(XName.Get("Identifier", @namespace),
                new XAttribute(XName.Get("authority"), Authority), Value);
        }

        #endregion Overrides of XmlObject
    }
}