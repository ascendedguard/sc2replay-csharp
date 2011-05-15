// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Timestamp.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Timestamp type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System;
    using System.IO;

    public class Timestamp : IComparable
    {
        private int value;

        public int Value
        {
            get { return this.value; }
        }

        public byte[] GetRawValue()
        {
            return FromInt32(this.value);
        }

        public TimeSpan TimeSpan
        {
            get
            {
                // 62.5 = 1000 / 16
                var milliseconds = (int)(this.value * 62.5);
                return new TimeSpan(0, 0, 0, 0, milliseconds);
            }
        }

        public static byte[] FromInt32(int value)
        {
            int bytesNeeded = 0;

            if (value > Math.Pow(2, 30))
            {
                throw new ArgumentOutOfRangeException("value", "Timestamp value cannot be greater than 2^30");
            }

            if (value > Math.Pow(2, 22))
            {
                bytesNeeded = 3;
            }

            if (value > Math.Pow(2, 14))
            {
                bytesNeeded = 2;
            }

            if (value > Math.Pow(2, 6))
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

        public static Timestamp Create(int value)
        {
            var timestamp = new Timestamp { value = value };
            return timestamp;
        }

        public static Timestamp Parse(BinaryReader reader)
        {
            int value = ParseTimestamp(reader);

            var timestamp = new Timestamp { value = value };
            return timestamp;
        }

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

        public int CompareTo(object obj)
        {
            if (obj is Timestamp)
            {
                return this.CompareTo((Timestamp)obj);
            }

            throw new NotImplementedException();
        }

        public int CompareTo(Timestamp obj)
        {
            return this.value.CompareTo(obj.value);
        }
    }
}
