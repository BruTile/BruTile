// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    using System.Collections.Generic;

    public class ServiceExceptionReport : XmlObject
    {
        public List<ServiceException> ServiceExceptions { get; private set; }

        public string Version { get; set; }

        public ServiceExceptionReport()
        {
            ServiceExceptions = new List<ServiceException>();
            Version = "1.3.0";
        }

        public ServiceExceptionReport(XElement node, string ns) : this()
        {
            var att = node.Attribute(XName.Get("version"));
            if (att != null)
                Version = att.Value;

            foreach (var serviceException in node.Elements(XName.Get("ServiceException", ns)))
                ServiceExceptions.Add(new ServiceException(serviceException, ns));
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public override XElement ToXElement(string @namespace)
        {
            throw new System.NotImplementedException();
        }

        #endregion Overrides of XmlObject
    }
}