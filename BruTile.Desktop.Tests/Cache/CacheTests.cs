using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using BruTile.Cache;
using NUnit.Framework;

namespace BruTile.Tests.Cache
{
    [TestFixture, Category("CacheTest")]
    public abstract class CacheTests<TCache>
        where TCache : ITileCache<byte[]>
    {
        protected const int TileSizeX = 256;
        protected const int TileSizeY = 256;
        protected const int BitsPerPixel = 8;
        protected const int MaxLevel = 5;

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
                        Cache.Add(new TileIndex(i, j, level), bm);
                        count++;
                    }
            }
            Console.WriteLine(string.Format("{0} dummy tiles inserted.", count));
        }

        public void FindTile()
        {
            var sw = new Stopwatch();

            var tk = new TileIndex(1, 2, 2);
            byte[] bm = Cache.Find(tk);
            sw.Start();
            bm = Cache.Find(tk);
            sw.Stop();
            Assert.IsNotNull(bm);
            Assert.AreEqual(TileSizeX * TileSizeY * BitsPerPixel, bm.Length);
            Assert.AreEqual(1, Convert.ToInt32(bm[0]));
            Assert.AreEqual(2, Convert.ToInt32(bm[1]));
            Assert.AreEqual(2, Convert.ToInt32(bm[2]));

            Console.WriteLine(string.Format("Specific Tile ({0},{1},{2}) found in {3}ms.", tk.Level, tk.Row, tk.Col, sw.ElapsedMilliseconds));

            sw.Reset();
            tk = new TileIndex(5, 5, MaxLevel - 1);
            sw.Start();
            bm = Cache.Find(tk);
            sw.Stop();
            Assert.IsNotNull(bm);
            Assert.AreEqual(TileSizeX * TileSizeY * BitsPerPixel, bm.Length);
            Assert.AreEqual(5, Convert.ToInt32(bm[0]));
            Assert.AreEqual(5, Convert.ToInt32(bm[1]));
            Assert.AreEqual(MaxLevel - 1, Convert.ToInt32(bm[2]));

            Console.WriteLine(string.Format("Specific Tile ({0},{1},{2}) found in {3}ms.", tk.Level, tk.Row, tk.Col, sw.ElapsedMilliseconds));
        }

        private const int NumberToSearch = 10000;
        private const int WaitMilliseconds = 0;

        public void FindTiles()
        {
            var sw = new Stopwatch();
            var waitHandle = new AutoResetEvent(false);
            sw.Start();
            var count = 0;
            foreach (var ti in GetRandomTileIndices(NumberToSearch))
            {
                ThreadPool.QueueUserWorkItem(FindTileOnTread, new object[] { ti, waitHandle, ++count });
            }
            waitHandle.WaitOne();
            sw.Stop();
            Console.WriteLine(string.Format("{0} Tiles found in {1}ms (Penalty: {2}ms).", NumberToSearch, sw.ElapsedMilliseconds, WaitMilliseconds));
        }

        private static readonly Random _random = new Random(93765783);

        private static IEnumerable<TileIndex> GetRandomTileIndices(int numberOfTileInfos)
        {
            for (int i = 0; i < numberOfTileInfos; i++)
            {
                int level = _random.Next(MaxLevel);
                var maxValue = (int)Math.Pow(2, level);
                yield return new TileIndex(_random.Next(maxValue), _random.Next(maxValue), level);
            }
        }

        public void FindTileOnTread(object arg)
        {
            var args = (object[])arg;
            var tileIndex = (TileIndex)args[0];
            var resetEvent = (AutoResetEvent)args[1];
            var count = (int) args[2];

            //Let this take some time
            if (WaitMilliseconds > 0)
                Thread.Sleep(WaitMilliseconds);
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
            var tk = new TileIndex(1, 2, 0);
            Cache.Remove(tk);

            byte[] bm = Cache.Find(tk);
            Assert.IsNull(bm);
        }
    }
}