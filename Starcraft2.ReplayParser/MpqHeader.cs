// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MpqHeader.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Defines the MpqHeader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System;
    using System.IO;

    /// <summary> Parses the header at the beginning of the MPQ file structure. </summary>
    public class MpqHeader
    {
        /// <summary>
        /// Parses the MPQ header on a file to determine version and build numbers.
        /// </summary>
        /// <param name="replay">Replay object to store </param>
        /// <param name="filename">Filename of the file to open.</param>
        public static void ParseHeader(Replay replay, string filename)
        {
            using (var reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                byte[] magic = reader.ReadBytes(3);
                byte format = reader.ReadByte();

                byte[] buffer = reader.ReadBytes(4);
                var dataMaxSize = BitConverter.ToInt32(buffer, 0);

                buffer = reader.ReadBytes(4);
                int headerOffset = BitConverter.ToInt32(buffer, 0);

                buffer = reader.ReadBytes(4);
                int userDataHeaderSize = BitConverter.ToInt32(buffer, 0);

                int dataType = reader.ReadByte(); // Should be 0x05 = Array w/ Keys

                int numElements = ParseVlfNumber(reader);

                // The first value is the words: "Starcraft II Replay 11"
                int index = ParseVlfNumber(reader);

                int type = reader.ReadByte(); // Should be 0x02 = binary data

                int numValues = ParseVlfNumber(reader);
                byte[] starcraft2 = reader.ReadBytes(numValues);

                int index2 = ParseVlfNumber(reader);
                int type2 = reader.ReadByte(); // Should be 0x05 = Array w/ Keys

                int numElementsVersion = ParseVlfNumber(reader);
                var version = new int[numElementsVersion];

                while (numElementsVersion > 0)
                {
                    int i = ParseVlfNumber(reader);
                    int t = reader.ReadByte(); // Type;

                    if (t == 0x09)
                    {
                        // VLF
                        version[i] = ParseVlfNumber(reader);
                    }
                    else if (t == 0x06) 
                    {
                        // Byte
                        version[i] = reader.ReadByte();
                    }
                    else if (t == 0x07) 
                    {
                        // 4 Bytes
                        version[i] = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    }

                    numElementsVersion--;
                }

                // We now have the version. Just parse.
                replay.ReplayVersion = string.Format("{0}.{1}.{2}.{3}", version[0], version[1], version[2], version[3]);
                replay.ReplayBuild = version[4];

                reader.Close();
            }
        }

        /// <summary> Parses a single VLF number using a reader object. </summary>
        /// <param name="reader"> The BinaryReader to read the object from. </param>
        /// <returns> Returns an integer value, representing the VLF number parsed. </returns>
        private static int ParseVlfNumber(BinaryReader reader)
        {
            var bytes = 0;
            var first = true;
            var number = 0;
            var multiplier = 1;

            while (true)
            {
                var i = reader.ReadByte();

                number += (i & 0x7F) * (int)Math.Pow(2, bytes * 7);

                if (first)
                {
                    if ((number & 1) != 0)
                    {
                        multiplier = -1;
                        number--;
                    }

                    first = false;
                }

                if ((i & 0x80) == 0)
                {
                    break;
                }

                bytes++;
            }

            return (number / 2) * multiplier;
        }
    }
}
