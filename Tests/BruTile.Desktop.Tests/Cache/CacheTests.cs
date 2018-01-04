using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;
using NUnit.Framework;

namespace BruTile.Tests.Cache
{
    //[TestFixture, Category("CacheTest")] // Perhaps think we should replace the SQLiteCache with MbTilesTileSource
    public abstract class CacheTests<TCache>
        where TCache : ITileCache<byte[]>
    {
        protected const int TileSizeX = 256;
        protected const int TileSizeY = 256;
        protected const int BitsPerPixel = 8;
        protected const int MaxLevel = 4;

        protected readonly TCache Cache;

        protected CacheTests(TCache cache)
        {
            Cache = cache;
        }

        public void TestInsertFindRemove()
        {
            InsertTiles();
            FindTile();
            FindTiles();
            RemoveTile();
        }

        protected virtual void InsertTiles()
        {
            var bm = new byte[TileSizeX * TileSizeY * BitsPerPixel];
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
            Console.WriteLine("{0} dummy tiles inserted.", count);
        }

        public void FindTile()
        {
            var sw = new Stopwatch();

            var tk = new TileIndex(1, 2, "2");
            byte[] bm = Cache.Find(tk);
            sw.Start();
            bm = Cache.Find(tk);
            sw.Stop();
            Assert.IsNotNull(bm);
            Assert.AreEqual(TileSizeX * TileSizeY * BitsPerPixel, bm.Length);
            Assert.AreEqual(1, Convert.ToInt32(bm[0]));
            Assert.AreEqual(2, Convert.ToInt32(bm[1]));
            Assert.AreEqual(2, Convert.ToInt32(bm[2]));

            Console.WriteLine($"Specific Tile ({tk.Level},{tk.Row},{tk.Col}) found in {sw.ElapsedMilliseconds}ms.");

            sw.Reset();
            tk = new TileIndex(5, 5, (MaxLevel - 1).ToString(CultureInfo.InvariantCulture));
            sw.Start();
            bm = Cache.Find(tk);
            sw.Stop();
            Assert.IsNotNull(bm);
            Assert.AreEqual(TileSizeX * TileSizeY * BitsPerPixel, bm.Length);
            Assert.AreEqual(5, Convert.ToInt32(bm[0]));
            Assert.AreEqual(5, Convert.ToInt32(bm[1]));
            Assert.AreEqual(MaxLevel - 1, Convert.ToInt32(bm[2]));

            Console.WriteLine($"Specific Tile ({tk.Level},{tk.Row},{tk.Col}) found in {sw.ElapsedMilliseconds}ms.");
        }

        private const int NumberToSearch = 20;
        private const int WaitMilliseconds = 0;

        public void FindTiles()
        {
            var waitHandle = new AutoResetEvent(false);
            var sw = new Stopwatch();
            var count = 0;
            foreach (var ti in GetRandomTileIndices(NumberToSearch))
            {
                var task = new Task(FindTileOnTread, new object[] {ti, waitHandle, ++count});
                task.Start();
            }

            waitHandle.WaitOne();
            sw.Stop();
            Console.WriteLine("{0} Tiles found in {1}ms (Penalty: {2}ms).", NumberToSearch, sw.ElapsedMilliseconds, WaitMilliseconds);
        }

        private static readonly Random Random = new Random(93765783);

        private static IEnumerable<TileIndex> GetRandomTileIndices(int numberOfTileInfos)
        {
            for (var i = 0; i < numberOfTileInfos; i++)
            {
                var level = Random.Next(MaxLevel);
                var maxValue = (int)Math.Pow(2, level);
                yield return new TileIndex(Random.Next(maxValue), Random.Next(maxValue), level.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void FindTileOnTread(object arg)
        {
            var args = (object[])arg;
            var tileIndex = (TileIndex)args[0];
            var resetEvent = (AutoResetEvent)args[1];
            var count = (int) args[2];

            var sw = new Stopwatch();
            sw.Start();
            var buffer = Cache.Find(tileIndex);
            if (buffer == null)
            {
                buffer = Cache.Find(tileIndex);
                if (buffer == null) Assert.IsTrue(false);

            }
            sw.Stop();

            Assert.AreEqual(buffer[0], tileIndex.Col);
            Assert.AreEqual(buffer[1], tileIndex.Row);
            Console.WriteLine("Found Tile({0}, {1}, {2}) in {3}ms", tileIndex.Level, tileIndex.Row, tileIndex.Col, sw.ElapsedMilliseconds);

            if (count == NumberToSearch)
            {
                Thread.Sleep(500);
                resetEvent.Set();
            }
        }

        public void RemoveTile()
        {
            var tk = new TileIndex(1, 2, "0");
            Cache.Remove(tk);

            byte[] bm = Cache.Find(tk);
            Assert.IsNull(bm);
        }
    }
}