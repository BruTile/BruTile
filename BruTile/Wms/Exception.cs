// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    using System.Collections.Generic;

    public class Exception : XmlObject
    {
        public Exception()
        {
            Format = new List<string>();
        }

        public Exception(XElement node, string @namespace)
        {
            foreach (var format in node.Elements(XName.Get("Format", @namespace)))
                Format.Add(format.Value);
        }

        public List<string> Format { get; private set; }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            if (CheckEmptyNode(reader, "Exception", Namespace))
                return;

            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    if (reader.LocalName == "Format")
                        Format.Add(reader.ReadElementContentAsString());
                    else
                        reader.Skip();
                }
                else
                {
                    reader.Read();
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            WriteXmlList("Format", Namespace, writer, Format);
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("Exception", Namespace),
                Format.Select(format => new XElement(XName.Get("Format", Namespace), format)).ToArray());
        }

        #endregion Overrides of XmlObject
    }
}