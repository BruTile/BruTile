using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using BruTile.Extensions;

namespace BruTile.Web.Wms
{
    public class WmsCapabilities
    {
        private Service _serviceField;
        private Capability _capabilityField;

        public int? UpdateSequence { get; set; }

        public WmsCapabilities()
            : this(WmsVersionEnum.Version_1_3_0)
        {
        }

        public WmsCapabilities(string version)
        {
            Version = new WmsVersion(version);
        }

        public WmsCapabilities(WmsVersionEnum version)
        {
            Version = new WmsVersion(version);
        }

        public WmsCapabilities(XDocument doc)
            : this()
        {
            XElement node;
            if (doc.FirstNode is XDocumentType)
            {
                var docType = doc.FirstNode;
                node = (XElement)docType.NextNode;
            }
            else
                node = (XElement)doc.FirstNode;

            var att = node.Attribute(XName.Get("version"));
            if (att == null)
                throw WmsParsingException.AttributeNotFound("version");
            Version = new WmsVersion(att.Value);

            att = node.Attribute("updateSequence");
            if (att != null)
                UpdateSequence = int.Parse(att.Value, NumberFormatInfo.InvariantInfo);

            var @namespace = Version.Version == WmsVersionEnum.Version_1_3_0
                            ? "http://www.opengis.net/wms"
                            : string.Empty;

            XmlObject.Namespace = @namespace;

            var element = node.Element(XName.Get("Service", @namespace));
            if (element == null)
            {
                XmlObject.Namespace = @namespace = string.Empty;
                element = node.Element(XName.Get("Service", @namespace));
            }
            if (element == null)
                throw WmsParsingException.ElementNotFound("Service");

            Service = new Service(element, @namespace);

            element = node.Element(XName.Get("Capability", @namespace));
            if (element == null)
                throw WmsParsingException.ElementNotFound("Capability");

            Capability = new Capability(element, @namespace);
        }

        public Service Service
        {
            get
            {
                if ((_serviceField == null))
                {
                    _serviceField = new Service();
                }
                return _serviceField;
            }
            set
            {
                _serviceField = value;
            }
        }

        public Capability Capability
        {
            get
            {
                if ((_capabilityField == null))
                {
                    _capabilityField = new Capability();
                }
                return _capabilityField;
            }
            set
            {
                _capabilityField = value;
            }
        }

        [Obsolete("Use Version instead")]
        public string WmsVersion { get { return Version.VersionString; } }

        public WmsVersion Version { get; set; }

        #region Overrides of XmlObject

        public static WmsCapabilities Parse(Stream stream)
        {
            var settings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Ignore};
            
            using (var reader = XmlReader.Create(stream, settings))
            {
                reader.MoveToContent();

                var version = reader.GetAttribute("version");
                var wms = new WmsCapabilities(version);
                var updateSequence = reader.GetAttribute("updateSequence");
                if (!string.IsNullOrEmpty(updateSequence))
                    wms.UpdateSequence = int.Parse(updateSequence, NumberFormatInfo.InvariantInfo);

                if (reader.IsEmptyElement)
                {
                    reader.Read();
                    return null;
                }

                if (wms.Version.Version >= WmsVersionEnum.Version_1_3_0)
                    reader.ReadStartElement("WMS_Capabilities");
                else
                {
                    reader.ReadStartElement("WMT_MS_Capabilities");
                }

                reader.MoveToContent();
                wms.Service.ReadXml(reader.ReadSubtree());
                reader.ReadEndElement();

                reader.MoveToContent();
                wms.Capability.ReadXml(reader.ReadSubtree());
                reader.ReadEndElement();

                reader.ReadEndElement();
                return wms;
            }
        }

        public string ToXml()
        {
            var sb = new StringBuilder();
            using (var s = new StringWriter(sb))
            {
                Save(s);
            }
            return sb.ToString();
        }

        public void Save(Stream stream)
        {
            using (var sw = new StreamWriter(stream))
                Save(sw);
        }

        public void Save(TextWriter streamWriter)
        {
            using (var xmlWriter = XmlWriter.Create(streamWriter))
            {
                xmlWriter.WriteStartDocument();

                Version.WriteCapabilitiesDocType(xmlWriter);
                Version.WriteStartRootElement(xmlWriter, UpdateSequence);

                XmlObject.WriteXmlItem("Service", Version.Namespace, xmlWriter, Service);
                XmlObject.WriteXmlItem("Capability", Version.Namespace, xmlWriter, Capability);

                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
            }
        }

        public XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get(Version.WmsCapabilityNodeTag, @namespace),
                Service.ToXElement(@namespace), Capability.ToXElement(@namespace));
        }

        public XDocument ToXDocument()
        {
            var declaration = new XDeclaration("1.0", "UTF-8", "no");
            if (Version.Version < WmsVersionEnum.Version_1_3_0)
            {
                return new XDocument(declaration,
                    new XDocumentType("WMS_MT_Capabilities", string.Empty, Wms.WmsVersion.System(Version.Version, "capabilities"), string.Empty),
                    ToXElement(string.Empty)
                    );
            }
            return new XDocument(declaration, ToXElement(WmsNamespaces.Wms));
        }

        #endregion Overrides of XmlObject

        private static XDocument ToXDocument(Uri uri)
        {
            Stream stream = GetRemoteXmlStream(uri);
            var sr = new StreamReader(stream);
            var ret = XDocument.Load(sr);
            return ret;
        }

        private static Stream GetRemoteXmlStream(Uri uri)
        {
            var myRequest = (HttpWebRequest)WebRequest.Create(uri);
            var myResponse = myRequest.GetSyncResponse(30000);
            var stream = myResponse.GetResponseStream();
            return stream;
        }
    }
}