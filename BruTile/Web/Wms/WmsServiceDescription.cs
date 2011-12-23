#region License

// Copyright 2010 - Paul den Dulk (Geodan)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

using System.Collections.Generic;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public struct WmsServiceDescription
    {
        /// <summary>
        /// Optional narrative description providing additional information
        /// </summary>
        public string Abstract;

        /// <summary>
        /// <para>The optional element "AccessConstraints" may be omitted if it do not apply to the server. If
        /// the element is present, the reserved word "none" (case-insensitive) shall be used if there are no
        /// access constraints, as follows: "none".</para>
        /// <para>When constraints are imposed, no precise syntax has been defined for the text content of these elements, but
        /// client applications may display the content for user information and action.</para>
        /// </summary>
        public string AccessConstraints;

        /// <summary>
        /// Optional WMS contact information
        /// </summary>
        public WmsContactInformation ContactInformation;

        /// <summary>
        /// The optional element "Fees" may be omitted if it do not apply to the server. If
        /// the element is present, the reserved word "none" (case-insensitive) shall be used if there are no
        /// fees, as follows: "none".
        /// </summary>
        public string Fees;

        /// <summary>
        /// Optional list of keywords or keyword phrases describing the server as a whole to help catalog searching
        /// </summary>
        public IList<string> Keywords;

        /// <summary>
        /// Maximum number of layers allowed (0=no restrictions)
        /// </summary>
        public int LayerLimit;

        /// <summary>
        /// Maximum height allowed in pixels (0=no restrictions)
        /// </summary>
        public int MaxHeight;

        /// <summary>
        /// Maximum width allowed in pixels (0=no restrictions)
        /// </summary>
        public int MaxWidth;

        /// <summary>
        /// Mandatory Top-level web address of service or service provider.
        /// </summary>
        public string OnlineResource;

        /// <summary>
        /// Mandatory Human-readable title for pick lists
        /// </summary>
        public string Title;

        /// <summary>
        /// Initializes a WmsServiceDescription object
        /// </summary>
        /// <param name="title">Mandatory Human-readable title for pick lists</param>
        /// <param name="onlineResource">Top-level web address of service or service provider.</param>
        public WmsServiceDescription(string title, string onlineResource)
        {
            Title = title;
            OnlineResource = onlineResource;
            Keywords = null;
            Abstract = "";
            ContactInformation = new WmsContactInformation();
            Fees = "";
            AccessConstraints = "";
            LayerLimit = 0;
            MaxWidth = 0;
            MaxHeight = 0;
        }

        public WmsServiceDescription(XElement xmlServiceDescription, WmsNamespaces namespaces)
        {
            var node = xmlServiceDescription.Element(XName.Get("Title", namespaces.Wms));

            Title = (node != null ? node.Value : string.Empty);
            node = xmlServiceDescription.Element(XName.Get("OnlineResource", namespaces.Wms));
            OnlineResource = (node != null ? node.Attribute(XName.Get("href", WmsNamespaces.Xlink)).Value : string.Empty);
            node = xmlServiceDescription.Element(XName.Get("Abstract", namespaces.Wms));
            Abstract = (node != null ? node.Value : string.Empty);
            node = xmlServiceDescription.Element(XName.Get("Fees", namespaces.Wms));
            Fees = (node != null ? node.Value : string.Empty);
            node = xmlServiceDescription.Element(XName.Get("AccessConstraints", namespaces.Wms));
            AccessConstraints = (node != null ? node.Value : string.Empty);

            Keywords = new List<string>();
            var xnlKeywords = xmlServiceDescription.Element(XName.Get("KeywordList", namespaces.Wms));
            if (xnlKeywords != null)
            {
                foreach (var xnlKeyword in xnlKeywords.Elements(XName.Get("Keyword", namespaces.Wms)))
                {
                    Keywords.Add(xnlKeyword.Value);
                }
            }

            //Contact information
            var contactInformation = xmlServiceDescription.Element(XName.Get("ContactInformation", namespaces.Wms));
            ContactInformation = contactInformation != null
                                     ? new WmsContactInformation(contactInformation, namespaces)
                                     : new WmsContactInformation();

            LayerLimit = 0;
            MaxWidth = 0;
            MaxHeight = 0;
        }
    }

    public class WmsContactInformation
    {
        /// <summary>
        /// Address
        /// </summary>
        public ContactAddress Address;

        /// <summary>
        /// Email address
        /// </summary>
        public string ElectronicMailAddress = string.Empty;

        /// <summary>
        /// Fax number
        /// </summary>
        public string FacsimileTelephone = string.Empty;

        /// <summary>
        /// Primary contact person
        /// </summary>
        public ContactPerson PersonPrimary;

        /// <summary>
        /// Position of contact person
        /// </summary>
        public string Position = string.Empty;

        /// <summary>
        /// Telephone
        /// </summary>
        public string VoiceTelephone = string.Empty;

        public WmsContactInformation(XElement xmlContactInformation, WmsNamespaces namespaces)
        {
            var node = xmlContactInformation.Element(XName.Get("ContactAddress", namespaces.Wms));
            if (node != null) Address = new ContactAddress(node, namespaces);

            node = xmlContactInformation.Element(XName.Get("ContactElectronicMailAddress", namespaces.Wms));
            ElectronicMailAddress = (node != null ? node.Value : string.Empty);

            node = xmlContactInformation.Element(XName.Get("ContactFacsimileTelephone", namespaces.Wms));
            FacsimileTelephone = (node != null ? node.Value : string.Empty);

            node = xmlContactInformation.Element(XName.Get("ContactPersonPrimary", namespaces.Wms));
            if (node != null)
                PersonPrimary = new ContactPerson(node, namespaces.Wms);

            node = xmlContactInformation.Element(XName.Get("ContactPosition", namespaces.Wms));
            Position = (node != null ? node.Value : string.Empty);
            node = xmlContactInformation.Element(XName.Get("ContactVoiceTelephone", namespaces.Wms));
            VoiceTelephone = (node != null ? node.Value : string.Empty);
        }

        public WmsContactInformation()
        {
        }

        #region Nested type: ContactAddress

        /// <summary>
        /// Information about a contact address for the service.
        /// </summary>
        public struct ContactAddress
        {
            /// <summary>
            /// Contact address
            /// </summary>
            public string Address;

            /// <summary>
            /// Type of address (usually "postal").
            /// </summary>
            public string AddressType;

            /// <summary>
            /// Contact City
            /// </summary>
            public string City;

            /// <summary>
            /// Country of contact address
            /// </summary>
            public string Country;

            /// <summary>
            /// Zipcode of contact
            /// </summary>
            public string PostCode;

            /// <summary>
            /// State or province of contact
            /// </summary>
            public string StateOrProvince;

            public ContactAddress(XElement xmlContactAddress, WmsNamespaces namespaces)
            {
                var node = xmlContactAddress.Element(XName.Get("ContactAddress", namespaces.Wms));
                Address = (node != null ? node.Value : string.Empty);

                node = xmlContactAddress.Element(XName.Get("ContactAddressType", namespaces.Wms));
                AddressType = (node != null ? node.Value : string.Empty);

                node = xmlContactAddress.Element(XName.Get("ContactCity", namespaces.Wms));
                City = (node != null ? node.Value : string.Empty);

                node = xmlContactAddress.Element(XName.Get("ContactCountry", namespaces.Wms));
                Country = (node != null ? node.Value : string.Empty);

                node = xmlContactAddress.Element(XName.Get("ContactPostCode", namespaces.Wms));
                PostCode = (node != null ? node.Value : string.Empty);

                node = xmlContactAddress.Element(XName.Get("ContactStateOrProvince", namespaces.Wms));
                StateOrProvince = (node != null ? node.Value : string.Empty);
            }
        }

        #endregion

        #region Nested type: ContactPerson

        /// <summary>
        /// Information about a contact person for the service.
        /// </summary>
        public struct ContactPerson
        {
            /// <summary>
            /// Organisation of primary person
            /// </summary>
            public string Organisation;

            /// <summary>
            /// Primary contact person
            /// </summary>
            public string Person;

            public ContactPerson(XElement xmlPerson, string nameSpace)
            {
                var node = xmlPerson.Element(XName.Get("ContactPerson", nameSpace));
                Person = (node != null ? node.Value : string.Empty);

                node = xmlPerson.Element(XName.Get("ContactOrganisation", nameSpace));
                Organisation = (node != null ? node.Value : string.Empty);
            }
        }

        #endregion
    }

}


