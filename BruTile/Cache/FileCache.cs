// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BruTile.Cache;

public class FileCache : IPersistentCache<byte[]>
{
    private readonly ReaderWriterLockSlim _rwLock = new();
    private readonly string _directory;
    private readonly string _format;
    private readonly TimeSpan _cacheExpireTime;

    /// <param name="directory">The location of the file cache.</param>
    /// <param name="format">The format of the files stored in the file cache (e.g. 'png').</param>
    /// <param name="cacheExpireTime">At what time the cache is considered expired.</param>
    /// <remarks>
    ///   The constructor creates the storage _directory if it does not exist.
    /// </remarks>

    public FileCache(string directory, string format, TimeSpan cacheExpireTime)
    {
        _directory = directory;
        _format = format;
        _cacheExpireTime = cacheExpireTime;

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <param name="directory">The location of the file cache.</param>
    /// <param name="format">The format of the files stored in the file cache (e.g. 'png').</param>
    /// <remarks>
    /// The constructor creates the storage _directory if it does not exist.
    /// </remarks>
    public FileCache(string directory, string format)
        : this(directory, format, TimeSpan.Zero)
    { }

    public void Add(TileIndex index, byte[] image)
    {
        try
        {
            _rwLock.EnterWriteLock();
            if (Exists(index))
            {
                return; // Ignore
            }
            var dir = GetDirectoryName(index);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            WriteToFile(image, index);
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public byte[] Find(TileIndex index)
    {
        try
        {
            _rwLock.EnterReadLock();
            if (!Exists(index)) return null; // To indicate not found
            return File.ReadAllBytes(GetFileName(index));
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    public void Remove(TileIndex index)
    {
        try
        {
            _rwLock.EnterWriteLock();
            if (Exists(index))
            {
                File.Delete(GetFileName(index));
            }
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public bool Exists(TileIndex index)
    {
        if (File.Exists(GetFileName(index)))
        {
            return _cacheExpireTime == TimeSpan.Zero || (DateTime.Now - new FileInfo(GetFileName(index)).LastWriteTime) <= _cacheExpireTime;
        }
        return false;
    }

    public string GetFileName(TileIndex index)
    {
        return Path.Combine(GetDirectoryName(index),
            string.Format(CultureInfo.InvariantCulture, "{0}.{1}", index.Row, _format));
    }

    private string GetDirectoryName(TileIndex index)
    {
        var level = index.Level.ToString(CultureInfo.InvariantCulture);
        level = level.Replace(':', '_');
        return Path.Combine(_directory,
            level,
            index.Col.ToString(CultureInfo.InvariantCulture));
    }

    private void WriteToFile(byte[] image, TileIndex index)
    {
        using var fileStream = File.Open(GetFileName(index), FileMode.Create);
        fileStream.Write(image, 0, image.Length);
        fileStream.Flush();
    }
}
