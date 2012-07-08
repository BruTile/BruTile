// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class OperationType : XmlObject
    {
        private List<string> _formatField;

        // ReSharper disable InconsistentNaming
        private List<DCPType> _DCpTypeField;

        public OperationType()
        {
        }

        public OperationType(XElement node, string @namespace)
        {
            foreach (var formatNode in node.Elements(XName.Get("Format", @namespace)))
                Format.Add(formatNode.Value);

            foreach (var dcptype in node.Elements(XName.Get("DCPType", @namespace)))
                DCPType.Add(new DCPType(dcptype, @namespace));

            if (Format.Count < 1)
                throw WmsParsingException.ElementNotFound("Format");
            if (DCPType.Count < 1)
                throw WmsParsingException.ElementNotFound("DCPType");
        }

        public List<string> Format
        {
            get
            {
                if ((_formatField == null))
                {
                    _formatField = new List<string>();
                }
                return _formatField;
            }
            set
            {
                _formatField = value;
            }
        }

        public List<DCPType> DCPType
        {
            get
            {
                if ((_DCpTypeField == null))
                {
                    _DCpTypeField = new List<DCPType>();
                }
                return _DCpTypeField;
            }
            set
            {
                _DCpTypeField = value;
            }
        }

        // ReSharper restore InconsistentNaming

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "Format":
                            Format.Add(reader.ReadElementContentAsString());
                            break;
                        case "DCPType":
                            var tmp = new DCPType();
                            tmp.ReadXml(reader);
                            DCPType.Add(tmp);
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
            WriteXmlList("Format", Namespace, writer, _formatField);
            WriteXmlList("DCPType", Namespace, writer, _DCpTypeField);
        }

        public override XElement ToXElement(string @namespace)
        {
            throw new System.NotImplementedException();
        }

        #endregion Overrides of XmlObject
    }
}