// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Http : XmlObject
    {
        private Get _getField;

        private Post _postField;

        public Http()
        {
        }

        public Http(XElement node, string ns)
        {
            var element = node.Element(XName.Get("Get", ns));
            if (element != null) _getField = new Get(element, ns);
            element = node.Element(XName.Get("Post", ns));
            if (element != null) _postField = new Post(element, ns);
        }

        public Get Get
        {
            get
            {
                if ((_getField == null))
                {
                    _getField = new Get();
                }
                return _getField;
            }
            set
            {
                _getField = value;
            }
        }

        public Post Post
        {
            get
            {
                if ((_postField == null))
                {
                    _postField = new Post();
                }
                return _postField;
            }
            set
            {
                _postField = value;
            }
        }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            if (CheckEmptyNode(reader, "HTTP", Namespace))
                return;

            while (!reader.EOF)
            {
                if (reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "Get":
                            Get = new Get();
                            Get.ReadXml(reader);
                            break;
                        case "Post":
                            _postField = new Post();
                            Post.ReadXml(reader);
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
                else
                    reader.Read();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            WriteXmlItem("Get", Namespace, writer, Get);
            WriteXmlItem("Post", Namespace, writer, _postField);
        }

        public override XElement ToXElement(string @namespace)
        {
            if (_postField != null)
            {
                return new XElement(XName.Get("HTTP", Namespace),
                    Get.ToXElement(@namespace), Post.ToXElement(@namespace));
            }
            return new XElement(XName.Get("HTTP", Namespace),
                Get.ToXElement(@namespace));
        }

        #endregion Overrides of XmlObject
    }
}