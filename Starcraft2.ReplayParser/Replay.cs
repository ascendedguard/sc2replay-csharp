using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Linq;

namespace Starcraft2.ReplayParser
{
    public class Replay
    {
        protected Replay()
        {
        }

        public PlayerDetails[] Players { get; private set; }
        public string Map { get; private set; }
        public DateTime Timestamp { get; private set; }
        public GameSpeed GameSpeed { get; set; }
        public string TeamSize { get; set; }
        public GameType GameType { get; set; }

        /// <summary>
        /// Parses a .SC2Replay file and returns relevant replay information.
        /// </summary>
        /// <param name="fileName">Full path to a .SC2Replay file.</param>
        public static Replay Parse(string fileName)
        {
            Replay replay;

            using (var archive = new MpqLib.Mpq.CArchive(fileName))
            {
                var files = archive.FindFiles("replay.*");

                // Local scope allows the byte[] to be GC sooner, and prevents misreferences
                {
                    const string curFile = "replay.details";

                    var fileSize = (from f in files
                                    where f.FileName.Equals(curFile)
                                    select f).Single().Size;


                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    replay = ParseReplayDetails(buffer);
                }
                
                {
                    const string curFile = "replay.attributes.events";
                    var fileSize = (from f in files
                                    where f.FileName.Equals(curFile)
                                    select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    var replayAttributes = ReplayAttributeEvents.Parse(buffer);
                    replayAttributes.ApplyAttributes(replay);
                } 
            }
            replay.Timestamp = File.GetCreationTime(fileName);
            
            return replay;
        }

        private static Replay ParseReplayDetails(byte[] buffer)
        {
            Replay replay;
            using (var stream = new MemoryStream(buffer, false))
            {
                replay = ParseReplayDetails(stream);

                stream.Close();
            }

            return replay;
        }

        private static Replay ParseReplayDetails(Stream stream)
        {
            Replay replay = new Replay();

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

            return replay;
        }
    }
}
