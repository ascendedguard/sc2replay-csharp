// -----------------------------------------------------------------------
// <copyright file="BitReader.cs" company="SC2ReplayParser">
//     2012 SC2ReplayParser
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser.Streams
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// A basic little-endian bitstream reader.
    /// </summary>
    public class BitReader
    {
        Stream stream;
        int bitCursor;
        int currentByte;

        public BitReader(Stream stream)
        {
            this.stream = stream;
            this.bitCursor = 0;
        }

        /// <summary>
        /// Reads up to 32 bits from the stream
        /// </summary>
        /// <returns>The value read</returns>
        public uint Read(int numBits)
        {
            uint value = 0;

            while (numBits > 0)
            {
                var bytePos = bitCursor & 7;
                int bitsLeftInByte = 8 - bytePos;
                if (bytePos == 0)
                {
                    currentByte = stream.ReadByte();
                }
                var bitsToRead = (bitsLeftInByte > numBits) ? numBits : bitsLeftInByte;

                value = (value << bitsToRead) | ((uint)currentByte >> bytePos) & ((1u << bitsToRead) - 1u);
                bitCursor += bitsToRead;
                numBits -= bitsToRead;
            }

            return value;
        }

        public void AlignToByte()
        {
            if ((bitCursor & 7) > 0) bitCursor = (bitCursor & 0x7ffffff8) + 8;
        }

        /// <summary>
        /// Reads up to 32 bits from the stream
        /// </summary>
        /// <param name="numBits"></param>
        /// <returns></returns>
        public uint Read(uint numBits)
        {
            return Read((int)numBits);
        }

        /// <summary>
        /// Returns true if end of stream has been reached.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return (bitCursor >> 3) == stream.Length;
            }
        }

        /// <summary>
        /// Returns the current cursor position.
        /// </summary>
        public int Cursor
        {
            get { return bitCursor; }
        }
    }
}
