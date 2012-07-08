using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BruTile.Web.Wms;
using NUnit.Framework;
using Exception = BruTile.Web.Wms.Exception;

namespace BruTile.Tests.Web
{
    public class SerializationTest
    {
        [Ignore("not working correctly")]
        [Test]
        public void TestLocal()
        {
            using (var fs = new StreamReader(File.OpenRead(Path.Combine("Resources", @"capabilities_1_3_0.xml"))))
            {
                var xml = fs.ReadToEnd();
                var doc = XDocument.Parse(xml);
                var wms1 = new WmsCapabilities(doc);
            }

            var wms = WmsCapabilities.Parse(File.OpenRead(Path.Combine("Resources", @"capabilities_1_3_0.xml")));

            Console.WriteLine(wms);

            /*
             * 
            s = new XmlSerializer(typeof(ServiceExceptionReport));
            var ser = (ServiceExceptionReport)s.Deserialize(XmlReader.Create(File.OpenRead(Path.Combine(path, @"exceptions_1_3_0.xml")),
                new XmlReaderSettings { IgnoreComments = true }));

            Assert.IsNotNull(ser);
             */
        }

        [Ignore("not completely implemented")]
        [Test]
        public void TestWrite()
        {
            var wms = new WmsCapabilities { Service = { Title = "Felix' WMS" } };

            var s = new XmlSerializer(typeof(WmsCapabilities));
            var sb = new StringBuilder();

            var xmlSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment, Encoding = Encoding.UTF8, Indent = true, IndentChars = "  " };
            s.Serialize(XmlWriter.Create(new StringWriter(sb), xmlSettings), wms);
            Console.WriteLine(sb.ToString());
        }

        [Ignore("not completely implemented")]
        [Test]
        public void TestSimple()
        {
            var bb = new BoundingBox { CRS = "EPSG:4326", MinX = 1, MinY = 2, MaxX = 3, MaxY = 4, ResX = 99, ResY = 100 };
            var bb2 = SerializeDeserialize(bb);

            var onlineResource = new OnlineResource { Href = "http://localhost:80?", Type = "simple" };
            var or2 = SerializeDeserialize(onlineResource);

            var logoUrl = new LogoURL() { Format = "png", Height = 256, Width = 256, OnlineResource = onlineResource };
            var lu2 = SerializeDeserialize(logoUrl);

            var au = new AuthorityURL { Name = "IVV", OnlineResource = new OnlineResource { Href = "http://www.ivv-aachen.de" } };
            var au2 = SerializeDeserialize(au);

            var ca = new ContactAddress { Address = "Oppenhoffallee 171", City = "Aachen", PostCode = "52066", Country = "Germany" };
            var ca2 = SerializeDeserialize(ca);

            var pp = new ContactPersonPrimary { ContactOrganization = "Ingenieurgruppe IVV", ContactPerson = "Felix Obermaier" };
            var pp2 = SerializeDeserialize(pp);

            var ci = new ContactInformation { ContactAddress = ca, ContactElectronicMailAddress = "spam@ivv-aachen.de", ContactPersonPrimary = pp };
            var ci2 = SerializeDeserialize(ci);

            var dataUrl = new DataURL { Format = "png", OnlineResource = onlineResource };
            var dataUrl2 = SerializeDeserialize(dataUrl);

            var dcpType = new DCPType { Http = new Http { Get = new Get { OnlineResource = onlineResource } } };
            var dcpType2 = SerializeDeserialize(dcpType);

            var dim = new Dimension { Name = "Meter", Units = "1", UnitSymbol = "m", MultipleValues = false, Value = "1.0" };
            var dim2 = SerializeDeserialize(dim);

            var ex = new Exception();
            ex.Format.Add("text/plan");
            ex.Format.Add("text/xml");
            var ex2 = SerializeDeserialize(ex);

            var exb = new ExGeographicBoundingBox
                          {
                              EastBoundLongitude = -170,
                              NorthBoundLatitude = 90,
                              SouthBoundLatitude = -90,
                              WestBoundLongitude = 170
                          };
            var exb2 = SerializeDeserialize(exb);

            var flUrl = new FeatureListURL()
                            {
                                Format = "txt",
                                OnlineResource = new OnlineResource { Href = "http://tmp:80/?", Type = "simple" }
                            };

            var id = new Identifier { Authority = "IVV", Value = "Ingenieurgruppe IVV" };
            var id2 = SerializeDeserialize(id);

            var key = new KeywordList();
            key.Keyword.Add(new Keyword { Vocabulary = "explicit", Value = "Lyrics" });
            key.Keyword.Add(new Keyword { Value = "Songtext" });
            var key2 = SerializeDeserialize(key);

            var legendUrl = new LegendURL { Format = "png", Height = 256, Width = 256, OnlineResource = onlineResource };
            var legendUrl2 = SerializeDeserialize(legendUrl);

            var layer = new Layer
                            {
                                Abstract = "Sample layer",
                                Attribution =
                                    new Attribution
                                        {
                                            LogoURL = logoUrl,
                                            OnlineResource = onlineResource,
                                            Title = "Attribution Title"
                                        },
                                Cascaded = 1,
                                CRS = new List<string>(new[] { "EPSG:4326" }),
                                ExGeographicBoundingBox = exb,
                                Title = "Layer Title",
                                KeywordList = key,
                                Queryable = false,
                                Name = "Layer Name"
                            };
            var layer2 = SerializeDeserialize(layer);

            var cap = new Capability { };
            var cap2 = SerializeDeserialize(cap);
        }

        private static T SerializeDeserialize<T>(T input)
        {
            var t = typeof(T);
            var ns = new XmlSerializerNamespaces();
            ns.Add("wms", XmlObject.Namespace);

            var s = new XmlSerializer(t);
            var path = t.Name + ".xml";
            using (var fs = new FileStream(path, FileMode.Create))
                s.Serialize(fs, input);

            var read = (T)s.Deserialize(XmlReader.Create(new StreamReader(path)));
            return read;
        }
    }
}