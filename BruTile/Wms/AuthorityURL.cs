// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Wms;

public class AuthorityURL : XmlObject
{
    private OnlineResource _onlineResourceField;

    public AuthorityURL() { }

    public AuthorityURL(XElement node, string @namespace)
    {
        var att = node.Attribute("name") ?? throw WmsParsingException.AttributeNotFound("name");
        Name = att.Value;

        var element = node.Element(XName.Get("OnlineResource", @namespace));
        if (element != null)
            OnlineResource = new OnlineResource(element);
    }

    public string Name { get; set; }

    public OnlineResource OnlineResource
    {
        get => _onlineResourceField ??= new OnlineResource();
        set => _onlineResourceField = value;
    }

    #region Overrides of XmlObject

    public override void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();
        Name = reader.GetAttribute("name");

        var isEmptyElement = reader.IsEmptyElement;
        reader.ReadStartElement();
        if (!isEmptyElement)
        {
            OnlineResource.ReadXml(reader);
            reader.ReadEndElement();
        }
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("name", Name);
        writer.WriteStartElement("OnlineResource", Namespace);
        OnlineResource.WriteXml(writer);
        writer.WriteEndElement();
    }

    public override XElement ToXElement(string @namespace)
    {
        return new XElement(XName.Get("AuthorityURL", @namespace),
                            new XAttribute(XName.Get("name"), Name),
                            OnlineResource.ToXElement(@namespace));
    }

    #endregion Overrides of XmlObject
}
