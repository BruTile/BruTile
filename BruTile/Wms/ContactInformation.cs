// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class ContactInformation : XmlObject
    {
        public ContactInformation()
        {
        }

        public ContactInformation(XElement node, string @namespace)
        {
            var element = node.Element(XName.Get("ContactPersonPrimary", @namespace));
            if (element != null) ContactPersonPrimary = new ContactPersonPrimary(element, @namespace);

            element = node.Element(XName.Get("ContactPosition", @namespace));
            if (element != null) ContactPosition = element.Value;

            element = node.Element(XName.Get("ContactAddress", @namespace));
            if (element != null) ContactAddress = new ContactAddress(element, @namespace);

            element = node.Element(XName.Get("ContactVoiceTelephone", @namespace));
            if (element != null) ContactVoiceTelephone = element.Value;
            element = node.Element(XName.Get("ContactFacsimileTelephone", @namespace));
            if (element != null) ContactFacsimileTelephone = element.Value;
            element = node.Element(XName.Get("ContactElectronicMailAddress", @namespace));
            if (element != null) ContactElectronicMailAddress = element.Value;
        }

        public bool IsEmpty
        {
            get
            {
                return _contactPersonPrimaryField.IsEmpty &
                       string.IsNullOrEmpty(ContactPosition) &
                       _contactAddressField.IsEmpty &
                       string.IsNullOrEmpty(ContactVoiceTelephone) &
                       string.IsNullOrEmpty(ContactFacsimileTelephone) &
                       string.IsNullOrEmpty(ContactElectronicMailAddress);
            }
        }

        private ContactPersonPrimary _contactPersonPrimaryField;

        private ContactAddress _contactAddressField;

        public string ContactPosition { get; set; }

        public string ContactVoiceTelephone { get; set; }

        public string ContactFacsimileTelephone { get; set; }

        public string ContactElectronicMailAddress { get; set; }

        public ContactPersonPrimary ContactPersonPrimary
        {
            get
            {
                if ((_contactPersonPrimaryField == null))
                {
                    _contactPersonPrimaryField = new ContactPersonPrimary();
                }
                return _contactPersonPrimaryField;
            }
            set
            {
                _contactPersonPrimaryField = value;
            }
        }

        public ContactAddress ContactAddress
        {
            get
            {
                if ((_contactAddressField == null))
                {
                    _contactAddressField = new ContactAddress();
                }
                return _contactAddressField;
            }
            set
            {
                _contactAddressField = value;
            }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            if (CheckEmptyNode(reader, "ContactInformation", Namespace))
                return;

            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    var isEmpty = reader.IsEmptyElement;
                    switch (reader.LocalName)
                    {
                        case "ContactPersonPrimary":
                            ContactPersonPrimary.ReadXml(reader.ReadSubtree());
                            break;
                        case "ContactPosition":
                            ContactPosition = reader.ReadElementContentAsString();
                            break;
                        case "ContactAddress":
                            ContactAddress.ReadXml(reader.ReadSubtree());
                            break;
                        case "ContactVoiceTelephone":
                            ContactVoiceTelephone = reader.ReadElementContentAsString();
                            break;
                        case "ContactFacsimileTelephone":
                            ContactFacsimileTelephone = reader.ReadElementContentAsString();
                            break;
                        case "ContactElectronicMailAddress":
                            ContactElectronicMailAddress = reader.ReadElementContentAsString();
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
            if (!ContactPersonPrimary.IsEmpty)
            {
                writer.WriteStartElement("ContactPersonPrimary", Namespace);
                ContactPersonPrimary.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(ContactPosition))
                writer.WriteElementString("ContactPosition", ContactPosition);

            if (!ContactAddress.IsEmpty)
            {
                writer.WriteStartElement("ContactAddress", Namespace);
                ContactAddress.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(ContactVoiceTelephone))
                writer.WriteElementString("ContactVoiceTelephone", ContactVoiceTelephone);
            if (!string.IsNullOrEmpty(ContactFacsimileTelephone))
                writer.WriteElementString("ContactFacsimileTelephone", ContactFacsimileTelephone);
            if (!string.IsNullOrEmpty(ContactElectronicMailAddress))
                writer.WriteElementString("ContactElectronicMailAddress", ContactElectronicMailAddress);
        }

        public override XElement ToXElement(string @namespace)
        {
            var elements = new List<XElement>();
            if (!ContactPersonPrimary.IsEmpty) elements.Add(ContactPersonPrimary.ToXElement(@namespace));
            if (!string.IsNullOrEmpty("ContactPosition")) elements.Add(new XElement(XName.Get("ContactPosition", @namespace), ContactPosition));
            if (!ContactAddress.IsEmpty) elements.Add(ContactAddress.ToXElement(@namespace));
            if (!string.IsNullOrEmpty("ContactPosition")) elements.Add(new XElement(XName.Get("ContactVoiceTelephone", @namespace), ContactVoiceTelephone));
            if (!string.IsNullOrEmpty("ContactPosition")) elements.Add(new XElement(XName.Get("ContactFacsimileTelephone", @namespace), ContactFacsimileTelephone));
            if (!string.IsNullOrEmpty("ContactPosition")) elements.Add(new XElement(XName.Get("ContactElectronicMailAddress", @namespace), ContactElectronicMailAddress));

            return new XElement(XName.Get("ContactInformation", @namespace), elements.ToArray());
        }

        #endregion Overrides of XmlObject
    }
}