#region Information

// Solution:  Spark
// Spark.Engine
// File:  LimitedStream.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:50 PM

#endregion

namespace FhirOnAzure.Engine.Auxiliary
{
    using System;
    using System.IO;

    public class LimitedStream : Stream
    {
        private readonly Stream innerStream;

        /// <summary>
        ///     Creates a write limit on the underlying <paramref name="stream" /> of <paramref name="sizeLimitInBytes" />, which
        ///     has a default of 2048 (2kB).
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="SizeLimitInBytes"></param>
        public LimitedStream(Stream stream, long sizeLimitInBytes = 2048)
        {
            if (stream == null)
                throw new ArgumentNullException("stream cannot be null");

            innerStream = stream;
            SizeLimitInBytes = sizeLimitInBytes;
        }

        public long SizeLimitInBytes { get; } = 2048;

        public override bool CanRead => innerStream.CanRead;

        public override bool CanSeek => innerStream.CanSeek;

        public override bool CanWrite => innerStream.CanWrite && innerStream.Length < SizeLimitInBytes;

        public override long Length => innerStream.Length;

        public override long Position
        {
            get => innerStream.Position;

            set => innerStream.Position = value;
        }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var bytesToBeAdded = Math.Min(buffer.Length - offset, count);
            if (Length + bytesToBeAdded <= SizeLimitInBytes)
                innerStream.Write(buffer, offset, count);
            else
                throw new ArgumentOutOfRangeException("buffer",
                    string.Format("Adding {0} bytes to the stream would exceed the size limit of {1} bytes.",
                        bytesToBeAdded, SizeLimitInBytes));
        }
    }
}