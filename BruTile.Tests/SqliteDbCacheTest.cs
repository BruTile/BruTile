using System;
using BruTile.Cache;
using Community.CsharpSqlite.SQLiteClient;
using NUnit.Framework;

namespace BruTile.Tests
{
    public class SqliteDbCacheTest : CacheTest<DbCache<SqliteConnection>>
    {
        private static SqliteConnection MakeConnection(String datasource)
        {
            //DbCache<SqliteConnection>.ParameterPrefix = '$';
            var cn = new SqliteConnection(string.Format("Data Source=file://{0}", datasource));
            cn.Open();
            var cmd = cn.CreateCommand();
            cmd.CommandText =
                "CREATE TABLE IF NOT EXISTS cache (level integer, row integer, col integer, size integer, image blob, primary key (level, row, col) on conflict replace);";
            cmd.ExecuteNonQuery();
            cn.Close();
            return cn;
        }

        //[TestFixtureSetUp]
        //public void SetUp()
        //{
        //    DbCache<SqliteConnection>.ParameterPrefix = '$';
        //}

        public SqliteDbCacheTest()
            : base(CleanConnection())
        {
        }

        private static DbCache<SqliteConnection> CleanConnection()
        {
            if (System.IO.File.Exists("test.db"))
                System.IO.File.Delete("test.db");
            var cn = MakeConnection("test.db");
            return new DbCache<SqliteConnection>(cn, (p, c) => c, "main", "cache");
        }

        [Test]
        [Ignore]
        public void Test()
        {
            TestInsertFindRemove();
            Console.WriteLine("Commands in store: {0}", Cache.CommandsInStore);
            Console.WriteLine("Max no. of commands borrowed: {0}", Cache.MaxBorrowed);
        }

        protected override void InsertTiles()
        {
            var bm = new byte[TileSizeX * TileSizeY * BitsPerPixel];

            Cache.Connection.Open();
            var trans = Cache.Connection.BeginTransaction();

            var count = 0;
            for (byte level = 0; level < MaxLevel; level++)
            {
                var numberOfTiles = Math.Pow(2, level);
                for (byte i = 0; i < numberOfTiles; i++)
                    for (byte j = 0; j < numberOfTiles; j++)
                    {
                        bm[0] = i;
                        bm[1] = j;
                        bm[2] = level;
                        Cache.Add(new TileIndex(i, j, level), bm);
                        count++;
                    }
            }

            trans.Commit();
            Cache.Connection.Close();

            using (var cn = new SqliteConnection(Cache.Connection.ConnectionString))
            {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandText = "SELECT count(*) FROM cache";
                Assert.AreEqual(count, Convert.ToInt32(cmd.ExecuteScalar()));
            }

            Console.WriteLine(string.Format("{0} dummy tiles inserted.", count));
        }
    }
}