// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Xml;

namespace BruTile.Wms
{
    // ReSharper disable InconsistentNaming
    public enum WmsVersionEnum
    {
        Version_1_0_0 = 0x010000,
        Version_1_0_7 = 0x010007,
        Version_1_1_0 = 0x010100,
        Version_1_1_1 = 0x010101,
        Version_1_3_0 = 0x010300
    }

    // ReSharper restore InconsistentNaming

    public class WmsVersion
    {
        public readonly string WmsCapabilityNodeTag;

        public readonly string VersionString;
        public readonly WmsVersionEnum Version;

        public WmsVersion(string version)
        {
            VersionString = version;
            switch (version)
            {
                case "1.0.0":
                    Version = WmsVersionEnum.Version_1_0_0;
                    break;
                case "1.0.7":
                    Version = WmsVersionEnum.Version_1_0_7;
                    break;
                case "1.1.0":
                    Version = WmsVersionEnum.Version_1_1_0;
                    break;
                case "1.1.1":
                    Version = WmsVersionEnum.Version_1_1_0;
                    break;
                default:
                    //case "1.3.0":
                    Version = WmsVersionEnum.Version_1_3_0;
                    VersionString = "1.3.0";
                    break;
            }
        }

        public WmsVersion(WmsVersionEnum version)
        {
            Version = version;
            VersionString = version switch
            {
                WmsVersionEnum.Version_1_0_0 => "1.0.0",
                WmsVersionEnum.Version_1_0_7 => "1.0.7",
                WmsVersionEnum.Version_1_1_0 => "1.1.0",
                WmsVersionEnum.Version_1_1_1 => "1.1.1",
                _ => "1.3.0",//case WmsVersionEnum.Version_1_3_0:
            };
            WmsCapabilityNodeTag = version < WmsVersionEnum.Version_1_3_0
                                       ? "WMT_MS_Capability"
                                       : "WMS_Capability";
        }

        public string Namespace => Version < WmsVersionEnum.Version_1_3_0 ? string.Empty : WmsNamespaces.Wms;

        public void WriteCapabilitiesDocType(XmlWriter writer)
        {
            if (Version >= WmsVersionEnum.Version_1_3_0)
                return;

            writer.WriteDocType("WMT_MS_Capability", null, System(Version, "capabilities"), null);
        }

        public void WriteServiceExceptionReportDocType(XmlWriter writer)
        {
            if (Version > WmsVersionEnum.Version_1_0_7 && Version < WmsVersionEnum.Version_1_3_0)
            {
                writer.WriteDocType("ServiceExceptionReport", null, System(Version, "exception"), null);
            }
        }

        public string SchemaUri(string name)
        {
            return System(Version, name);
        }

        internal static string System(WmsVersionEnum version, string name)
        {
            const string urlFormat = "http://schemas.opengis.net/wms/{0}.{1}.{2}/{3}_{0}_{1}_{2}.dtd";
            var versionBytes = BitConverter.GetBytes((int)version);

            if (!BitConverter.IsLittleEndian) Array.Reverse(versionBytes);
            return string.Format(urlFormat, versionBytes[0], versionBytes[1], versionBytes[2], name);
        }

        internal void WriteStartRootElement(XmlWriter writer, string updateSequence)
        {
            writer.WriteAttributeString("version", VersionString);
            if (updateSequence != null)
                writer.WriteAttributeString("updateSequence", updateSequence);

            if (Version >= WmsVersionEnum.Version_1_3_0)
            {
                writer.WriteAttributeString("xmlns", "xlink", string.Empty, WmsNamespaces.Xlink);
                writer.WriteAttributeString("xmlns", "xsi", string.Empty, WmsNamespaces.Xsi);
                writer.WriteAttributeString("xsi", "schemaLocation", string.Empty, $"{WmsNamespaces.Wms}");
                writer.WriteStartElement("WMS_Capabilities", WmsNamespaces.Wms, WmsNamespaces.WmsSchemaUrl(Version, "capabilities"));
            }
            else
            {
                writer.WriteStartElement("WMT_MS_Capabilities");
            }
        }
    }
}
