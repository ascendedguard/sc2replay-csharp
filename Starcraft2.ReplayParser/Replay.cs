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

        private static readonly string mpqExtractorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MPQEditor.exe");

        /// <summary>
        /// Parses a .SC2Replay file and returns relevant replay information.
        /// </summary>
        /// <param name="fileName">Full path to a .SC2Replay file.</param>
        /// <exception cref="FileNotFoundException">Thrown if MPQEditor.exe is not found in the application directory.</exception>
        public static Replay Parse(string fileName)
        {
            if (File.Exists(mpqExtractorPath) == false)
            {
                throw new FileNotFoundException("MPQEditor.exe was not found in the current directory.", "MPQEditor.exe");
            }

            string tempDirectory = Path.Combine(Path.GetTempPath(), "SC2ReplayParser");

            Directory.CreateDirectory(tempDirectory);

            // Writes the MPQ Script to extract replay.details from the .sc2replay archive.
            string mpqScriptPath = Path.Combine(tempDirectory, "mpqScript.txt");
            string mpqScript = "e \"" + fileName + "\" replay.details \"" + tempDirectory + "\"";

            // The process tries to bring up a console window for a second. I hide this using the WindowStyle.
            File.WriteAllText(mpqScriptPath, mpqScript);
            ProcessStartInfo info = new ProcessStartInfo(mpqExtractorPath, "/console \"" + mpqScriptPath + "\"")
                                        {
                                            CreateNoWindow = true,
                                            WindowStyle = ProcessWindowStyle.Hidden
                                        };

            Process p = Process.Start(info);
            p.WaitForExit();

            File.Delete(mpqScriptPath);

            // Path to the extracted replay.details file.
            string replayDetailsPath = Path.Combine(tempDirectory, "replay.details");
            
            Replay replay = ParseReplayDetails(replayDetailsPath);

            File.Delete(replayDetailsPath);

            replay.Timestamp = File.GetCreationTime(fileName);

            return replay;
        }

        public static Replay ParseReplayDetails(string fileName)
        {
            var replay = new Replay();

            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(fileStream))
                {
                    reader.ReadBytes(6); // unknownHeader
                    byte doublePlayerCount = reader.ReadByte(); // doublePlayerCount;
                    int playerCount = (doublePlayerCount / 2);

                    // Parsing Player Info
                    var players = new PlayerDetails[playerCount];

                    for (int i = 0; i < playerCount; i++)
                    {
                        players[i] = ReadPlayerDetailStruct(reader);
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

        private static PlayerDetails ReadPlayerDetailStruct(BinaryReader reader)
        {
            PlayerDetails player = new PlayerDetails();

            reader.ReadBytes(4); // playerHeader
            byte doubleShortNameLength = reader.ReadByte(); // doubleShortNameLength
            int shortNameLength = doubleShortNameLength / 2;

            string shortName = new String(reader.ReadChars(shortNameLength));

            // These variables could be deleted, they're only there for debugging to ensure proper values.
            // Perhaps the expected values should be verified?
            byte[] unknown1 = reader.ReadBytes(3); // unknown1 "02 05 08"
            KeyValueStruct param1 = KeyValueStruct.Parse(reader); // param1 - key = "00 09"
            byte[] unknown2 = reader.ReadBytes(6); // unknown2 - Unknown
            KeyValueStruct param2 = KeyValueStruct.Parse(reader); // param2 - key = "04 09"
            byte[] unknown3 = reader.ReadBytes(2); // unknown3 - "06 02"

            // Unknown4 should be "04 02". There is an unknown number of bytes (1-4) before this.
            // Once i find unknown4, i can continue parsing.

            byte unknown4Start = reader.ReadByte();
            byte unknown4End = 0;

            while (unknown4End != 2)
            {
                while (unknown4Start != 4)
                {
                    unknown4Start = reader.ReadByte();
                }

                unknown4End = reader.ReadByte();
            }

            byte doubleRaceLength = reader.ReadByte();
            int raceLength = doubleRaceLength / 2;

            string race = new String(reader.ReadChars(raceLength));

            reader.ReadBytes(3); // unknown5

            var keys = new KeyValueStruct[9];
            // paramList - This contains the player's color and should eventually be parsed.
            for(int i = 0; i < 9; i++)
            {
                keys[i] = KeyValueStruct.Parse(reader);
            }

            return new PlayerDetails
                       {
                           Name = shortName,
                           Race = race,
                           Color =
                               Color.FromArgb((byte) keys[0].Value, (byte) keys[1].Value, (byte) keys[2].Value,
                                              (byte) keys[3].Value),
                           Team = keys[8].Value,
                       };
        }
    }
}
