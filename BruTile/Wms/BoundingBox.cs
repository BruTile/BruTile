// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Wms;

public class BoundingBox : XmlObject
{
    // ReSharper disable once UnusedParameter.Local
    public BoundingBox(XElement node)
    {
        var att =
            node.Attribute(XName.Get("CRS")) ??
            node.Attribute(XName.Get("crs")) ??
            node.Attribute(XName.Get("SRS")) ??
            node.Attribute(XName.Get("srs"));

        if (att != null)
            CRS = att.Value;
        else
            throw WmsParsingException.AttributeNotFound("CRS/SRS");

        att = node.Attribute(XName.Get("minx"));
        if (att == null) throw WmsParsingException.AttributeNotFound("minx");
        MinX = double.Parse(att.Value, NumberFormatInfo.InvariantInfo);
        att = node.Attribute(XName.Get("maxx"));
        if (att == null) throw WmsParsingException.AttributeNotFound("maxx");
        MaxX = double.Parse(att.Value, NumberFormatInfo.InvariantInfo);
        att = node.Attribute(XName.Get("miny"));
        if (att == null) throw WmsParsingException.AttributeNotFound("miny");
        MinY = double.Parse(att.Value, NumberFormatInfo.InvariantInfo);
        att = node.Attribute(XName.Get("maxy"));
        if (att == null) throw WmsParsingException.AttributeNotFound("maxy");
        MaxY = double.Parse(att.Value, NumberFormatInfo.InvariantInfo);

        att = node.Attribute(XName.Get("resx"));
        if (att != null) ResX = double.Parse(att.Value, NumberFormatInfo.InvariantInfo);
        att = node.Attribute(XName.Get("resy"));
        if (att != null) ResY = double.Parse(att.Value, NumberFormatInfo.InvariantInfo);
    }

    public override XElement ToXElement(string ns)
    {
        var attributes = new List<object>
                             {
                                 new XAttribute("CRS", CRS),
                                 new XAttribute("minx", MinX.ToString(NumberFormatInfo.InvariantInfo)),
                                 new XAttribute("maxx", MaxX.ToString(NumberFormatInfo.InvariantInfo)),
                                 new XAttribute("miny", MinY.ToString(NumberFormatInfo.InvariantInfo)),
                                 new XAttribute("maxy", MaxY.ToString(NumberFormatInfo.InvariantInfo))
                             };
        if (ResX.HasValue)
            attributes.Add(new XAttribute("resx", ResX.Value.ToString(NumberFormatInfo.InvariantInfo)));
        if (ResY.HasValue)
            attributes.Add(new XAttribute("resy", ResY.Value.ToString(NumberFormatInfo.InvariantInfo)));
        return new XElement(XName.Get("BoundingBox", ns), attributes.ToArray());
    }

    public string CRS { get; set; }

    public double MinX { get; set; }

    public double MinY { get; set; }

    public double MaxX { get; set; }

    public double MaxY { get; set; }

    public double? ResX { get; set; }

    public double? ResY { get; set; }

    #region Overrides of XmlObject

    public override void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();
        CRS = reader.GetAttribute("CRS");
        if (double.TryParse(reader.GetAttribute("minx"), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var minX))
            MinX = minX;
        if (double.TryParse(reader.GetAttribute("maxx"), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var maxX))
            MaxX = maxX;
        if (double.TryParse(reader.GetAttribute("miny"), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var minY))
            MinY = minY;
        if (double.TryParse(reader.GetAttribute("maxy"), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var maxY))
            MaxY = maxY;

        var res = reader.GetAttribute("resx");
        if (res != null && double.TryParse(res, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var resX))
            ResX = resX;
        res = reader.GetAttribute("resy");
        if (res != null && double.TryParse(res, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var resY))
            ResY = resY;
        var isEmptyElement = reader.IsEmptyElement;
        reader.ReadStartElement();
        if (!isEmptyElement)
        {
            reader.ReadEndElement();
        }
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("CRS", CRS);
        writer.WriteAttributeString("minx", MinX.ToString(NumberFormatInfo.InvariantInfo));
        writer.WriteAttributeString("maxx", MaxX.ToString(NumberFormatInfo.InvariantInfo));
        writer.WriteAttributeString("miny", MinY.ToString(NumberFormatInfo.InvariantInfo));
        writer.WriteAttributeString("maxy", MaxY.ToString(NumberFormatInfo.InvariantInfo));
        if (ResX.HasValue)
            writer.WriteAttributeString("resx", ResX.Value.ToString(NumberFormatInfo.InvariantInfo));
        if (ResY.HasValue)
            writer.WriteAttributeString("resy", ResY.Value.ToString(NumberFormatInfo.InvariantInfo));
    }

    #endregion Overrides of XmlObject
}
