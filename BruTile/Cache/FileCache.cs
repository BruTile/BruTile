// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Globalization;
using System.IO;

namespace BruTile.Cache
{
    public class FileCache : ITileCache<byte[]>
    {
        #region Fields

        private object _syncRoot = new object();
        private string _directory;
        private string _format;

        #endregion

        #region Public Methods
        /// <remarks>The constructor creates the storage _directory if it does not exist.</remarks>
        public FileCache(string directory, string format)
        {
            this._directory = directory;
            this._format = format;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Add(TileIndex index, byte[] image)
        {
            lock (this._syncRoot)
            {
                if (this.Exists(index))
                {
                    return; // ignore
                }
                string dir = GetDirectoryName(index);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                this.WriteToFile(image, index);
            }
        }

        public byte[] Find(TileIndex index)
        {
            lock (_syncRoot)
            {
                if (!Exists(index)) return null; // to indicate not found
                using (FileStream fileStream = new FileStream(GetFileName(index), FileMode.Open, FileAccess.Read))
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

        private bool Exists(TileIndex index)
        {
          return File.Exists(GetFileName(index));
        }

        #endregion

        #region Private Methods

        private string GetFileName(TileIndex index)
        {
            return String.Format(CultureInfo.InvariantCulture,
              "{0}\\{1}.{2}", GetDirectoryName(index), index.Row, _format);
        }

        private string GetDirectoryName(TileIndex index)
        {
            return String.Format(CultureInfo.InvariantCulture,
              "{0}\\{1}\\{2}", _directory, index.Level, index.Col);
        }

        private void WriteToFile(byte[] image, TileIndex index)
        {
          using (FileStream fileStream = File.Open(GetFileName(index), FileMode.CreateNew))
            {
                fileStream.Write(image, 0, (int)image.Length);
                fileStream.Flush();
                fileStream.Close();
            }
        }

        #endregion
    }
}
