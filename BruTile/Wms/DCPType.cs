// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    // ReSharper disable InconsistentNaming
    public class DCPType : XmlObject
    {
        public DCPType()
        {
            Http = new Http();
        }

        public DCPType(XElement node, string @namespace)
        {
            var element = node.Element(XName.Get("HTTP", @namespace));
            if (element != null)
                Http = new Http(element, @namespace);
        }

        public Http Http { get; set; }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            if (CheckEmptyNode(reader, "DCPType", Namespace))
                return;
            reader.MoveToContent();
            Http.ReadXml(reader.ReadSubtree());
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("HTTP", Namespace);
            Http.WriteXml(writer);
            writer.WriteEndElement();
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("DCPType", @namespace),
                Http.ToXElement(@namespace));
        }

        #endregion Overrides of XmlObject
    }

    // ReSharper restore InconsistentNaming
}