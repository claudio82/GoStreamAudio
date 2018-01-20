using System;
using System.IO;

namespace GoStreamAudioLib
{
    public class ReadFullyStream : Stream
    {
        // private fields        
        private readonly Stream sourceStream;           // the stream's source
        private long pos;                               // pseudo-position in the stream
        private readonly byte[] readAheadBuffer;        // the reading ahead's buffer
        private int readAheadLength;                    // number of bytes for ahead's reading buffer
        private int readAheadOffset;                    // reading offset in the ahead's buffer

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="sourceStream">the stream's source</param>
        public ReadFullyStream(Stream sourceStream)
        {
            this.sourceStream = sourceStream;
            readAheadBuffer = new byte[4096];
        }

        /// <summary>
        /// check if the stream can be read
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// check if the stream can be seeked
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// check if the stream can be written
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// flushes the stream (not supported)
        /// </summary>
        public override void Flush()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// the stream length
        /// </summary>
        public override long Length
        {
            get { return pos; }
        }

        /// <summary>
        /// the position in the stream (it cannot be set)
        /// </summary>
        public override long Position
        {
            get
            {
                return pos;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// read some bytes into given buffer
        /// </summary>
        /// <param name="buffer">the input buffer</param>
        /// <param name="offset">the starting position</param>
        /// <param name="count">the number of bytes to read</param>
        /// <returns>the effective number of bytes read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            while (bytesRead < count)
            {
                int readAheadAvailableBytes = readAheadLength - readAheadOffset;
                int bytesRequired = count - bytesRead;
                if (readAheadAvailableBytes > 0)
                {
                    //there is available space in ahead's buffer, so copy the smallest number of bytes from 
                    // ahead's buffer into this buffer
                    int toCopy = Math.Min(readAheadAvailableBytes, bytesRequired);
                    Array.Copy(readAheadBuffer, readAheadOffset, buffer, offset + bytesRead, toCopy);
                    bytesRead += toCopy;
                    readAheadOffset += toCopy;
                }
                else 
                {
                    //ahead's buffer is full
                    readAheadOffset = 0;
                    readAheadLength = sourceStream.Read(readAheadBuffer, 0, readAheadBuffer.Length);
                    //if ahead buffer is zero length, stop reading
                    if (readAheadLength == 0)
                    {
                        break;
                    }
                }
            }
            pos += bytesRead;
            return bytesRead;
        }

        /// <summary>
        /// operation is not supported
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// operation is not supported
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// operation is not supported
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }
    }
}
