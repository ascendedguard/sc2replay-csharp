using System;
using System.Collections.Generic;
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
        public IList<ChatMessage> ChatMessages { get; set; }

        /// <summary>
        /// Parses a .SC2Replay file and returns relevant replay information.
        /// </summary>
        /// <param name="fileName">Full path to a .SC2Replay file.</param>
        public static Replay Parse(string fileName)
        {
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException("The specified file does not exist.", fileName);
            }

            var replay = new Replay();

            // File in the version numbers for later use.
            MpqHeader.ParseHeader(replay, fileName);

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
                
                {
                    const string curFile = "replay.message.events";
                    var fileSize = (from f in files
                                    where f.FileName.Equals(curFile)
                                    select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    replay.ChatMessages = ReplayMessageEvents.Parse(buffer);
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
