using System;
using System.IO;
using System.Text;

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// Represents a key-value pair in the replay format.
    /// </summary>
    public struct KeyValueStruct
    {
        public byte[] Key { get; set; }
        public short Value { get; set; }

        /// <summary>
        /// Overrides the ToString method, showing the Key and Value pairs in an appropriate format.
        /// </summary>
        /// <returns>A string containing the Key in Hex format, with the value in decimal.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("Key { ");
            
            foreach(byte b in Key)
            {
                builder.AppendFormat("{0:X} ", b);                
            }

            builder.Append("} - Value { ");

            builder.AppendFormat("{0} ", Value);

            builder.Append("}");
            return builder.ToString();
        }

        /// <summary>
        /// Parses a KeyValueStruct from an expected position in a BinaryReader,
        /// returning the KeyValueStruct and advancing the reader past the object.
        /// </summary>
        /// <param name="reader">BinaryReader advanced to a point expecting a KeyValueStruct</param>
        /// <returns>The KeyValueStruct found at the current reader position.</returns>
        public static KeyValueStruct Parse(BinaryReader reader)
        {
            var kv = new KeyValueStruct { Key = reader.ReadBytes(2) };

            byte firstByte = reader.ReadByte();

            if ((firstByte & 0x80) > 0)
            {
                firstByte <<= 1; // Knock off the insignificant bit
                byte secondByte = reader.ReadByte();

                short value = BitConverter.ToInt16(new[] { firstByte, secondByte }, 0);

                value >>= 1; // Shift to the right to accomodate for the signal bit to get the true value.

                kv.Value = (short)(value / 2);
            }
            else
            {
                kv.Value = (short)(firstByte / 2);
            }

            return kv;
        }
    }
}
