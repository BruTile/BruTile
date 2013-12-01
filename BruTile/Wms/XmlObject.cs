// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BruTile.Web.Wms
{
    public static class WmsNamespaces
    {
        public static string Xlink { get { return "http://www.w3.org/1999/xlink"; } }

        public static string Xsi { get { return "http://www.w3.org/2001/XMLSchema-instance"; } }

        public static string Wms { get { return "http://schemas.opengis.net"; } }

        public static string WmsSchemaUrl(WmsVersionEnum version, string schemaName)
        {
            const string urlFormat = "http://schemas.opengis.net/wms/{0}.{1}.{2}/{4}_{0}_{1}_{2}.dtd";
            var versionBytes = BitConverter.GetBytes((int)version);

            if (!BitConverter.IsLittleEndian) Array.Reverse(versionBytes);
            return string.Format(urlFormat, versionBytes[0], versionBytes[1], versionBytes[2], schemaName);
        }
    }

    public abstract class XmlObject : IXmlSerializable
    {
        public const string WmsNamespaceUri = "http://www.opengis.net/wms";

        static XmlObject()
        {
            Namespace = WmsNamespaceUri;
        }

        #region Implementation of IXmlSerializable

        public XmlSchema GetSchema()
        {
            return null;
        }

        public static string Namespace { get; set; }

        public abstract void ReadXml(XmlReader reader);

        public abstract void WriteXml(XmlWriter writer);

        public abstract XElement ToXElement(string @namespace);

        public override string ToString()
        {
            return ToXElement(Namespace).ToString();
        }

        #endregion Implementation of IXmlSerializable

        public static void WriteXmlList(string element, string @namespace, XmlWriter writer, IList<string> items)
        {
            if (items == null || items.Count == 0) return;

            foreach (var item in items)
                writer.WriteElementString(element, @namespace, item);
        }

        protected static void WriteXmlList<T>(string element, string @namespace, XmlWriter writer, IList<T> items)
            where T : class, IXmlSerializable
        {
            if (items == null || items.Count == 0) return;

            foreach (var item in items)
            {
                WriteXmlItem(element, @namespace, writer, item);
            }
        }

        protected internal static void WriteXmlItem<T>(string element, string @namespace, XmlWriter writer, T item)
            where T : class, IXmlSerializable
        {
            if (item == null)
                return;

            writer.WriteStartElement(element, @namespace);
            item.WriteXml(writer);
            writer.WriteEndElement();
        }

        protected static bool CheckEmptyNode(XmlReader reader, string localName, string @namespace, bool advance = true)
        {
            reader.MoveToContent();
            var isEmpty = reader.IsEmptyElement;
            if (advance)
                reader.ReadStartElement(localName /*, @namespace*/ );
            return isEmpty;
        }
    }
}