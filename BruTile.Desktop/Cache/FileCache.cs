// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
#if HAS_SERIALIZABLE_ATTRIBUTE
using System.Reflection;
using System.Runtime.Serialization;
#endif
using System.Threading;

namespace BruTile.Cache
{
#if HAS_SERIALIZABLE_ATTRIBUTE
    [Serializable]
#endif
    public class FileCache : IPersistentCache<byte[]>
    {
#if HAS_SERIALIZABLE_ATTRIBUTE
        [NonSerialized]
#endif
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly string _directory;
        private readonly string _format;
        private readonly TimeSpan _cacheExpireTime;

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

        /// <remarks>
        /// The constructor creates the storage _directory if it does not exist.
        /// </remarks>
        public FileCache(string directory, string format)
            : this(directory, format, TimeSpan.Zero)
        {
        }

        public void Add(TileIndex index, byte[] image)
        {
            try
            {
                _rwLock.EnterWriteLock();
                if (Exists(index))
                {
                    return; // ignore
                }
                string dir = GetDirectoryName(index);
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
                if (!Exists(index)) return null; // to indicate not found
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
            using (var fileStream = File.Open(GetFileName(index), FileMode.Create))
            {
                fileStream.Write(image, 0, image.Length);
                fileStream.Flush();
                //fileStream.Close();
            }
        }

#if DEBUG
        public bool EqualSetup(FileCache other)
        {
            if (!string.Equals(_directory, other._directory))
                return false;

            if (!string.Equals(_format, other._format))
                return false;

            if (!_cacheExpireTime.Equals(other._cacheExpireTime))
                return false;

            System.Diagnostics.Debug.Assert(_rwLock != null && other._rwLock != null && _rwLock != other._rwLock);

            return true;
        }
#endif

#if HAS_SERIALIZABLE_ATTRIBUTE
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            var fi = GetType().GetField("_rwLock", BindingFlags.Instance | BindingFlags.NonPublic);
            System.Diagnostics.Debug.Assert(fi != null);
            fi.SetValue(this, new ReaderWriterLockSlim());
        }
#endif
    }
}