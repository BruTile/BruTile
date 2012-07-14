// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Capability : XmlObject
    {
        private Request _requestField;

        private List<string> _exceptionField;

        private Dictionary<XName, XNode> _extendedCapabilitiesField;

        private Layer _layerField;

        public Capability()
        {
        }

        public Capability(XElement node, string @namespace)
        {
            var element = node.Element(XName.Get("Request", @namespace));
            if (element == null)
                throw WmsParsingException.ElementNotFound("Request");
            Request = new Request(element, @namespace);

            foreach (var el in node.Elements())
            {
                if (el.Name.LocalName == "Request" || el.Name.LocalName == "Layer")
                    continue;

                if (el.Name.LocalName == "Exception")
                {
                    Exception.Add(el.Value);
                    continue;
                }

                ExtendedCapabilities.Add(el.Name, el);
            }

            if (Exception.Count == 0)
                throw WmsParsingException.ElementNotFound("Exception");

            bool baseNodeCreated = false;
            foreach (var layerNode in node.Elements(XName.Get("Layer", @namespace)))
            {
                var layer = new Layer(layerNode, @namespace);
                if (_layerField == null)
                {
                    _layerField = layer;
                }

                else if (_layerField != null && !baseNodeCreated)
                {
                    var tmpLayer = new Layer();
                    tmpLayer.Title = "Root Layer";
                    tmpLayer.ChildLayers.Add(Layer);
                    tmpLayer.ChildLayers.Add(layer);
                    Layer = tmpLayer;
                    baseNodeCreated = true;
                }
                else
                {
                    _layerField.ChildLayers.Add(layer);
                }
            }
        }

        public Request Request
        {
            get
            {
                if ((_requestField == null))
                {
                    _requestField = new Request();
                }
                return _requestField;
            }
            set
            {
                _requestField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayAttribute(Order = 1)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Format", IsNullable = false)]
        public List<string> Exception
        {
            get
            {
                if ((_exceptionField == null))
                {
                    _exceptionField = new List<string>();
                }
                return _exceptionField;
            }
            set
            {
                _exceptionField = value;
            }
        }

        public Dictionary<XName, XNode> ExtendedCapabilities
        {
            get
            {
                if ((_extendedCapabilitiesField == null))
                {
                    _extendedCapabilitiesField = new Dictionary<XName, XNode>();
                }
                return _extendedCapabilitiesField;
            }
            set
            {
                _extendedCapabilitiesField = value;
            }
        }

        public Layer Layer
        {
            get
            {
                if ((_layerField == null))
                {
                    _layerField = new Layer();
                }
                return _layerField;
            }
            set
            {
                _layerField = value;
            }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            if (CheckEmptyNode(reader, "Capability", string.Empty, true))
                throw WmsParsingException.ElementNotFound("Capability");

            bool baseLayerCreated = false;

            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "Request":
                            _requestField = new Request();
                            _requestField.ReadXml(reader.ReadSubtree());
                            break;

                        case "Exception":
                            if (_exceptionField == null)
                                _exceptionField = new List<string>();
                            var subReader = reader.ReadSubtree();
                            while (!subReader.EOF)
                            {
                                reader.ReadStartElement("Format");
                                var format = reader.ReadContentAsString();
                                reader.ReadEndElement();
                                _exceptionField.Add(format);
                            }
                            break;
                        case "Layer":
                            if (_layerField == null)
                            {
                                _layerField = new Layer();
                                _layerField.ReadXml(reader);
                            }
                            else
                            {
                                if (!baseLayerCreated)
                                {
                                    var tmp = _layerField;
                                    _layerField = new Layer();
                                    _layerField.Name = _layerField.Title = "Created base layer";
                                    _layerField.ChildLayers.Add(tmp);
                                    baseLayerCreated = true;
                                }
                                var layer = new Layer();
                                layer.ReadXml(reader);
                                _layerField.ChildLayers.Add(layer);
                            }
                            break;

                        default:
                            ExtendedCapabilities.Add(XName.Get(reader.LocalName, reader.NamespaceURI), XNode.ReadFrom(reader));
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
            WriteXmlItem("Request", Namespace, writer, Request);
            writer.WriteStartElement("Exception", Namespace);
            if (_exceptionField != null)
            {
                foreach (var ex in Exception)
                    writer.WriteElementString("Format", Namespace, ex);
            }
            writer.WriteEndElement();

            if (_extendedCapabilitiesField != null)
            {
                foreach (var xElement in _extendedCapabilitiesField)
                    xElement.Value.WriteTo(writer);
            }

            WriteXmlItem("Layer", Namespace, writer, _layerField);
        }

        public override XElement ToXElement(string @namespace)
        {
            throw new System.NotImplementedException();
        }

        #endregion Overrides of XmlObject
    }
}