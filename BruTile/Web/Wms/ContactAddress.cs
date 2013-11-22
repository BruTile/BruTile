// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class ContactAddress : XmlObject
    {
        public ContactAddress()
        {
        }

        public ContactAddress(XElement node, string @namespace)
        {
            var element = node.Element(XName.Get("AddressType", @namespace));
            if (element != null) AddressType = element.Value;

            element = node.Element(XName.Get("Address", @namespace));
            if (element != null) Address = element.Value;
            element = node.Element(XName.Get("City", @namespace));
            if (element != null) City = element.Value;
            element = node.Element(XName.Get("StateOrProvince", @namespace));
            if (element != null) StateOrProvince = element.Value;
            element = node.Element(XName.Get("PostCode", @namespace));
            if (element != null) PostCode = element.Value;
            element = node.Element(XName.Get("Country", @namespace));
            if (element != null) Country = element.Value;
        }

        public string AddressType { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string StateOrProvince { get; set; }

        public string PostCode { get; set; }

        public string Country { get; set; }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(AddressType) & string.IsNullOrEmpty(Address) &
                       string.IsNullOrEmpty(City) & string.IsNullOrEmpty(StateOrProvince) &
                       string.IsNullOrEmpty(PostCode) & string.IsNullOrEmpty(Country);
            }
            //set { throw new System.NotImplementedException(); }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            if (CheckEmptyNode(reader, "ContactAddress", Namespace))
                return;

            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    var isEmpty = reader.IsEmptyElement;
                    switch (reader.LocalName)
                    {
                        case "AddressType":
                            AddressType = isEmpty ? string.Empty : reader.ReadElementContentAsString();
                            break;
                        case "Address":
                            Address = isEmpty ? string.Empty : reader.ReadElementContentAsString();
                            break;
                        case "City":
                            City = isEmpty ? string.Empty : reader.ReadElementContentAsString();
                            break;
                        case "StateOrProvince":
                            StateOrProvince = isEmpty ? string.Empty : reader.ReadElementContentAsString();
                            break;
                        case "PostCode":
                            PostCode = isEmpty ? string.Empty : reader.ReadElementContentAsString();
                            break;
                        case "Country":
                            Country = isEmpty ? string.Empty : reader.ReadElementContentAsString();
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
            if (!string.IsNullOrEmpty(AddressType))
                writer.WriteElementString("AddressType", Namespace, AddressType);
            if (!string.IsNullOrEmpty(Address))
                writer.WriteElementString("Address", Namespace, Address);
            if (!string.IsNullOrEmpty(City))
                writer.WriteElementString("City", Namespace, City);
            if (!string.IsNullOrEmpty(StateOrProvince))
                writer.WriteElementString("StateOrProvince", Namespace, StateOrProvince);
            if (!string.IsNullOrEmpty(PostCode))
                writer.WriteElementString("PostCode", Namespace, PostCode);
            if (!string.IsNullOrEmpty(Country))
                writer.WriteElementString("Country", Namespace, Country);
        }

        public override XElement ToXElement(string @namespace)
        {
            var lst = new List<XElement>();
            if (!string.IsNullOrEmpty(AddressType))
                lst.Add(new XElement(XName.Get("AddressType", @namespace), Address));
            if (!string.IsNullOrEmpty(Address))
                lst.Add(new XElement(XName.Get("Address", @namespace), Address));
            if (!string.IsNullOrEmpty(City))
                lst.Add(new XElement(XName.Get("City", @namespace), City));
            if (!string.IsNullOrEmpty(StateOrProvince))
                lst.Add(new XElement(XName.Get("StateOrProvince", @namespace), StateOrProvince));
            if (!string.IsNullOrEmpty(PostCode))
                lst.Add(new XElement(XName.Get("PostCode", @namespace), PostCode));
            if (!string.IsNullOrEmpty(Country))
                lst.Add(new XElement(XName.Get("Country", @namespace), Country));

            return new XElement(XName.Get("ContactAddress", @namespace), lst.ToArray());
        }

        #endregion Overrides of XmlObject
    }
}