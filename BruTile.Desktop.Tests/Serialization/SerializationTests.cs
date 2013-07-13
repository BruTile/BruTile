using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using NUnit.Framework;

namespace BruTile.Tests.Serialization
{
    public class SerializationTests
    {
        [Test]
        public void TestExtent()
        {
            var e1 = new Extent(-10, -10, 10, 10);
            var e2 = SandD(e1);
            Assert.AreEqual(e1.CenterX, e2.CenterX);
            Assert.AreEqual(e1.CenterY, e2.CenterY);
            Assert.AreEqual(e1.Width, e2.Width);
            Assert.AreEqual(e1.Height, e2.Height);
        }

        [Test]
        public void TestResolution()
        {
            var r1 = new Resolution {Id = "XXX", UnitsPerPixel = 1245};
            var r2 = SandD(r1);
            Assert.AreEqual(r1.Id, r2.Id);
            Assert.AreEqual(r1.UnitsPerPixel, r2.UnitsPerPixel);
        }

        [Test]
        public void TestBingSchema()
        {
            string message;
            var s1 = new BingSchema();
            var s2 = SandD(s1);
            var equal = EqualTileSchemas(s1, s2, out message);
            Assert.IsTrue(equal, message);
        }

        [Test]
        public void TestSphericalMercatorWorldSchema()
        {
            string message;
            var s1 = new SphericalMercatorWorldSchema();
            var s2 = SandD(s1);
            var equal = EqualTileSchemas(s1, s2, out message);
            Assert.IsTrue(equal, message);
        }

        [Test]
        public void TestSphericalMercatorInvertedWorldSchema()
        {
            string message;
            var s1 = new SphericalMercatorInvertedWorldSchema();
            var s2 = SandD(s1);
            var equal = EqualTileSchemas(s1, s2, out message);
            Assert.IsTrue(equal, message);
        }

        [Test]
        public void TestGlobalMercatorSchema()
        {
            string message;
            var s1 = new GlobalMercator("png", 2, 11);
            var s2 = SandD(s1);
            var equal = EqualTileSchemas(s1, s2, out message);
            Assert.IsTrue(equal, message);
        }

        [Test]
        public void TestWkstNederlandSchema()
        {
            string message;
            var s1 = new WkstNederlandSchema();
            var s2 = SandD(s1);
            var equal = EqualTileSchemas(s1, s2, out message);
            Assert.IsTrue(equal, message);
        }

        #region Caches

        [Test]
        public void TestMemoryCacheBytes()
        {
            var c1 = new MemoryCache<byte[]>(17, 34);
            var c2 = SandD(c1);

            Assert.NotNull(c2);
#if DEBUG
            Assert.IsTrue(c1.EqualSetup(c2));
#endif

        }

        /*
        [Test]
        public void TestMemoryCacheBitmap()
        {
            var c1 = new BruTile.Cache.MemoryCache<System.Drawing.Bitmap>(17, 34);
            var c2 = SandD(c1);

            Assert.NotNull(c2);
#if DEBUG
            Assert.IsTrue(c1.EqualSetup(c2));
#endif
        }
         */
        [Test]
        public void FileCache()
        {
            var c1 = new FileCache(Path.Combine(Path.GetTempPath(), "_test"), "jpg", new TimeSpan(8, 22, 19, 35));
            var c2 = SandD(c1);
            Assert.NotNull(c2);
#if DEBUG
            Assert.IsTrue(c1.EqualSetup(c2));
#endif
        }

        #endregion

        #region Tile sources

        [Test]
        public void TestOsmTileSource()
        {
            var tsc = OsmTileServerConfig.Create(KnownTileServers.Mapnik, null);
            var ts1 = new OsmTileSource(new OsmRequest(tsc), new FakePersistentCache<byte[]>());
            var ts2 = SandD(ts1);

            Assert.NotNull(ts2);
            string message;
            var equal = EqualTileSources(ts1, ts2, out message);
            Assert.IsTrue(equal, message);
        }

        [Test]
        public void TestOsmTileServerConfig()
        {
            var tsc1 = OsmTileServerConfig.Create(KnownTileServers.Mapnik, string.Empty);
            var tsc2 = SandD(tsc1);
            Assert.NotNull(tsc1);

            Assert.AreEqual(tsc1.UrlFormat, tsc2.UrlFormat, "UrlFormats don't match");
            Assert.AreEqual(tsc1.ServerIdentifier, tsc2.ServerIdentifier, "ServerIdentifiers don't match");
            Assert.AreEqual(tsc1.NumberOfServers, tsc2.NumberOfServers, "Number of servers differ");
            Assert.AreEqual(tsc1.MinResolution, tsc2.MinResolution, "Min resolution levels don't match");
            Assert.AreEqual(tsc1.MaxResolution, tsc2.MaxResolution, "Max resolution levels don't match");
        }

        //[Test]
        //[Ignore("Test a path to a folder on a specific machine")]
        //public void TestMbTiles()
        //{
        //    var p1 = new MbTilesTileSource(@"C:\Users\obe.IVV-AACHEN\Downloads\geography-class.mbtiles");
        //    var p2 = SandD(p1);
        //    Assert.IsNotNull(p2);
        //    Assert.AreEqual(p1.Format, p2.Format, "MbTiles Format not equal");
        //    Assert.AreEqual(p1.Type, p2.Type, "MbTiles Type not equal");
        //    string msg;
        //    Assert.IsTrue(EqualTileSources(p1, p2, out msg), msg);
        //    //Assert.IsTrue(EqualTileSchemas(p1.Schema, p2.Schema, out msg), msg);
        //}

        #endregion

        #region private helper methods

        private static bool EqualTileSources(ITileSource ts1, ITileSource ts2, out string message)
        {
            if (!ReferenceEquals(ts1, ts2))
            {
                if (ts1 == null)
                {
                    message = "The reference tile source is null!";
                    return false;
                }
                if (ts2 == null)
                {
                    message = "One of the tile sources is null, and the other not";
                    return false;
                }

                if (!EqualTileSchemas(ts1.Schema, ts2.Schema, out message))
                    return false;

                if (!EqualTileProviders(ts1.Provider, ts2.Provider, out message))
                    return false;
            }
            
            message = "Tile sources seem to be equal";
            return true;
        }

        private static bool EqualTileProviders(ITileProvider tp1, ITileProvider tp2, out string message)
        {
            if (!ReferenceEquals(tp1, tp2))
            {
                if (tp1 == null)
                {
                    message = "The reference tile provider is null!";
                    return false;
                }
                if (tp2 == null)
                {
                    message = "One of the tile providers is null, and the other not";
                    return false;
                }

                if (tp1.GetType() != tp2.GetType())
                {
                    message = "Tile providers are of different type";
                    return false;
                }

                for (var i = 0; i < 8; i++)
                {
                    TileInfo ti = null;
                    try
                    {
                        ti = RandomTileInfo();
                        var t1 = tp1.GetTile(ti);
                        var t2 = tp2.GetTile(ti);
                        if (!TilesEqual(t1, t2))
                        {
                            message = "Request builders produce different results for same tile request";
                            return false;
                        }
                    }
                    catch(WebException ex)
                    {
                        if (ti == null)
                            Console.WriteLine("No tile info!");
                        else
                            Console.WriteLine("TileInfo: {0}, {1}, {2}\n{3}\n{4}", ti.Index.Level, ti.Index.Col, ti.Index.Row,
                                              ex.Message, ex.Response.ResponseUri);
                    }
                }
            }

            message = "Tile providers appear to be equal";
            return true;
        }

        private static bool TilesEqual(IEnumerable<byte> t1, IList<byte> t2)
        {
            if (ReferenceEquals(t1, t2))
                return true;

            if (t1 == null ^ t2 == null)
                return false;

            if (t1 == null)
                return false;

            return !t1.Where((t, i) => t != t2[i]).Any();
        }

        /*
        private static bool EqualRequestBuilders(IRequest ts1, IRequest ts2, out string message)
        {
            if (!ReferenceEquals(ts1, ts2))
            {
                if (ts1 == null ^ ts2 == null)
                {
                    message = "One of the request builders is null, and the other not";
                    return false;
                }

                if (ts1 == null)
                {
                    message = "Reference request builder is null";
                    return false;
                }

                for (var i = 0; i < 50; i++)
                {
                    var rnd = RandomTileInfo();
                    var uri1 = ts1.GetUri(rnd);
                    var uri2 = ts2.GetUri(rnd);

                    if (uri1 != uri2)
                    {
                        message = "Request builders produce different results for same tile request";
                        return false;
                    }
                }
            }
            message = "Request builder seem to be equal";
            return true;
        }
         */

        private static readonly Random Rnd = new Random();
        private static TileInfo RandomTileInfo()
        {
            var level = Rnd.Next(3, 12);
            var max = 2 ^ level;
            return new TileInfo { Extent = new Extent(), Index = new TileIndex(Rnd.Next(0, max), Rnd.Next(0, max), level) };
        }

        private static bool EqualTileSchemas(ITileSchema ts1, ITileSchema ts2, out string message)
        {
            if (ts1.Name != ts2.Name)
            {
                message = "Names don't match";
                return false;
            }

            if (ts1.Srs != ts2.Srs)
            {
                message = "Srs' don't match";
                return false;
            }

            if (ts1.Format != ts2.Format)
            {
                message = "Format doesn't match";
                return false;
            }

            if (ts1.Extent != ts2.Extent)
            {
                message = "Extents doesn't match";
                return false;
            }

            if (ts1.Height != ts2.Height)
            {
                message = "Heights doesn't match";
                return false;
            }

            if (ts1.Width != ts2.Width)
            {
                message = "Widths doesn't match";
                return false;
            }

            if (ts1.OriginX != ts2.OriginX)
            {
                message = "OriginX' doesn't match";
                return false;
            }

            if (ts1.OriginY != ts2.OriginY)
            {
                message = "OriginYs doesn't match";
                return false;
            }

            if (ts1.Resolutions.Count != ts2.Resolutions.Count)
            {
                message = "Number of resolutions doesn't match";
                return false;
            }

            foreach (var key in ts1.Resolutions.Keys)
            {
                var r1 = ts1.Resolutions[key];
                var r2 = ts2.Resolutions[key];
                if (r1.Id != r2.Id || r1.UnitsPerPixel != r2.UnitsPerPixel)
                {
                    message = string.Format("Resolution doesn't match at index {0}", key);
                    return false;
                }
            }

            message = "Schemas are equal!";
            return true;
        }

        private static T SandD<T>(T obj, IFormatter formatter = null)
        {
            if (formatter == null)
            {
                formatter = new BinaryFormatter();
                formatter.AddBruTileSurrogates();
            }

            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(ms);
            }
        }

        #endregion
    }
}