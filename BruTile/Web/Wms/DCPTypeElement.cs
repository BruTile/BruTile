// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class DCPTypeElement : XmlObject
    {
        private readonly string _name;

        public string Name
        {
            get { return _name; }
        }

        private OnlineResource _onlineResourceField;

        protected DCPTypeElement(string name)
        {
            _name = name;
        }

        protected DCPTypeElement(string name, XElement node, string @namespace)
            : this(name)
        {
            var element = node.Element(XName.Get("OnlineResource", @namespace));
            if (element != null)
                OnlineResource = new OnlineResource(element, @namespace);
        }

        public OnlineResource OnlineResource
        {
            get
            {
                if ((_onlineResourceField == null))
                {
                    _onlineResourceField = new OnlineResource();
                }
                return _onlineResourceField;
            }
            set
            {
                _onlineResourceField = value;
            }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            if (CheckEmptyNode(reader, _name, Namespace))

                OnlineResource = new OnlineResource();
            OnlineResource.ReadXml(reader);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            WriteXmlItem(_name, Namespace, writer, OnlineResource);
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get(_name, @namespace),
                                OnlineResource.ToXElement(@namespace));
        }

        #endregion Overrides of XmlObject
    }
}