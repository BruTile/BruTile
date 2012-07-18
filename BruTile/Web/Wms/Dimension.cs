// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Dimension : XmlObject
    {
        public Dimension()
        {
        }

        public Dimension(XElement el, string ns)
        {
            var att = el.Attribute("name");
            if (att == null)
                throw WmsParsingException.AttributeNotFound("name");
            Name = att.Value;

            att = el.Attribute("units");
            if (att == null)
                throw WmsParsingException.AttributeNotFound("units");
            Units = att.Value;

            att = el.Attribute("unitSymbol");
            UnitSymbol = att != null ? att.Value : string.Empty;

            att = el.Attribute("default");
            Default = att != null ? att.Value : string.Empty;

            att = el.Attribute("multipleValues");
            if (att == null)
                MultipleValues = null;
            else
                MultipleValues = att.Value == "1";

            att = el.Attribute("nearestValue");
            if (att == null)
                NearestValue = null;
            else
                NearestValue = att.Value == "1";

            att = el.Attribute("current");
            if (att == null)
                Current = null;
            else
                Current = att.Value == "1";

            Value = el.Value;
        }

        public string Name { get; set; }

        public string Units { get; set; }

        public string UnitSymbol { get; set; }

        public string Default { get; set; }

        public bool? MultipleValues { get; set; }

        public bool? NearestValue { get; set; }

        public bool? Current { get; set; }

        public string Value { get; set; }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            Name = reader.GetAttribute("name");
            Units = reader.GetAttribute("units");
            UnitSymbol = reader.GetAttribute("unitSymbol");
            Default = reader.GetAttribute("default");
            var att = reader.GetAttribute("multipleValues");
            if (!string.IsNullOrEmpty(att)) MultipleValues = att == "1";
            att = reader.GetAttribute("nearestValue");
            if (!string.IsNullOrEmpty(att)) NearestValue = att == "1";
            att = reader.GetAttribute("current");
            if (!string.IsNullOrEmpty(att)) Current = att == "1";

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
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("units", Units);
            if (!string.IsNullOrEmpty(UnitSymbol))
                writer.WriteAttributeString("unitSymbol", UnitSymbol);
            if (!string.IsNullOrEmpty(Default))
                writer.WriteAttributeString("default", Default);
            if (MultipleValues.HasValue)
                writer.WriteAttributeString("multipleValues", MultipleValues.Value ? "1" : "0");
            if (NearestValue.HasValue)
                writer.WriteAttributeString("nearestValue", NearestValue.Value ? "1" : "0");
            if (Current.HasValue)
                writer.WriteAttributeString("current", Current.Value ? "1" : "0");
            writer.WriteValue(Value);
        }

        public override XElement ToXElement(string @namespace)
        {
            var lst = new List<XAttribute>(new[] {
                new XAttribute(XName.Get("name"), Name),
                new XAttribute(XName.Get("units"), Units)}
                );

            if (!string.IsNullOrEmpty(UnitSymbol))
                lst.Add(new XAttribute(XName.Get("unitSymbol"), UnitSymbol));
            if (!string.IsNullOrEmpty(Default))
                lst.Add(new XAttribute(XName.Get("default"), Default));
            if (MultipleValues.HasValue)
                lst.Add(new XAttribute(XName.Get("multipleValues"), MultipleValues.Value ? "1" : "0"));
            if (NearestValue.HasValue)
                lst.Add(new XAttribute(XName.Get("nearestValue"), NearestValue.Value ? "1" : "0"));
            if (Current.HasValue)
                lst.Add(new XAttribute(XName.Get("current"), Current.Value ? "1" : "0"));

            return new XElement(XName.Get("Dimension", @namespace), lst.ToArray(), Value);
        }

        #endregion Overrides of XmlObject
    }
}