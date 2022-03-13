// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Wms
{
    public class OnlineResource : XmlObject
    {
        public OnlineResource()
        {
            Type = "simple";
        }

        // ReSharper disable once UnusedParameter.Local
        public OnlineResource(XElement node)
        {
            Href = node.Attribute(XName.Get("href", WmsNamespaces.Xlink))?.Value;
            Type = node.Attribute(XName.Get("type", WmsNamespaces.Xlink))?.Value;
        }

        public override XElement ToXElement(string ns)
        {
            return new XElement(XName.Get("OnlineResource", ns),
                new XAttribute(XName.Get("href", WmsNamespaces.Xlink), Href),
                new XAttribute(XName.Get("type"), Type));
        }

        [System.Xml.Serialization.XmlAttribute]
        public string Href { get; set; }

        public string Type { get; set; }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Href = reader.GetAttribute("href", WmsNamespaces.Xlink);
            Type = reader.GetAttribute("type");
            var isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
                reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("href", WmsNamespaces.Xlink, Href);
            writer.WriteAttributeString("type", Type);
        }

        #endregion Overrides of XmlObject
    }
}
