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
