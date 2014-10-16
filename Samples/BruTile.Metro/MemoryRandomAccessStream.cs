using System;
using System.IO;
using Windows.Storage.Streams;

namespace BruTile.Metro
{
    class MemoryRandomAccessStream : IRandomAccessStream
    {
        private readonly Stream _internalStream;

        public MemoryRandomAccessStream(Stream stream)
        {
            _internalStream = stream;
        }

        public MemoryRandomAccessStream(byte[] bytes)
        {
            _internalStream = new MemoryStream(bytes);
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            _internalStream.Position = (long)position;
            return _internalStream.AsInputStream();
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            _internalStream.Position = (long)position;
            return _internalStream.AsOutputStream();
        }

        public ulong Size
        {
            get { return (ulong)_internalStream.Length; }
            set { _internalStream.SetLength((long)value); }
        }

        public bool CanRead
        {
            get { return true; }
        }

        public bool CanWrite
        {
            get { return true; }
        }

        public IRandomAccessStream CloneStream()
        {
            throw new NotSupportedException();
        }

        public ulong Position
        {
            get { return (ulong)_internalStream.Position; }
        }

        public void Seek(ulong position)
        {
            _internalStream.Seek((long)position, 0);
        }

        public void Dispose()
        {
            _internalStream.Dispose();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            var inputStream = GetInputStreamAt(0);
            return inputStream.ReadAsync(buffer, count, options);
        }

        public Windows.Foundation.IAsyncOperation<bool>

            FlushAsync()
        {
            var outputStream = GetOutputStreamAt(0);
            return outputStream.FlushAsync();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<uint, uint>

            WriteAsync(IBuffer buffer)
        {
            var outputStream = GetOutputStreamAt(0);
            return outputStream.WriteAsync(buffer);
        }
    }
}
