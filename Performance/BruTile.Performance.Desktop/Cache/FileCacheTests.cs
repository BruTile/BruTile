using System;
using System.IO;
using BruTile.Cache;
using NUnit.Framework;

namespace BruTile.PerformanceTests.Cache
{
    [TestFixture, Category("CacheTest")]
    class FileCacheTests
    {
        private FileCache _cache;

        [OneTimeSetUp]
        public void Setup()
        {
            _cache = ClearedFileCacheTest();
        }

        [Test]
        public void LoadTilesAndFindTiles()
        {
            var threadCount = 500;
            var loadWork = new WorkTimer(threadCount);
            var findWork = new WorkTimer(threadCount);
            loadWork.TimeWork(
                i => new
                {
                    TileIndex = new TileIndex(i % 10, i / 10, "1"),
                    Image = new [] { (byte)i }
                },
                args => _cache.Add(args.TileIndex, args.Image));
            loadWork.WaitForTestsToComplete();
            findWork.TimeWork(i => new TileIndex(i % 10, i / 10, "1"), index => _cache.Find(index));
            findWork.WaitForTestsToComplete();
            Console.WriteLine("Total FileCache.Add time is {0}ms", loadWork.TotalTime);
            Console.WriteLine("Average FileCache.Add time is {0}ms", loadWork.TotalTime / threadCount);
            Console.WriteLine("Max FileCache.Add time is {0}ms", loadWork.MaxTime);
            Console.WriteLine("Min FileCache.Add time is {0}ms", loadWork.MinTime);
            Console.WriteLine("Total FileCache.Find time is {0}ms", findWork.TotalTime);
            Console.WriteLine("Average FileCache.Find time is {0}ms", findWork.TotalTime / threadCount);
            Console.WriteLine("Max FileCache.Find time is {0}ms", findWork.MaxTime);
            Console.WriteLine("Min FileCache.Find time is {0}ms", findWork.MinTime);
        }

        private static FileCache ClearedFileCacheTest()
        {
            if (Directory.Exists("FileCacheTest"))
            {
                DeleteDirectory("FileCacheTest");
            }
            return new FileCache("FileCacheTest", "buf");
        }

        /// <summary>
        /// Depth-first recursive delete, with handling for descendant 
        /// directories open in Windows Explorer.
        /// </summary>
        public static void DeleteDirectory(string path)
        {
            // gettting it wrong with Ryan S: https://stackoverflow.com/a/1703799/85325
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
    }
}