using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;

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

        public ReplayAttributeEvents ReplayAttributeEvents { get; set; }

        /// <summary>
        /// Parses a .SC2Replay file and returns relevant replay information.
        /// </summary>
        /// <param name="fileName">Full path to a .SC2Replay file.</param>
        public static Replay Parse(string fileName)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), "SC2ReplayParser");

            Directory.CreateDirectory(tempDirectory);

            string replayDetailsPath = Path.Combine(tempDirectory, "replay.details");
            string replayAttributeEventsPath = Path.Combine(tempDirectory, "replay.attributes.events");
            using (var archive = new MpqLib.Mpq.CArchive(fileName))
            {
                archive.ExportFile("replay.details", replayDetailsPath);
                archive.ExportFile("replay.attributes.events", replayAttributeEventsPath);
            }

            // Path to the extracted replay.details file.
            Replay replay = ParseReplayDetails(replayDetailsPath);
            replay.ReplayAttributeEvents = ReplayAttributeEvents.Parse(replayAttributeEventsPath);
            replay.Timestamp = File.GetCreationTime(fileName);
            
            // Clean-up
            File.Delete(replayDetailsPath);
            File.Delete(replayAttributeEventsPath);

            return replay;
        }

        public static Replay ParseReplayDetails(string fileName)
        {
            var replay = new Replay();

            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(fileStream))
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

                fileStream.Close();
            }

            return replay;
        }
    }
}
