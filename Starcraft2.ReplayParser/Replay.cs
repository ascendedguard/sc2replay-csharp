using System;
using System.IO;
using System.Linq;

namespace Starcraft2.ReplayParser
{
    public class Replay
    {
        protected Replay()
        {
        }

        public string ReplayVersion { get; set; }
        public int ReplayBuild { get; set; }

        public PlayerDetails[] Players { get; private set; }
        public string Map { get; private set; }
        public DateTime Timestamp { get; private set; }
        public GameSpeed GameSpeed { get; set; }
        public string TeamSize { get; set; }
        public GameType GameType { get; set; }

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

                // The first value is always the version, lets extract it.
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

            while(true)
            {
                var i = reader.ReadByte();

                number += (i & 0x7F)*(int)Math.Pow(2, bytes*7);
                
                if(first)
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

            return (number/2)*multiplier;
        }

        /// <summary>
        /// Parses a .SC2Replay file and returns relevant replay information.
        /// </summary>
        /// <param name="fileName">Full path to a .SC2Replay file.</param>
        public static Replay Parse(string fileName)
        {
            var replay = new Replay();

            // File in the version numbers for later use.
            ParseHeader(replay, fileName);

            using (var archive = new MpqLib.Mpq.CArchive(fileName))
            {
                //archive.ToString
                var files = archive.FindFiles("replay.*");

                // Local scope allows the byte[] to be GC sooner, and prevents misreferences
                {
                    const string curFile = "replay.details";

                    var fileSize = (from f in files
                                    where f.FileName.Equals(curFile)
                                    select f).Single().Size;


                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    ParseReplayDetails(replay, buffer);
                }
                
                {
                    const string curFile = "replay.attributes.events";
                    var fileSize = (from f in files
                                    where f.FileName.Equals(curFile)
                                    select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    ReplayAttributeEvents.Parse(replay, buffer);
                } 
            }
            replay.Timestamp = File.GetCreationTime(fileName);
            
            return replay;
        }

        private static void ParseReplayDetails(Replay replay, byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer, false))
            {
                ParseReplayDetails(replay, stream);

                stream.Close();
            }
        }

        private static void ParseReplayDetails(Replay replay, Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                byte[] version = reader.ReadBytes(6); // unknownHeader
                byte doublePlayerCount = reader.ReadByte(); // doublePlayerCount;
                int playerCount = (doublePlayerCount / 2);

                // Parsing Player Info
                var players = new PlayerDetails[playerCount];

                for (int i = 0; i < playerCount; i++)
                {
                    players[i] = PlayerDetails.Parse(reader);
                }

                replay.Players = players;

                reader.ReadBytes(2); // unknown1
                byte doubleMapNameLength = reader.ReadByte();
                int mapNameLength = (doubleMapNameLength/2);

                replay.Map = new string(reader.ReadChars(mapNameLength));

                reader.Close();
            }
        }
    }
}
