// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Wms;

public class WmsCapabilities
{
    private Service _serviceField;
    private Capability _capabilityField;
    public string UpdateSequence { get; set; }
    public ServiceExceptionReport ServiceExceptionReport { get; }
    public WmsVersion Version { get; set; }

    public WmsCapabilities()
        : this(WmsVersionEnum.Version_1_3_0)
    { }

    [Obsolete("Use WmsCapabilities.CreateAsync")]
    public WmsCapabilities(string url, ICredentials credentials = null)
        : this(new Uri(url), credentials)
    { }

    public static async Task<WmsCapabilities> CreateAsync(string url, ICredentials credentials = null)
    {
        return await CreateAsync(new Uri(url), credentials);
    }

    public static async Task<WmsCapabilities> CreateAsync(Uri uri, ICredentials credentials = null)
    {
        var document = await ToXDocumentAsync(CompleteGetCapabilitiesRequest(uri), credentials);
        return new WmsCapabilities(document);
    }

    [Obsolete("Use WmsCapabilities.CreateAsync")]
    public WmsCapabilities(Uri uri, ICredentials credentials = null)
        : this(ToXDocumentAsync(CompleteGetCapabilitiesRequest(uri), credentials).Result)
    { }

    public WmsCapabilities(string version)
    {
        Version = new WmsVersion(version);
    }

    public WmsCapabilities(WmsVersionEnum version)
    {
        Version = new WmsVersion(version);
    }

    public WmsCapabilities(Stream stream)
        : this(XDocument.Load(stream))
    { }

    public WmsCapabilities(XDocument doc)
        : this()
    {
        if (doc.Root != null && doc.Root.Name == "ServiceExceptionReport")
        {
            ServiceExceptionReport = new ServiceExceptionReport(doc.Root, string.Empty);
            return;
        }

        var node = doc.Element(XName.Get("WMT_MS_Capabilities")) ?? doc.Element(XName.Get("WMS_Capabilities"));
        if (node == null)
        {
            // Try load root node with xmlns="http://www.opengis.net/wms"
            node = doc.Element(XName.Get("WMS_Capabilities", "http://www.opengis.net/wms"));
            if (node == null)
            {
                throw WmsParsingException.ElementNotFound("WMS_Capabilities or WMT_MS_Capabilities");
            }
        }

        var att = node.Attribute(XName.Get("version")) ?? throw WmsParsingException.AttributeNotFound("version");
        Version = new WmsVersion(att.Value);

        var @namespace = Version.Version == WmsVersionEnum.Version_1_3_0
            ? "http://www.opengis.net/wms"
            : string.Empty;
        XmlObject.Namespace = @namespace;

        UpdateSequence = node.Attribute("updateSequence")?.Value;

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
        get => _serviceField ??= new Service();
        set => _serviceField = value;
    }

    public Capability Capability
    {
        get => _capabilityField ??= new Capability();
        set => _capabilityField = value;
    }

    #region Overrides of XmlObject

    public static WmsCapabilities Parse(Stream stream)
    {
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };

        using var reader = XmlReader.Create(stream, settings);
        reader.MoveToContent();

        var version = reader.GetAttribute("version");
        var wms = new WmsCapabilities(version)
        {
            UpdateSequence = reader.GetAttribute("updateSequence")
        };

        if (reader.IsEmptyElement)
        {
            reader.Read();
            return null;
        }

        reader.ReadStartElement(wms.Version.Version >= WmsVersionEnum.Version_1_3_0
            ? "WMS_Capabilities"
            : "WMT_MS_Capabilities");

        reader.MoveToContent();
        wms.Service.ReadXml(reader.ReadSubtree());
        reader.ReadEndElement();

        reader.MoveToContent();
        wms.Capability.ReadXml(reader.ReadSubtree());
        reader.ReadEndElement();

        reader.ReadEndElement();
        return wms;
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
        using var sw = new StreamWriter(stream);
        Save(sw);
    }

    public void Save(TextWriter streamWriter)
    {
        using var xmlWriter = XmlWriter.Create(streamWriter);
        xmlWriter.WriteStartDocument();

        Version.WriteCapabilitiesDocType(xmlWriter);
        Version.WriteStartRootElement(xmlWriter, UpdateSequence);

        XmlObject.WriteXmlItem("Service", Version.Namespace, xmlWriter, Service);
        XmlObject.WriteXmlItem("Capability", Version.Namespace, xmlWriter, Capability);

        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndDocument();
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
                new XDocumentType("WMS_MT_Capabilities", string.Empty,
                    WmsVersion.System(Version.Version, "capabilities"), string.Empty),
                ToXElement(string.Empty)
            );
        }

        return new XDocument(declaration, ToXElement(WmsNamespaces.Wms));
    }

    #endregion Overrides of XmlObject

    private static async Task<XDocument> ToXDocumentAsync(Uri uri, ICredentials credentials)
    {
        await using var stream = await GetRemoteXmlStreamAsync(uri, credentials);
        var sr = new StreamReader(stream);
        var ret = XDocument.Load(sr);
        return ret;

    }

    private static async Task<Stream> GetRemoteXmlStreamAsync(Uri uri, ICredentials credentials)
    {
        var httpClientHandler = new HttpClientHandler();
        try
        {
            // Blazor does not support this,
            if (credentials != null)
                httpClientHandler.Credentials = credentials;
        }
        catch (PlatformNotSupportedException)
        {
        }

        using var httpClient = new HttpClient(httpClientHandler)
        {
            Timeout = TimeSpan.FromMilliseconds(30000)
        };
        return await httpClient.GetStreamAsync(uri).ConfigureAwait(false);
    }

    #region validation and completion of request url

    internal static Uri CompleteGetCapabilitiesRequest(Uri serverUrl)
    {
        if (serverUrl == null)
            throw new ArgumentNullException(nameof(serverUrl));

        if (string.IsNullOrWhiteSpace(serverUrl.Scheme))
            throw new ArgumentException("The url provides no schema");

        if (string.IsNullOrWhiteSpace(serverUrl.Authority))
            throw new ArgumentException("The url provides no hostname or ip-address");

        // Check which parameters are supplied and if the parameters are correct
        var parameters = new HashSet<string>();
        if (!ValidateGetCapabilitiesRequest(serverUrl.Query, parameters))
        {
            // There are parameters missing, we need to add them!
            var qry = new StringBuilder(serverUrl.Query);

            // Determine the insert position
            var pos = serverUrl.Query.IndexOf("?", StringComparison.OrdinalIgnoreCase) + 1;

            // Add missing quotation mark
            if (pos == 0)
            {
                qry.Append('?');
                pos = qry.Length;
            }

            // Ensure REQUEST=GetCapabilities&
            if (!parameters.Contains("REQUEST"))
                qry.Insert(pos, "REQUEST=GetCapabilities&");

            // Ensure VERSION=x.x.x&
            // According to http://cite.opengeospatial.org/pub/cite/files/edu/wms/text/operations.html#id6
            // this value is mandatory, too. If not specified, we take the most recent.
            if (!parameters.Contains("VERSION"))
                qry.Insert(pos, "VERSION=1.3.0&");

            // Ensure SERVICE=WMS
            if (!parameters.Contains("SERVICE"))
                qry.Insert(pos, "SERVICE=WMS&");

            // Write modified server URL
            var serverUrlBuilder = new UriBuilder(serverUrl.Scheme, serverUrl.Host, serverUrl.Port, serverUrl.AbsolutePath)
            {
                Query = qry.ToString().Substring(1)
            };
            serverUrl = serverUrlBuilder.Uri;
        }

        return serverUrl;
    }

    /// <summary>
    /// Method to check a GetCapabilities request URL for completeness and validity
    /// </summary>
    /// <param name="query">The query part of the request URL</param>
    /// <param name="parameters">
    /// An optional set that stores the supplied parameters. 
    /// All encountered parameters are added to the set.
    /// This set can be used to later supply missing values
    /// </param>
    /// <returns><value>true</value>If the request string contains all mandatory parameters</returns>
    /// <exception cref="ArgumentException">Thrown if supplied</exception>
    internal static bool ValidateGetCapabilitiesRequest(string query, HashSet<string> parameters = null)
    {
        var flag = 0;

        var posQuestion = query.IndexOf('?');
        if (posQuestion > -1)
        {
            foreach (var parameter in query.Substring(posQuestion + 1).Split('&'))
            {
                var kvp = parameter.ToUpperInvariant().Split('=');
                parameters?.Add(kvp[0]);
                switch (kvp[0])
                {
                    case "SERVICE":
                        if (kvp[1] != "WMS")
                            throw new ArgumentException(
                                $"Wrong service name ('{parameter.Substring(kvp[0].Length + 1)}')",
                                nameof(query));
                        flag |= 1;
                        break;
                    case "VERSION":
                        switch (kvp[1])
                        {
                            case "1.0.0":
                            case "1.0.7":
                            case "1.1.0":
                            case "1.1.1":
                            case "1.3":
                            case "1.3.0":
                                flag |= 2;
                                continue;
                        }

                        throw new ArgumentException(
                            $"Invalid version for WMS ('{parameter.Substring(kvp[0].Length + 1)}')",
                            nameof(query));
                    case "REQUEST":
                        if (kvp[1] != "GETCAPABILITIES")
                            throw new ArgumentException(
                                $"Wrong operation name for GetCapabilities ('{parameter.Substring(kvp[0].Length + 1)}')",
                                nameof(query));
                        flag |= 4;
                        break;
                }
            }
        }

        return flag == 7;
    }
    #endregion
}
