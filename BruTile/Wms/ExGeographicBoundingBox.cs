// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class ExGeographicBoundingBox : XmlObject
    {
        public ExGeographicBoundingBox()
        {
        }

        public ExGeographicBoundingBox(XElement node, string @namespace)
        {
            var element = node.Element(XName.Get("westBoundLongitude", @namespace));
            if (element == null)
                throw WmsParsingException.ElementNotFound("westBoundLongitude");
            WestBoundLongitude = double.Parse(element.Value, NumberFormatInfo.InvariantInfo);

            element = node.Element(XName.Get("eastBoundLongitude", @namespace));
            if (element == null)
                throw WmsParsingException.ElementNotFound("eastBoundLongitude");
            EastBoundLongitude = double.Parse(element.Value, NumberFormatInfo.InvariantInfo);

            element = node.Element(XName.Get("northBoundLatitude", @namespace));
            if (element == null)
                throw WmsParsingException.ElementNotFound("northBoundLatitude");
            NorthBoundLatitude = double.Parse(element.Value, NumberFormatInfo.InvariantInfo);

            element = node.Element(XName.Get("southBoundLatitude", @namespace));
            if (element == null)
                throw WmsParsingException.ElementNotFound("southBoundLatitude");
            SouthBoundLatitude = double.Parse(element.Value, NumberFormatInfo.InvariantInfo);
        }

        public double WestBoundLongitude { get; set; }

        public double EastBoundLongitude { get; set; }

        public double SouthBoundLatitude { get; set; }

        public double NorthBoundLatitude { get; set; }

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
                        case "westBoundLongitude":
                            tmpString = reader.ReadElementContentAsString();
                            WestBoundLongitude = double.Parse(tmpString, NumberFormatInfo.InvariantInfo);
                            break;
                        case "eastBoundLongitude":
                            tmpString = reader.ReadElementContentAsString();
                            WestBoundLongitude = double.Parse(tmpString, NumberFormatInfo.InvariantInfo);
                            break;
                        case "northBoundLatitude":
                            tmpString = reader.ReadElementContentAsString();
                            NorthBoundLatitude = double.Parse(tmpString, NumberFormatInfo.InvariantInfo);
                            break;
                        case "southBoundLatitude":
                            tmpString = reader.ReadElementContentAsString();
                            SouthBoundLatitude = double.Parse(tmpString, NumberFormatInfo.InvariantInfo);
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
            writer.WriteElementString("westBoundLongitude", Namespace,
                                      WestBoundLongitude.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteElementString("eastBoundLongitude", Namespace,
                                      EastBoundLongitude.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteElementString("northBoundLatitude", Namespace,
                                      NorthBoundLatitude.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteElementString("southBoundLatitude", Namespace,
                                      SouthBoundLatitude.ToString(NumberFormatInfo.InvariantInfo));
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("EX_GeographicBoundingBox", @namespace),
                new XElement(XName.Get("westBoundLongitude", @namespace), WestBoundLongitude),
                new XElement(XName.Get("eastBoundLongitude", @namespace), EastBoundLongitude),
                new XElement(XName.Get("northBoundLatitude", @namespace), NorthBoundLatitude),
                new XElement(XName.Get("southBoundLatitude", @namespace), SouthBoundLatitude)
                );
        }

        #endregion Overrides of XmlObject
    }
}