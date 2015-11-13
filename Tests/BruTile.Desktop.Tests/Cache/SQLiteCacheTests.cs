using System;
using System.Data.SQLite;
using System.Globalization;
using BruTile.Cache;
using NUnit.Framework;

namespace BruTile.Tests.Cache
{
    public class SQLiteDbCacheTests : CacheTests<DbCache<SQLiteConnection>>
    {
        private static SQLiteConnection MakeConnection(String datasource)
        {
            var cn = new SQLiteConnection(string.Format("Data Source={0}", datasource));
            cn.Open();
            SQLiteCommand cmd = cn.CreateCommand();
            cmd.CommandText =
                "CREATE TABLE IF NOT EXISTS cache (level integer, row integer, col integer, size integer, image blob, primary key (level, row, col) on conflict replace);";
            cmd.ExecuteNonQuery();
            cn.Close();
            return cn;
        }

        public SQLiteDbCacheTests()
            : base(CleanConnection())
        {
        }

        private static DbCache<SQLiteConnection> CleanConnection()
        {
            if (System.IO.File.Exists("test.db"))
                System.IO.File.Delete("test.db");
            SQLiteConnection cn = MakeConnection("test.db");
            return new DbCache<SQLiteConnection>(cn, (p, c) => c, "main", "cache");
        }

        [Test]
        public void InsertFindRemoveTest()
        {
            TestInsertFindRemove();
#if DEBUG
            Console.WriteLine("Commands in store: {0}", Cache.CommandsInStore);
            Console.WriteLine("Max no. of commands borrowed: {0}", Cache.MaxBorrowed);
#endif
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
                        Cache.Add(new TileIndex(i, j, level.ToString(CultureInfo.InvariantCulture)), bm);
                        count++;
                    }
            }

            trans.Commit();
            Cache.Connection.Close();

            using (var cn = (SQLiteConnection)Cache.Connection.Clone())
            {
                cn.Open();
                SQLiteCommand cmd = cn.CreateCommand();
                cmd.CommandText = "SELECT count(*) FROM cache";
                Assert.AreEqual(count, Convert.ToInt32(cmd.ExecuteScalar()));
            }
            Console.WriteLine(string.Format("{0} dummy tiles inserted.", count));
        }
    }
}
