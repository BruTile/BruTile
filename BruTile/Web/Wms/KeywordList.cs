// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class KeywordList : XmlObject
    {
        private List<Keyword> _keywordField;

        public KeywordList()
        {
        }

        public KeywordList(XElement node, string @namespace)
        {
            foreach (var element in node.Elements(XName.Get("Keyword", @namespace)))
            {
                Keyword.Add(new Keyword(element, @namespace));
            }
        }

        public List<Keyword> Keyword
        {
            get
            {
                if (_keywordField == null)
                {
                    _keywordField = new List<Keyword>();
                }
                return _keywordField;
            }
            set
            {
                _keywordField = value;
            }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            Keyword.Clear();

            var subReader = reader.ReadSubtree();

            if (CheckEmptyNode(subReader, "KeywordList", string.Empty, true))
                return;

            while (!subReader.EOF)
            {
                subReader.MoveToContent();
                if (subReader.LocalName == "Keyword")
                {
                    var att = subReader.GetAttribute("vocabulary");
                    reader.ReadStartElement("Keyword");
                    var val = reader.ReadContentAsString();
                    reader.ReadEndElement();
                    Keyword.Add(new Keyword { Vocabulary = att, Value = val });
                }
                else
                {
                    subReader.ReadEndElement();
                }
            }

            reader.Skip();
        }

        public override void WriteXml(XmlWriter writer)
        {
            foreach (var keyword in Keyword)
            {
                writer.WriteStartElement("Keyword", Namespace);
                keyword.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public override XElement ToXElement(string @namespace)
        {
            var elements = Keyword.Select(keyword => keyword.ToXElement(@namespace)).ToList();
            return new XElement(XName.Get("KeywordList", @namespace), elements);
        }

        #endregion Overrides of XmlObject
    }
}