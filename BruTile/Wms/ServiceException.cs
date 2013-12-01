// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class ServiceException : XmlObject
    {
        public ServiceException()
        {
        }

        public ServiceException(XElement node, string @namespace)
        {
            var att = node.Attribute("code");
            Code = att != null ? att.Value : string.Empty;

            att = node.Attribute("locator");
            Locator = att != null ? att.Value : string.Empty;

            Value = node.Value;
        }

        public string Code
        {
            get;
            set;
        }

        public string Locator
        {
            get;
            set;
        }

        public string Value { get; set; }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Code = reader.GetAttribute("code");
            Locator = reader.GetAttribute("locator");
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
            if (!string.IsNullOrEmpty(Code))
                writer.WriteAttributeString("code", Code);
            if (!string.IsNullOrEmpty(Locator))
                writer.WriteAttributeString("locator", Locator);
            writer.WriteString(Value);
        }

        public override XElement ToXElement(string @namespace)
        {
            var att = new List<XAttribute>();
            if (!string.IsNullOrEmpty(Code))
                att.Add(new XAttribute(XName.Get("code"), Code));
            if (!string.IsNullOrEmpty(Locator))
                att.Add(new XAttribute(XName.Get("locator"), Locator));

            return new XElement(XName.Get("ServiceException", @namespace), att.ToArray(), Value);
        }

        #endregion Overrides of XmlObject
    }
}