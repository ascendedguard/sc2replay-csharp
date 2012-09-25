// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Timestamp.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Parses the timestamp format used in Starcraft 2 Replay files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System;
    using System.IO;

    /// <summary>
    /// Parses the timestamp format used in Starcraft 2 Replay files.
    /// </summary>
    public class Timestamp : IComparable
    {
        #region Public Properties

        /// <summary> Gets a TimeSpan conversion of the Timestamp value. </summary>
        public TimeSpan TimeSpan
        {
            get
            {
                // 62.5 = 1000 / 16
                var milliseconds = (int)(this.Value * 62.5);
                return new TimeSpan(0, 0, 0, 0, milliseconds);
            }
        }

        /// <summary> Gets the integer value of the Timestamp. </summary>
        public int Value { get; private set; }

        #endregion

        #region Public Methods

        /// <summary> Creates a Timestamp object from an integer value. </summary>
        /// <param name="value"> The integer value. </param>
        /// <returns> Returns a Timestamp object with the given value. </returns>
        public static Timestamp Create(int value)
        {
            var timestamp = new Timestamp { Value = value };
            return timestamp;
        }

        /// <summary> Converts an integer timestamp value into a byte array, in the format used for replay files. </summary>
        /// <param name="value"> The integer timestamp to convert. </param>
        /// <returns> Returns a byte array containing the timestamp in it's replay format. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the value given is greater than 2^30. </exception>
        public static byte[] FromInt32(int value)
        {
            int bytesNeeded = 0;

            if (value > Math.Pow(2, 30))
            {
                throw new ArgumentOutOfRangeException("value", "Timestamp value cannot be greater than 2^30");
            }
            else if (value > Math.Pow(2, 22))
            {
                bytesNeeded = 3;
            }
            else if (value > Math.Pow(2, 14))
            {
                bytesNeeded = 2;
            }
            else if (value > Math.Pow(2, 6))
            {
                bytesNeeded = 1;
            }

            var bytes = BitConverter.GetBytes(value);
            bytes[bytesNeeded] = (byte)((bytes[bytesNeeded] << 2) | bytesNeeded);

            var final = new byte[bytesNeeded + 1];

            int index = bytesNeeded;
            for (int i = 0; i < bytesNeeded + 1; i++)
            {
                final[i] = bytes[index];
                index--;
            }

            return final;
        }

        /// <summary> Parses a Timestamp object from a BinaryReader and advances it's position. </summary>
        /// <param name="reader"> The binary reader, at the position of the Timestamp object. </param>
        /// <returns> Returns the parsed Timestamp. </returns>
        public static Timestamp Parse(BinaryReader reader)
        {
            int value = ParseTimestamp(reader);

            var timestamp = new Timestamp { Value = value };
            return timestamp;
        }

        /// <summary> Compares two Timestamp objects to determine which has a greater value. </summary>
        /// <param name="obj"> A timestamp object to compare against. </param>
        /// <returns> An integer value describing the relative valeus between the two objects. </returns>
        /// <exception cref="NotSupportedException"> Thrown if the compared object is not a Timestamp object. </exception>
        public int CompareTo(object obj)
        {
            if (obj is Timestamp)
            {
                return this.CompareTo((Timestamp)obj);
            }
           
            throw new NotSupportedException();
        }

        /// <summary> Compares two Timestamp objects to determine which has a greater value. </summary>
        /// <param name="obj"> A timestamp object to compare against. </param>
        /// <returns> An integer value describing the relative valeus between the two objects. </returns>
        public int CompareTo(Timestamp obj)
        {
            return this.Value.CompareTo(obj.Value);
        }

        /// <summary> Retrieves the byte array representation of a Timestamp object, as used by a replay file. </summary>
        /// <returns> Returns the Timestamp as a byte array, as used in replay files. </returns>
        public byte[] GetRawValue()
        {
            return FromInt32(this.Value);
        }

        #endregion

        #region Methods

        /// <summary> Parses a Timestamp integer from the current position in a BinaryReader. </summary>
        /// <param name="reader"> The binary reader, at the position containing the Timestamp object. </param>
        /// <returns> Returns the integer form of the Timestamp object parsed. </returns>
        private static int ParseTimestamp(BinaryReader reader)
        {
            byte one = reader.ReadByte();
            if ((one & 3) > 0)
            {
                int two = reader.ReadByte();
                two = (short)(((one >> 2) << 8) | two);

                if ((one & 3) >= 2)
                {
                    var tmp = reader.ReadByte();
                    two = (two << 8) | tmp;

                    if ((one & 3) == 3)
                    {
                        tmp = reader.ReadByte();
                        two = (two << 8) | tmp;
                    }
                }

                return two;
            }

            return one >> 2;
        }

        #endregion
    }
}