// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Wmts
{
    /// <summary>
    /// An identifier for the crs
    /// </summary>
    public struct CrsIdentifier : IEquatable<CrsIdentifier>
    {
        private readonly string _authority;
        private readonly string _version;
        private readonly string _identifier;

        public static bool TryParse(string urn_ogc_def_crs, out CrsIdentifier crs)
        {
            var parts = urn_ogc_def_crs.Split(':');
            switch (parts.Length)
            {
                case 2:
                    crs = new CrsIdentifier(parts[0], "", parts[1]);
                    break;
                case 6:
                    crs = new CrsIdentifier(parts[4], "", parts[5]);
                    break;
                case 7:
                    crs = new CrsIdentifier(parts[4], parts[5], parts[6]);
                    break;
                default:
                    crs = new CrsIdentifier();
                    break;
            }

            return !string.IsNullOrEmpty(crs.Authority);
        }

        /// <summary>
        /// Initializes this coordinate system identifier
        /// </summary>
        /// <param name="authority">The authority</param>
        /// <param name="version">The version</param>
        /// <param name="identifier">The identifier</param>
        internal CrsIdentifier(string authority, string version, string identifier)
        {
            _authority = authority;
            _version = version;
            _identifier = identifier;
        }

        /// <summary>
        /// The authority
        /// </summary>
        public string Authority
        {
            get { return _authority; }
        }

        /// <summary>
        /// The identifier
        /// </summary>
        public string Identifier
        {
            get { return _identifier; }
        }

        /// <summary>
        /// The version
        /// </summary>
        public string Version
        {
            get { return _version; }
        }

        public override string ToString()
        {
            return string.Format("urn:ogc:def:crs:{0}:{1}:{2}", Authority, Version, Identifier);
        }

        public bool Equals(CrsIdentifier other)
        {
            if (Authority != other.Authority) return false;
            if (Version != other.Version) return false;
            if (Identifier != other.Identifier) return false;
            return true;
        }
    }
}