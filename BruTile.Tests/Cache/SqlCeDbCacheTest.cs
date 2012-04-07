using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using BruTile.Cache;
using NUnit.Framework;

namespace BruTile.Tests
{
    public class SqlCeDbCacheTest : CacheTest<DbCache<SqlCeConnection>>
    {
        private static SqlCeConnection MakeConnection(String datasource)
        {
            using (var engine = new SqlCeEngine(string.Format("Data Source={0};Persist Security Info=false;", datasource)))
            {
                engine.CreateDatabase();
            }

            var cn = new SqlCeConnection(string.Format("Data Source={0};Persist Security Info=false;Max Database Size=1024;", datasource));
            cn.Open();
            SqlCeCommand cmd = cn.CreateCommand();
            cmd.CommandText =
                "CREATE TABLE cache (level integer, row integer, col integer, size integer, [image] image, primary key (level, row, col));";
            cmd.ExecuteNonQuery();
            cn.Close();
            return cn;
        }

        public SqlCeDbCacheTest()
            : base(CleanConnection())
        {
        }

        protected override void InsertTiles()
        {
            var bm = new byte[TileSizeX * TileSizeY * BitsPerPixel];
            var count = 0;

            Cache.Connection.Open();
            var trans = Cache.Connection.BeginTransaction();
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
            trans.Commit(CommitMode.Immediate);
            Cache.Connection.Close();

            Console.WriteLine(string.Format("{0} dummy tiles inserted.", count));
        }

        private static DbCache<SqlCeConnection> CleanConnection()
        {
            if (System.IO.File.Exists("test.sdf"))
                System.IO.File.Delete("test.sdf");
            SqlCeConnection cn = MakeConnection("test.sdf");
            return new DbCache<SqlCeConnection>(cn, (p, c) => c, "main", "cache",
               SqlCeAddTileCommand, null, null);
        }

        private static DbCommand SqlCeAddTileCommand(DbConnection connection,
            DecorateDbObjects qualifier, String schema, String table, char parameterPrefix)
        {
            var cmd = ((SqlCeConnection)connection).CreateCommand();
            cmd.CommandText = String.Format(
                "INSERT INTO {0} VALUES({1}Level, {1}Row, {1}Col, {1}Size, {1}Image);", qualifier(schema, table), parameterPrefix);

            SqlCeParameter par = cmd.CreateParameter();
            par.DbType = DbType.Int32;
            par.ParameterName = "Level";
            cmd.Parameters.Add(par);

            par = cmd.CreateParameter();
            par.DbType = DbType.Int32;
            par.ParameterName = "Row";
            cmd.Parameters.Add(par);

            par = cmd.CreateParameter();
            par.DbType = DbType.Int32;
            par.ParameterName = "Col";
            cmd.Parameters.Add(par);

            par = cmd.CreateParameter();
            par.DbType = DbType.Int32;
            par.ParameterName = "Size";
            cmd.Parameters.Add(par);

            par = cmd.CreateParameter();
            par.SqlDbType = SqlDbType.Image;
            par.ParameterName = "Image";
            cmd.Parameters.Add(par);

            return cmd;
        }

        [Test]
        [Ignore]
        public void Test()
        {
            TestInsertFindRemove();
            Console.WriteLine("Commands in store: {0}", Cache.CommandsInStore);
            Console.WriteLine("Max no. of commands borrowed: {0}", Cache.MaxBorrowed);
        }
    }
}