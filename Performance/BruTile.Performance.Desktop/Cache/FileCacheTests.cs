﻿using System;
using System.IO;
using BruTile.Cache;
using BruTile.Performance.Desktop.Utilities;
using NUnit.Framework;

namespace BruTile.Performance.Desktop.Cache
{
    [TestFixture, Category("CacheTest")]
    class FileCacheTests
    {
        private FileCache _cache;
        private string _directory;

        [OneTimeSetUp]
        public void Setup()
        {
            _directory = Path.Combine(Paths.AssemblyDirectory, "FileCacheTest");

            _cache = ClearedFileCacheTest(_directory);
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

        private static FileCache ClearedFileCacheTest(string directory)
        {
            if (Directory.Exists(directory))
            {
                DeleteDirectory(directory);
            }
            return new FileCache(directory, "buf");
        }

        public static void DeleteDirectory(string directory)
        {
            var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                File.Delete(file);
            }
            Directory.Delete(directory, true);
        }
    }
}