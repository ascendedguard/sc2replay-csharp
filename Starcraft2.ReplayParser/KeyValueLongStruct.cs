// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyValueLongStruct.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Represents a key-value pair in the replay format, with the value represented as a long.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents a key-value pair in the replay format.
    /// </summary>
    public struct KeyValueLongStruct
    {
        #region Public Properties

        /// <summary> Gets or sets the key, describing the data presented. </summary>
        public byte[] Key { get; set; }

        /// <summary> Gets or sets the value contained in the struct, represented as a long. </summary>
        public long Value { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses a KeyValueStruct from an expected position in a BinaryReader,
        /// returning the KeyValueStruct and advancing the reader past the object.
        /// </summary>
        /// <param name="reader"> BinaryReader advanced to a point expecting a KeyValueStruct </param>
        /// <returns> The KeyValueStruct found at the current reader position. </returns>
        public static KeyValueLongStruct Parse(BinaryReader reader)
        {
            var kv = new KeyValueLongStruct { Key = reader.ReadBytes(2), Value = ParseLongValueStruct(reader) };

            return kv;
        }

        /// <summary> Parses the long value of a KeyValueLongStruct, with the reader at the current position of the value. </summary>
        /// <param name="reader"> The reader, at the current position of the value of the struct. </param>
        /// <returns> Returns the value of the struct as a long. </returns>
        public static long ParseLongValueStruct(BinaryReader reader)
        {
            long l2 = 0;

            for (int k = 0;; k += 7)
            {
                long l1 = reader.ReadByte();

                l2 |= (l1 & 0x7F) << k;

                if ((l1 & 0x80) == 0)
                {
                    return (l2 & 1L) > 0L ? -(l2 >> 1) : l2 >> 1;
                }
            }
        }

        /// <summary> Overrides the ToString method, showing the Key and Value pairs in an appropriate format. </summary>
        /// <returns> A string containing the Key in Hex format, with the value in decimal. </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("Key { ");

            foreach (byte b in this.Key)
            {
                builder.AppendFormat("{0:X} ", b);
            }

            builder.Append("} - Value { ");

            builder.AppendFormat("{0} ", this.Value);

            builder.Append("}");
            return builder.ToString();
        }

        #endregion
    }
}