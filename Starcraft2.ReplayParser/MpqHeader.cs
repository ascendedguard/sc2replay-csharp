using System;
using System.IO;

namespace Starcraft2.ReplayParser
{
    public class MpqHeader
    {
        /// <summary>
        /// Parses the MPQ header on a file to determine version and build numbers.
        /// </summary>
        /// <param name="replay">Replay object to store </param>
        /// <param name="filename"></param>
        public static void ParseHeader(Replay replay, string filename)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                byte[] magic = reader.ReadBytes(3);
                byte format = reader.ReadByte();

                byte[] buffer = reader.ReadBytes(4);
                int uDataMaxSize = BitConverter.ToInt32(buffer, 0);

                buffer = reader.ReadBytes(4);
                int headerOffset = BitConverter.ToInt32(buffer, 0);

                buffer = reader.ReadBytes(4);
                int userDataHeaderSize = BitConverter.ToInt32(buffer, 0);

                int dataType = reader.ReadByte(); // Should be 0x05 = Array w/ Keys

                int numElements = ParseVLFNumber(reader);

                // The first value is the words: "Starcraft II Replay 11"
                int index = ParseVLFNumber(reader);

                int type = reader.ReadByte(); // Should be 0x02 = binary data

                int numValues = ParseVLFNumber(reader); //reader.ReadByte();
                byte[] starcraft2 = reader.ReadBytes(numValues);

                int index2 = ParseVLFNumber(reader);
                int type2 = reader.ReadByte(); // Should be 0x05 = Array w/ Keys

                int numElementsVersion = ParseVLFNumber(reader);
                var version = new int[numElementsVersion];

                while (numElementsVersion > 0)
                {
                    int i = ParseVLFNumber(reader);
                    int t = reader.ReadByte(); // Type;

                    if (t == 0x09) //VLF
                    {
                        version[i] = ParseVLFNumber(reader);
                    }
                    else if (t == 0x06) //Byte
                    {
                        version[i] = reader.ReadByte();
                    }
                    else if (t == 0x07) //4 Bytes
                    {
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

        private static int ParseVLFNumber(BinaryReader reader)
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

                if ((i & 0x80) == 0) break;

                bytes++;
            }

            return (number / 2) * multiplier;
        }
    }
}
