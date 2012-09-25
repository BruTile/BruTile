// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Style : XmlObject
    {
        private List<LegendURL> _legendURLField;

        private StyleSheetURL _styleSheetURLField;

        private StyleURL _styleURLField;

        public Style()
        { }

        public Style(XElement node, string @namespace)
        {
            var element = node.Element(XName.Get("Name", @namespace));
            if (element == null)
                throw WmsParsingException.ElementNotFound("Name");
            Name = element.Value;

            element = node.Element(XName.Get("Title", @namespace));
            if (element == null)
                throw WmsParsingException.ElementNotFound("Title");
            Title = element.Value;

            element = node.Element(XName.Get("Abstract", @namespace));
            if (element != null)
                Abstract = element.Value;

            foreach (var el in node.Elements(XName.Get("LegendURL", @namespace)))
                LegendURL.Add(new LegendURL(el, @namespace));

            element = node.Element(XName.Get("StyleSheetURL", @namespace));
            if (element != null)
                StyleSheetURL = new StyleSheetURL(element, @namespace);

            element = node.Element(XName.Get("StyleURL", @namespace));
            if (element != null)
                StyleURL = new StyleURL(element, @namespace);
        }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Abstract { get; set; }

        public List<LegendURL> LegendURL
        {
            get
            {
                if ((_legendURLField == null))
                {
                    _legendURLField = new List<LegendURL>();
                }
                return _legendURLField;
            }
            set
            {
                _legendURLField = value;
            }
        }

        public StyleSheetURL StyleSheetURL
        {
            get
            {
                if ((_styleSheetURLField == null))
                {
                    _styleSheetURLField = new StyleSheetURL();
                }
                return _styleSheetURLField;
            }
            set
            {
                _styleSheetURLField = value;
            }
        }

        public StyleURL StyleURL
        {
            get
            {
                if ((_styleURLField == null))
                {
                    _styleURLField = new StyleURL();
                }
                return _styleURLField;
            }
            set
            {
                _styleURLField = value;
            }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }
            Name = Title = Abstract = null;
            LegendURL.Clear();
            _styleSheetURLField = null;
            _styleURLField = null;

            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "Name":
                            Name = reader.ReadElementContentAsString();
                            break;
                        case "Title":
                            Title = reader.ReadElementContentAsString();
                            break;
                        case "Abstract":
                            Abstract = reader.ReadElementContentAsString();
                            break;
                        case "LegendURL":
                            var legendUrl = new LegendURL();
                            legendUrl.ReadXml(reader);
                            LegendURL.Add(legendUrl);
                            break;
                        case "StyleSheetURL":
                            _styleSheetURLField = new StyleSheetURL();
                            _styleSheetURLField.ReadXml(reader);
                            break;
                        case "":
                            _styleURLField = new StyleURL();
                            _styleURLField.ReadXml(reader);
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
            writer.WriteElementString("Name", Namespace, Name);
            writer.WriteElementString("Title", Namespace, Title);
            if (!string.IsNullOrEmpty(Abstract))
                writer.WriteElementString("Abstract", Namespace, Abstract);

            foreach (var legendURL in LegendURL)
            {
                writer.WriteStartElement("LegendURL");
                legendURL.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (_styleSheetURLField != null)
            {
                writer.WriteStartElement("StyleSheetURL");
                _styleSheetURLField.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (_styleURLField != null)
            {
                writer.WriteStartElement("StyleURL");
                _styleURLField.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public override XElement ToXElement(string @namespace)
        {
            var elements = new List<XElement>(
                new[] {
                        new XElement(XName.Get("Name", @namespace), Name),
                        new XElement(XName.Get("Title", @namespace), Title),
                      });
            if (!string.IsNullOrEmpty(Abstract))
                elements.Add(new XElement(XName.Get("Abstract", @namespace), Abstract));

            if (LegendURL.Count > 0)
            {
                foreach (var legendURL in LegendURL)
                    elements.Add(legendURL.ToXElement(@namespace));
            }

            if (_styleSheetURLField != null)
                elements.Add(_styleSheetURLField.ToXElement(@namespace));
            if (_styleURLField != null)
                elements.Add(_styleURLField.ToXElement(@namespace));
            return new XElement(XName.Get("Style", @namespace), elements.ToArray());
        }

        #endregion Overrides of XmlObject
    }
}