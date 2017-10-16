// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Wms
{
    public class LatLonBoundingBox : XmlObject
    {
        public LatLonBoundingBox()
        {
        }

        public LatLonBoundingBox(XElement node, string @namespace)
        {
            var att = node.Attribute(XName.Get("minx"));
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
        }

        public double MinX { get; set; }

        public double MaxX { get; set; }

        public double MinY { get; set; }

        public double MaxY { get; set; }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            reader.Read();
            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    string tmpString;
                    switch (reader.LocalName)
                    {
                        case "minx":
                            tmpString = reader.ReadElementContentAsString();
                            MinX = double.Parse(tmpString, NumberFormatInfo.InvariantInfo);
                            break;
                        case "miny":
                            tmpString = reader.ReadElementContentAsString();
                            MinY = double.Parse(tmpString, NumberFormatInfo.InvariantInfo);
                            break;
                        case "maxx":
                            tmpString = reader.ReadElementContentAsString();
                            MaxX = double.Parse(tmpString, NumberFormatInfo.InvariantInfo);
                            break;
                        case "maxy":
                            tmpString = reader.ReadElementContentAsString();
                            MinY = double.Parse(tmpString, NumberFormatInfo.InvariantInfo);
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
            writer.WriteElementString("minx", Namespace,
                                      MinX.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteElementString("miny", Namespace,
                                      MinY.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteElementString("maxx", Namespace,
                                      MaxX.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteElementString("maxy", Namespace,
                                      MaxY.ToString(NumberFormatInfo.InvariantInfo));
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("LatLonBoundingBox", @namespace),
                new XElement(XName.Get("minx", @namespace), MinX),
                new XElement(XName.Get("miny", @namespace), MinY),
                new XElement(XName.Get("maxx", @namespace), MaxX),
                new XElement(XName.Get("maxy", @namespace), MaxY)
                );
        }

        #endregion Overrides of XmlObject
    }
}