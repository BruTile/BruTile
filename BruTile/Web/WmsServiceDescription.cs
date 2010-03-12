﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BruTile.Web
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
        public string[] Keywords;

        /// <summary>
        /// Maximum number of layers allowed (0=no restrictions)
        /// </summary>
        public uint LayerLimit;

        /// <summary>
        /// Maximum height allowed in pixels (0=no restrictions)
        /// </summary>
        public uint MaxHeight;

        /// <summary>
        /// Maximum width allowed in pixels (0=no restrictions)
        /// </summary>
        public uint MaxWidth;

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

        public struct WmsContactInformation
        {
            /// <summary>
            /// Address
            /// </summary>
            public ContactAddress Address;

            /// <summary>
            /// Email address
            /// </summary>
            public string ElectronicMailAddress;

            /// <summary>
            /// Fax number
            /// </summary>
            public string FacsimileTelephone;

            /// <summary>
            /// Primary contact person
            /// </summary>
            public ContactPerson PersonPrimary;

            /// <summary>
            /// Position of contact person
            /// </summary>
            public string Position;

            /// <summary>
            /// Telephone
            /// </summary>
            public string VoiceTelephone;

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
            }

            #endregion
        }

    }

}
