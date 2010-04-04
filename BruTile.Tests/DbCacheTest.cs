using System;
using System.Collections.Generic;
using BruTile;
using NUnit.Framework;
using System.Data.SQLite;
using BruTile.Cache;

namespace SharpMap.Layers.Tests
{
    [TestFixture]
    public class DbCacheTest
    {
        private DbCache<SQLiteConnection> _cache;

        static SQLiteConnection MakeConnection(String datasource)
        {
            SQLiteConnection cn = new SQLiteConnection(string.Format("Data Source={0}", datasource));
            cn.Open();
            SQLiteCommand cmd = cn.CreateCommand();
            cmd.CommandText =
                "CREATE TABLE IF NOT EXISTS cache (level integer, row integer, col integer, size integer, image blob, primary key (level, row, col) on conflict replace);";
            cmd.ExecuteNonQuery();
            cn.Close();
            return cn;
        }

        public DbCacheTest()
        {
            if (System.IO.File.Exists("test.db"))
                System.IO.File.Delete("test.db");
            SQLiteConnection cn = MakeConnection("test.db");

            //_cache = new DbCache<SQLiteConnection>(cn, delegate(string p, string c) { return c; }, "main", "cache");
            _cache = new DbCache<SQLiteConnection>(cn, (p, c) => c, "main", "cache");
        }

        [Test]
        public void TestInsertFindRemove()
        {
            InsertTiles();
            FindTile();
            RemoveTile();
        }
        
        public void InsertTiles()
        {
            byte[] bm = new byte[128*128*1];

            _cache.Connection.Open();
            bm[0] = 0;
            bm[1] = 0;
            bm[2] = 0;
            _cache.Add(new TileIndex(0, 0, 0), bm);
            bm[0] = 0;
            bm[1] = 1;
            bm[2] = 0;
            _cache.Add(new TileIndex(0, 1, 0), bm);
            bm[0] = 0;
            bm[1] = 2;
            bm[2] = 0;
            _cache.Add(new TileIndex(0, 2, 0), bm);
            bm[0] = 1;
            bm[1] = 0;
            bm[2] = 0;
            _cache.Add(new TileIndex(1, 0, 0), bm);
            bm[0] = 1;
            bm[1] = 1;
            bm[2] = 0;
            _cache.Add(new TileIndex(1, 1, 0), bm);
            bm[0] = 1;
            bm[1] = 2;
            bm[2] = 0;
            _cache.Add(new TileIndex(1, 2, 0), bm);
            bm[0] = 2;
            bm[1] = 0;
            bm[2] = 0;
            _cache.Add(new TileIndex(2, 0, 0), bm);
            bm[0] = 2;
            bm[1] = 1;
            bm[2] = 0;
            _cache.Add(new TileIndex(2, 1, 0), bm);
            bm[0] = 2;
            bm[1] = 2;
            bm[2] = 0;
            _cache.Add(new TileIndex(2, 2, 0), bm);

            _cache.Connection.Close();

            using (SQLiteConnection cn = (SQLiteConnection)_cache.Connection.Clone())
            {
                cn.Open();
                SQLiteCommand cmd = cn.CreateCommand();
                cmd.CommandText = "SELECT count(*) FROM cache";
                Assert.AreEqual(9, Convert.ToInt32(cmd.ExecuteScalar()));
            }
        }

        public void FindTile()
        {
            TileIndex tk = new TileIndex(1,2,0);
            byte[] bm = _cache.Find(tk);
            Assert.IsNotNull(bm);
            Assert.AreEqual(128*128*1, bm.Length);
            Assert.AreEqual(1, Convert.ToInt32(bm[0]));
            Assert.AreEqual(2, Convert.ToInt32(bm[1]));
            Assert.AreEqual(0, Convert.ToInt32(bm[2]));
        }

        public void RemoveTile()
        {
            TileIndex tk = new TileIndex(1, 2, 0);
            _cache.Remove(tk);

            byte[] bm = _cache.Find(tk);
            Assert.IsNull(bm);
        }

        //[Test]
        //public void EsriTest()
        //{
        //    BruTileDataSourceEsri dse = new BruTileDataSourceEsri(
        //        new DbCache<SQLiteConnection>(MakeConnection("esri.sqlite"), (p, c) => c, "main", "cache"));

        //    IList<TileInfo> infos = Tile.GetTiles(dse.TileSchema, new Extent(7, 48, 9, 55), 6);
        //    foreach (TileInfo tileInfo in infos)
        //        dse.GetTile(tileInfo);

        //    infos = Tile.GetTiles(dse.TileSchema, new Extent(7, 48, 9, 55), 7);
        //    foreach (TileInfo tileInfo in infos)
        //        dse.GetTile(tileInfo);

        //    infos = Tile.GetTiles(dse.TileSchema, new Extent(7, 48, 9, 55), 8);
        //    foreach (TileInfo tileInfo in infos)
        //        dse.GetTile(tileInfo);
        //}

        //[Test]
        //public void BingTest()
        //{
        //    BruTileDataSource dse = BruTileDataSource.Create(BruTileDataSources.Bing, 
        //        new DbCache<SQLiteConnection>(MakeConnection("bing.sqlite"), (p, c) => c, "main", "cache"));

        //    Double eWidth = dse.TileSchema.Extent.MaxX - dse.TileSchema.Extent.MinX;
        //    Double eHeight = dse.TileSchema.Extent.MaxY - dse.TileSchema.Extent.MinY;
        //    Double fX = Math.Abs(eWidth/360d);
        //    Double fY = Math.Abs(eHeight/180d);
        //    Double left = dse.TileSchema.Extent.CenterX + 7*fX;
        //    Double bottom = dse.TileSchema.Extent.CenterY + 48*fY;
        //    Double right = dse.TileSchema.Extent.CenterX + 9 * fX;
        //    Double top = dse.TileSchema.Extent.CenterY + 55 * fY;

        //    Extent ex = new Extent(left, bottom, right, top);
        //    IList<TileInfo> infos = Tile.GetTiles(dse.TileSchema, ex, 6);
        //    foreach (TileInfo tileInfo in infos)
        //        dse.GetTile(tileInfo);

        //    infos = Tile.GetTiles(dse.TileSchema, ex, 7);
        //    foreach (TileInfo tileInfo in infos)
        //        dse.GetTile(tileInfo);

        //    infos = Tile.GetTiles(dse.TileSchema, ex, 8);
        //    foreach (TileInfo tileInfo in infos)
        //        dse.GetTile(tileInfo);
        //}

    }
}