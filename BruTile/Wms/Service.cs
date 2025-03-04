// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Wms;

public class Service : XmlObject
{
    private KeywordList _keywordListField;
    private OnlineResource _onlineResourceField;
    private ContactInformation _contactInformationField;

    /// <summary>
    /// ParseName - Many WMS' do not set the Name correctly, so a client may set to false to allow these slightly out of spec servers
    /// </summary>
    public static bool ParseName { get; set; }
    static Service()
    {
        ParseName = true;
    }

    public Service()
    { }

    public Service(XElement node, string @namespace)
    {
        XElement element;
        if (ParseName)
        {
            element = node.Element(XName.Get("Name", @namespace));
            if (element == null)
                throw WmsParsingException.ElementNotFound("Name");
            var value = element.Value;
            if (!value.StartsWith("ogc:wms", StringComparison.CurrentCultureIgnoreCase))
                Debug.WriteLine("Warning: Invalid service name: '{0}'. Must be 'OGC:WMS'", value);
            Name = ServiceName.WMS;
        }

        element = node.Element(XName.Get("Title", @namespace));
        if (element == null)
            throw WmsParsingException.ElementNotFound("Title");
        Title = element.Value;

        element = node.Element(XName.Get("Abstract", @namespace));
        Abstract = element != null ? element.Value : string.Empty;

        element = node.Element(XName.Get("KeywordList", @namespace));
        if (element != null)
            KeywordList = new KeywordList(element, @namespace);

        element = node.Element(XName.Get("OnlineResource", @namespace));
        if (element != null)
            OnlineResource = new OnlineResource(element);

        element = node.Element(XName.Get("ContactInformation", @namespace));
        if (element != null)
            ContactInformation = new ContactInformation(element, @namespace);

        element = node.Element(XName.Get("Fees", @namespace));
        Fees = element != null ? element.Value : string.Empty;

        element = node.Element(XName.Get("AccessConstraints", @namespace));
        AccessConstraints = element != null ? element.Value : string.Empty;

        element = node.Element(XName.Get("LayerLimit", @namespace));
        if (element == null)
            LayerLimit = null;
        else
            LayerLimit = int.Parse(element.Value, NumberFormatInfo.InvariantInfo);

        element = node.Element(XName.Get("MaxWidth", @namespace));
        if (element == null)
            MaxWidth = null;
        else
            MaxWidth = int.Parse(element.Value, NumberFormatInfo.InvariantInfo);

        element = node.Element(XName.Get("MaxHeight", @namespace));
        if (element == null)
            MaxHeight = null;
        else
            MaxHeight = int.Parse(element.Value, NumberFormatInfo.InvariantInfo);
    }

    public string Title { get; set; }

    public string Abstract { get; set; }

    public string Fees { get; set; }

    public string AccessConstraints { get; set; }

    private int? _layerLimit;

    public int? LayerLimit
    {
        get => _layerLimit;
        set
        {
            if (value.HasValue && value < 1)
                throw WmsPropertyException.PositiveInteger("LayerLimit", value.Value);
            _layerLimit = value;
        }
    }

    private int? _maxWidth;

    public int? MaxWidth
    {
        get => _maxWidth;
        set
        {
            if (value.HasValue && value.Value < 1)
                throw WmsPropertyException.PositiveInteger("MaxWidth", value.Value);
            _maxWidth = value;
        }
    }

    private int? _maxHeight;

    public int? MaxHeight
    {
        get => _maxHeight;
        set
        {
            if (value.HasValue && value.Value < 1)
                throw WmsPropertyException.PositiveInteger("MaxHeight", value.Value);
            _maxHeight = value;
        }
    }

    public ServiceName Name { get; set; }

    public KeywordList KeywordList
    {
        get => _keywordListField ??= new KeywordList();
        set => _keywordListField = value;
    }

    public OnlineResource OnlineResource
    {
        get => _onlineResourceField ??= new OnlineResource();
        set => _onlineResourceField = value;
    }

    public ContactInformation ContactInformation
    {
        get => _contactInformationField ??= new ContactInformation();
        set => _contactInformationField = value;
    }

    #region Overrides of XmlObject

    public override void ReadXml(XmlReader reader)
    {
        if (CheckEmptyNode(reader, string.Empty))
            throw WmsParsingException.ElementNotFound("Service");

        while (!reader.EOF)
        {
            if (reader.IsStartElement())
            {
                switch (reader.LocalName)
                {
                    case "Name":
                        var name = reader.ReadElementContentAsString();
                        const string prefix = "ogc:";
                        if (name.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)) name = name[prefix.Length..];
                        Name = Enum.Parse<ServiceName>(name, true);
                        break;
                    case "Title":
                        Title = reader.ReadElementContentAsString();
                        break;
                    case "Abstract":
                        Abstract = reader.ReadElementContentAsString();
                        break;
                    case "KeywordList":
                        KeywordList = new KeywordList();
                        KeywordList.ReadXml(reader);
                        break;
                    case "OnlineResource":
                        OnlineResource = new OnlineResource();
                        OnlineResource.ReadXml(reader);
                        break;
                    case "ContactInformation":
                        ContactInformation = new ContactInformation();
                        ContactInformation.ReadXml(reader);
                        break;
                    case "Fees":
                        Fees = reader.ReadElementContentAsString();
                        break;
                    case "AccessConstraints":
                        AccessConstraints = reader.ReadElementContentAsString();
                        break;
                    case "LayerLimit":
                        LayerLimit = reader.ReadElementContentAsInt();
                        break;
                    case "MaxWidth":
                        MaxWidth = reader.ReadElementContentAsInt();
                        break;
                    case "MaxHeight":
                        MaxHeight = reader.ReadElementContentAsInt();
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
        writer.WriteElementString("Name", Namespace, Name.ToString());
        writer.WriteElementString("Title", Namespace, Title);
        if (!string.IsNullOrEmpty(Abstract))
            writer.WriteElementString("Abstract", Namespace, Abstract);
        if (_keywordListField != null && _keywordListField.Keyword.Count > 0)
        {
            writer.WriteStartElement("KeywordList", Namespace);
            KeywordList.WriteXml(writer);
            writer.WriteEndElement();
        }

        writer.WriteStartElement("OnlineResource", Namespace);
        OnlineResource.WriteXml(writer);
        writer.WriteEndElement();

        if (_contactInformationField != null)
        {
            writer.WriteStartElement("ContactInformation", Namespace);
            _contactInformationField.WriteXml(writer);
            writer.WriteEndElement();
        }

        if (!string.IsNullOrEmpty(Fees))
            writer.WriteElementString("Fees", Namespace, Fees);

        if (!string.IsNullOrEmpty(AccessConstraints))
            writer.WriteElementString("AccessConstraints", Namespace, AccessConstraints);

        if (LayerLimit.HasValue)
            writer.WriteElementString("LayerLimit", Namespace, LayerLimit.Value.ToString());
        if (MaxWidth.HasValue)
            writer.WriteElementString("MaxWidth", Namespace, MaxWidth.Value.ToString());
        if (MaxHeight.HasValue)
            writer.WriteElementString("MaxHeight", Namespace, MaxHeight.Value.ToString());
    }

    public override XElement ToXElement(string @namespace)
    {
        throw new NotImplementedException();
    }

    #endregion Overrides of XmlObject
}
