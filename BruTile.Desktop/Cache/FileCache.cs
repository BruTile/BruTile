// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;

namespace BruTile.Cache
{
    [Serializable]
    public class FileCache : IPersistentCache<byte[]>
    {
        private readonly object _syncRoot = new object();
        private readonly string _directory;
        private readonly string _format;
        private readonly TimeSpan _cacheExpireTime = TimeSpan.Zero;

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
            lock (_syncRoot)
            {
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
        }

        public byte[] Find(TileIndex index)
        {
            lock (_syncRoot)
            {
                if (!Exists(index)) return null; // to indicate not found
                using (var fileStream = new FileStream(GetFileName(index), FileMode.Open, FileAccess.Read))
                {
                    return Utilities.ReadFully(fileStream);
                }
            }
        }

        public void Remove(TileIndex index)
        {
            lock (_syncRoot)
            {
                if (Exists(index))
                {
                    File.Delete(GetFileName(index));
                }
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

#if !NET35

        public string GetFileName(TileIndex index)
        {
            return Path.Combine(GetDirectoryName(index), 
                string.Format(CultureInfo.InvariantCulture, "{0}.{1}", index.Row, _format));
        }

        private string GetDirectoryName(TileIndex index)
        {
            return Path.Combine(_directory, 
                index.Level.ToString(CultureInfo.InvariantCulture), 
                index.Col.ToString(CultureInfo.InvariantCulture));
        }

#else
        public string GetFileName(TileIndex index)
        {
            return string.Format(CultureInfo.InvariantCulture,
                                 "{0}\\{1}.{2}", GetDirectoryName(index), index.Row, _format);
        }
        
        private string GetDirectoryName(TileIndex index)
        {
            return string.Format(CultureInfo.InvariantCulture,
                                 "{0}\\{1}\\{2}", _directory, index.Level, index.Col);
        }
#endif

        private void WriteToFile(byte[] image, TileIndex index)
        {
            using (FileStream fileStream = File.Open(GetFileName(index), FileMode.Create))
            {
                fileStream.Write(image, 0, image.Length);
                fileStream.Flush();
                fileStream.Close();
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

            System.Diagnostics.Debug.Assert(_syncRoot != null && other._syncRoot != null && _syncRoot != other._syncRoot);

            return true;
        }
#endif
    }
}