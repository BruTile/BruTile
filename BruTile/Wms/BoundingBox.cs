// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class BoundingBox : XmlObject
    {
        //<BoundingBox CRS="CRS:84" minx="-71.63" miny="41.75" maxx="-70.78" maxy="42.90" resx="0.01" resy="0.01"/>

        public BoundingBox()
        { }

        public BoundingBox(XElement node, string nameSpace)
        {
            var att = node.Attribute(XName.Get("CRS"));
            if (att == null) att = node.Attribute(XName.Get("crs"));
            if (att == null) att = node.Attribute(XName.Get("SRS"));
            if (att == null) att = node.Attribute(XName.Get("srs"));
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

        public override XElement ToXElement(string nameSpace)
        {
            var attributes = new List<XAttribute>
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
            return new XElement(XName.Get("BoundingBox", nameSpace), attributes.ToArray());
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
            double val;
            if (double.TryParse(reader.GetAttribute("minx"), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out val))
                MinX = val;
            if (double.TryParse(reader.GetAttribute("maxx"), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out val))
                MaxX = val;
            if (double.TryParse(reader.GetAttribute("miny"), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out val))
                MinY = val;
            if (double.TryParse(reader.GetAttribute("maxy"), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out val))
                MaxY = val;

            var res = reader.GetAttribute("resx");
            if (res != null && double.TryParse(res, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out val))
                ResX = val;
            res = reader.GetAttribute("resy");
            if (res != null && double.TryParse(res, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out val))
                ResY = val;
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
}