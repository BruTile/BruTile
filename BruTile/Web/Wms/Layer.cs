// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace BruTile.Web.Wms
{
    public class Layer : XmlObject
    {
        private KeywordList _keywordListField;

        private List<string> _crsField = new List<string>();

        private List<Layer> _childLayers;

        public string Name { get; set; }

        public string Title { get; set; }

        public string Abstract { get; set; }

        public double? MinScaleDenominator { get; set; }

        public double? MaxScaleDenominator { get; set; }

        private int _cascaded;

        /// <summary>
        /// Gets a value whether this layer has been fetched from another wms server. If the value is greater than one, it has been gotten from the total number of servers
        /// </summary>
        public int Cascaded
        {
            get
            {
                return _cascaded;
            }
            set
            {
                if (value < 0)
                    throw WmsPropertyException.NonNegativeInteger("Cascaded", value);
                _cascaded = value;
            }
        }

        private int _fixedWidth;

        public int FixedWidth
        {
            get { return _fixedWidth; }
            set
            {
                if (value < 0)
                    throw WmsPropertyException.NonNegativeInteger("FixedWidth", value);
                _fixedWidth = value;
            }
        }

        private int _fixedHeight;

        public int FixedHeight
        {
            get { return _fixedHeight; }
            set
            {
                if (value < 0)
                    throw WmsPropertyException.NonNegativeInteger("FixedHeight", value);
                _fixedHeight = value;
            }
        }

        public Layer()
        {
            Queryable = false;
            Opaque = false;
            NoSubsets = false;
            BoundingBox = new List<BoundingBox>();
        }

        public Layer(XElement node, string ns)
        {
            var att = node.Attribute(XName.Get("queryable"));
            Queryable = att != null && att.Value == "1";

            att = node.Attribute(XName.Get("cascaded"));
            Cascaded = att != null ? int.Parse(att.Value, NumberFormatInfo.InvariantInfo) : 0;

            att = node.Attribute(XName.Get("opaque"));
            Opaque = att != null && att.Value == "1";

            att = node.Attribute(XName.Get("noSubsets"));
            NoSubsets = att != null && att.Value == "1";

            att = node.Attribute(XName.Get("fixedWidth"));
            FixedWidth = att != null ? int.Parse(att.Value, NumberFormatInfo.InvariantInfo) : 0;

            att = node.Attribute(XName.Get("fixedHeight"));
            FixedHeight = att != null ? int.Parse(att.Value, NumberFormatInfo.InvariantInfo) : 0;

            var element = node.Element(XName.Get("Name", ns));
            Name = element != null ? element.Value : string.Empty;

            element = node.Element(XName.Get("Title", ns));
            Title = element != null ? element.Value : string.Empty;

            element = node.Element(XName.Get("Abstract", ns));
            Abstract = element != null ? element.Value : string.Empty;

            element = node.Element(XName.Get("KeywordList", ns));
            if (element != null) _keywordListField = new KeywordList(element, ns);

            foreach (var el in node.Elements(XName.Get("CRS", ns)))
                _crsField.Add(el.Value);

            if (_crsField.Count == 0)
            { }

            element = node.Element(XName.Get("EX_GeographicBoundingBox", ns));
            if (element != null) ExGeographicBoundingBox = new ExGeographicBoundingBox(element, ns);

            BoundingBox = new List<BoundingBox>();
            foreach (var el in node.Elements(XName.Get("BoundingBox", ns)))
                BoundingBox.Add(new BoundingBox(el, ns));

            Dimension = new List<Dimension>();
            foreach (var el in node.Elements(XName.Get("Dimension", ns)))
                Dimension.Add(new Dimension(el, ns));

            element = node.Element(XName.Get("Attribution", ns));
            if (element != null) Attribution = new Attribution(element, ns);

            AuthorityURL = new List<AuthorityURL>();
            foreach (var el in node.Elements(XName.Get("AuthorityURL", ns)))
                AuthorityURL.Add(new AuthorityURL(el, ns));

            Identifier = new List<Identifier>();
            foreach (var el in node.Elements(XName.Get("Identifier", ns)))
                Identifier.Add(new Identifier(el, ns));

            MetadataURL = new List<MetadataURL>();
            foreach (var el in node.Elements(XName.Get("MetadataURL", ns)))
                MetadataURL.Add(new MetadataURL(el, ns));

            DataURL = new List<DataURL>();
            foreach (var el in node.Elements(XName.Get("DataURL", ns)))
                DataURL.Add(new DataURL(el, ns));

            FeatureListURL = new List<FeatureListURL>();
            foreach (var el in node.Elements(XName.Get("FeatureListURL", ns)))
                FeatureListURL.Add(new FeatureListURL(el, ns));

            Style = new List<Style>();
            foreach (var el in node.Elements(XName.Get("Style", ns)))
                Style.Add(new Style(el, ns));

            element = node.Element(XName.Get("MinScaleDenominator", ns));
            if (element != null)
                MinScaleDenominator = double.Parse(element.Value, NumberFormatInfo.InvariantInfo);

            element = node.Element(XName.Get("MaxScaleDenominator", ns));
            if (element != null)
                MaxScaleDenominator = double.Parse(element.Value, NumberFormatInfo.InvariantInfo);

            foreach (var layerNode in node.Elements(XName.Get("Layer", ns)))
                ChildLayers.Add(new Layer(layerNode, ns));
        }

        public KeywordList KeywordList
        {
            get
            {
                if ((_keywordListField == null))
                {
                    _keywordListField = new KeywordList();
                }
                return _keywordListField;
            }
            set
            {
                _keywordListField = value;
            }
        }

        public List<string> CRS
        {
            get
            {
                if ((_crsField == null))
                {
                    _crsField = new List<string>();
                }
                return _crsField;
            }
            set
            {
                _crsField = value;
            }
        }

        public ExGeographicBoundingBox ExGeographicBoundingBox { get; set; }

        public List<BoundingBox> BoundingBox { get; private set; }

        public List<Dimension> Dimension { get; private set; }

        public Attribution Attribution { get; set; }

        public List<AuthorityURL> AuthorityURL { get; private set; }

        public List<Identifier> Identifier { get; private set; }

        public List<MetadataURL> MetadataURL { get; private set; }

        public List<DataURL> DataURL { get; private set; }

        public List<FeatureListURL> FeatureListURL { get; private set; }

        public List<Style> Style { get; private set; }

        public List<Layer> ChildLayers
        {
            get
            {
                if ((_childLayers == null))
                {
                    _childLayers = new List<Layer>();
                }
                return _childLayers;
            }
            set
            {
                _childLayers = value;
            }
        }

        public bool Queryable { get; set; }

        public bool Opaque { get; set; }

        public bool NoSubsets { get; set; }

        #region Overrides of XmlObject

        public override void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (Queryable) writer.WriteAttributeString("queryable", "1");
            if (Cascaded > 0) writer.WriteAttributeString("cascaded", Cascaded.ToString(NumberFormatInfo.InvariantInfo));
            if (Opaque) writer.WriteAttributeString("opaque", "1");
            if (NoSubsets) writer.WriteAttributeString("noSubsets", "1");
            if (FixedWidth > 0) writer.WriteAttributeString("fixedWidth", FixedWidth.ToString(NumberFormatInfo.InvariantInfo));
            if (FixedHeight > 0) writer.WriteAttributeString("fixedHeight", FixedHeight.ToString(NumberFormatInfo.InvariantInfo));

            if (!string.IsNullOrEmpty(Name))
                writer.WriteElementString("Name", Namespace, Name);
            writer.WriteElementString("Title", Namespace, Title);
            if (!string.IsNullOrEmpty(Abstract))
                writer.WriteElementString("Abstract", Namespace, Abstract);
            WriteXmlItem("KeywordList", Namespace, writer, _keywordListField);
            WriteXmlList("CRS", Namespace, writer, _crsField);
            WriteXmlItem("EX_GeographicBoundingBox", Namespace, writer, ExGeographicBoundingBox);
            WriteXmlList("BoundingBox", Namespace, writer, BoundingBox);
            WriteXmlList("Dimension", Namespace, writer, Dimension);
            WriteXmlItem("Attribution", Namespace, writer, Attribution);
            WriteXmlList("AuthorityURL", Namespace, writer, AuthorityURL);
            WriteXmlList("Identifier", Namespace, writer, Identifier);
            WriteXmlList("MetadataURL", Namespace, writer, MetadataURL);
            WriteXmlList("DataURL", Namespace, writer, DataURL);
            WriteXmlList("FeatureListURL", Namespace, writer, DataURL);
            WriteXmlList("Style", Namespace, writer, Style);
            if (MinScaleDenominator.HasValue)
                writer.WriteElementString("MinScaleDenominator", Namespace, MinScaleDenominator.Value.ToString(NumberFormatInfo.InvariantInfo));
            if (MaxScaleDenominator.HasValue)
                writer.WriteElementString("MaxScaleDenominator", Namespace, MaxScaleDenominator.Value.ToString(NumberFormatInfo.InvariantInfo));
            WriteXmlList("Layer", Namespace, writer, ChildLayers);
        }

        public override XElement ToXElement(string @namespace)
        {
            return new XElement(XName.Get("Layer", @namespace));
        }

        #endregion Overrides of XmlObject
    }
}