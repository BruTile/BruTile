// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class ContactPersonPrimary : XmlObject
    {
        public ContactPersonPrimary()
        {
        }

        public ContactPersonPrimary(XElement node, string @namespace)
        {
            var element = node.Element(XName.Get("ContactPerson", @namespace));
            if (element != null) ContactPerson = element.Value;

            element = node.Element(XName.Get("ContactOrganization", @namespace));
            if (element != null) ContactOrganization = element.Value;
        }

        public string ContactPerson { get; set; }

        public string ContactOrganization { get; set; }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(ContactPerson) & string.IsNullOrEmpty(ContactOrganization); }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            if (CheckEmptyNode(reader, "ContactPersonPrimary", Namespace))
                return;

            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    var isEmpty = reader.IsEmptyElement;
                    switch (reader.LocalName)
                    {
                        case "ContactPerson":
                            ContactPerson = isEmpty ? string.Empty : reader.ReadElementContentAsString();
                            break;
                        case "ContactOrganization":
                            ContactOrganization = isEmpty ? string.Empty : reader.ReadElementContentAsString();
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    reader.ReadEndElement();
                    return;
                }
                else
                {
                    reader.Read();
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("ContactPerson", Namespace, ContactPerson);
            writer.WriteElementString("ContactOrganization", Namespace, ContactOrganization);
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("ContactPersonPrimary", @namespace),
                new XElement(XName.Get("ContactPerson", @namespace), ContactPerson),
                new XElement(XName.Get("ContactOrganization", @namespace), ContactOrganization));
        }

        #endregion Overrides of XmlObject
    }
}