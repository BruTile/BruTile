// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Request : XmlObject
    {
        private OperationType _getCapabilitiesField;

        private OperationType _getMapField;

        private OperationType _getFeatureInfoField;

        private Dictionary<XName, OperationType> _extendedOperationField;

        public Request()
        { }

        public Request(XElement node, string @namespace)
        {
            foreach (var element in node.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "GetCapabilities":
                        GetCapabilities = new OperationType(element, @namespace);
                        break;
                    case "GetMap":
                        GetMap = new OperationType(element, @namespace);
                        break;
                    case "GetFeatureInfo":
                        GetFeatureInfo = new OperationType(element, @namespace);
                        break;
                    default:
                        ExtendedOperation.Add(element.Name.LocalName, new OperationType(element, @namespace));
                        break;
                }
            }

            if (GetCapabilities == null)
                throw WmsParsingException.ElementNotFound("GetCapabilities");

            if (GetMap == null)
                throw WmsParsingException.ElementNotFound("GetMap");
        }

        public OperationType GetCapabilities
        {
            get
            {
                if ((_getCapabilitiesField == null))
                {
                    _getCapabilitiesField = new OperationType();
                }
                return _getCapabilitiesField;
            }
            set
            {
                _getCapabilitiesField = value;
            }
        }

        public OperationType GetMap
        {
            get
            {
                if ((_getMapField == null))
                {
                    _getMapField = new OperationType();
                }
                return _getMapField;
            }
            set
            {
                _getMapField = value;
            }
        }

        public OperationType GetFeatureInfo
        {
            get
            {
                if ((_getFeatureInfoField == null))
                {
                    _getFeatureInfoField = new OperationType();
                }
                return _getFeatureInfoField;
            }
            set
            {
                _getFeatureInfoField = value;
            }
        }

        public Dictionary<XName, OperationType> ExtendedOperation
        {
            get
            {
                if ((_extendedOperationField == null))
                {
                    _extendedOperationField = new Dictionary<XName, OperationType>();
                }
                return _extendedOperationField;
            }
            set
            {
                _extendedOperationField = value;
            }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
  
            reader.MoveToContent();
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "GetCapabilities":
                            _getCapabilitiesField = new OperationType();
                            _getCapabilitiesField.ReadXml(reader);
                            break;
                        case "GetMap":
                            _getMapField = new OperationType();
                            _getMapField.ReadXml(reader);
                            break;
                        case "GetFeatureInfo":
                            _getFeatureInfoField = new OperationType();
                            _getFeatureInfoField.ReadXml(reader);
                            break;

                        default:
                            var name = XName.Get(reader.LocalName, reader.NamespaceURI);
                            var operation = new OperationType();
                            operation.ReadXml(reader);
                            ExtendedOperation.Add(name, operation);
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
            WriteXmlItem("GetCapabilities", Namespace, writer, GetCapabilities);
            WriteXmlItem("GetMap", Namespace, writer, GetMap);
            WriteXmlItem("GetFeatureInfo", Namespace, writer, _getFeatureInfoField);

            if (_extendedOperationField != null)
            {
                foreach (var operationType in _extendedOperationField)
                {
                    var key = operationType.Key;
                    WriteXmlItem(key.LocalName, key.NamespaceName, writer, operationType.Value);
                }
            }
        }

        public override XElement ToXElement(string @namespace)
        {
            throw new System.NotImplementedException();
        }

        #endregion Overrides of XmlObject
    }
}