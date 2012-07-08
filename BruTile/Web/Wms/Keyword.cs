// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Keyword : XmlObject
    {
        public string Vocabulary { get; set; }

        public string Value { get; set; }

        public Keyword()
        {
        }

        public Keyword(XElement node, string @namespace)
        {
            var att = node.Attribute(XName.Get("Vocabulary", @namespace));
            Vocabulary = att != null ? att.Value : string.Empty;

            Value = node.Value;
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Vocabulary = reader.GetAttribute("vocabulary");
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
            if (string.IsNullOrEmpty(Vocabulary))
                writer.WriteAttributeString("vocabulary", Vocabulary);
            writer.WriteString(Value);
        }

        public override XElement ToXElement(string @namespace)
        {
            if (string.IsNullOrEmpty(Vocabulary))
                return new XElement(XName.Get("Keyword", @namespace), Value);

            return new XElement(XName.Get("Keyword", @namespace),
                new XAttribute(XName.Get("vocabulary"), Vocabulary), Value);
        }

        #endregion Overrides of XmlObject
    }
}