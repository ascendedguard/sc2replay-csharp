using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Starcraft2.ReplayParser
{
    using System.Text;

    using MpqLib.Mpq;

    public class Replay
    {
        internal Replay()
        {
        }

        /// <summary>
        /// Gets the version number of the replay.
        /// </summary>
        public string ReplayVersion { get; internal set; }

        /// <summary>
        /// Gets the build number of the Starcraft 2 version used in creating the replay.
        /// </summary>
        public int ReplayBuild { get; internal set; }

        /// <summary>
        /// Gets the details of all players in the replay.
        /// </summary>
        public Player[] Players { get; internal set; }
        
        /// <summary>
        /// Gets the map the game was played on.
        /// </summary>
        public string Map { get; internal set; }

        /// <summary>
        /// Gets the Time at which the game took place.
        /// </summary>
        public DateTime Timestamp { get; internal set; }

        /// <summary>
        /// Gets the speed the game was played at.
        /// </summary>
        public GameSpeed GameSpeed { get; internal set; }

        /// <summary>
        /// Gets the team size of the selected gametype.
        /// </summary>
        /// <example>1v1, 2v2, 3v3, 4v4</example>
        public string TeamSize { get; internal set; }

        /// <summary> Gets the Gateway (KR, NA, etc.) that the game was played in. </summary>
        public string Gateway { get; internal set; }

        /// <summary>
        /// Gets the type of game this replay covers, whether it was a private or open match.
        /// </summary>
        public GameType GameType { get; internal set; }

        /// <summary>
        /// Gets a list of all chat messages which took place during the game.
        /// </summary>
        public IList<ChatMessage> ChatMessages { get; internal set; }

        public List<IGameEvent> PlayerEvents { get; internal set; }

        public Player GetPlayerById(int playerId)
        {
            int playerIndex = playerId - 1;

            if (playerIndex < this.Players.Length)
            {
                return this.Players[playerIndex];
            }

            return null;
        }

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
            
            CArchive archive;

            try
            {
                archive = new CArchive(fileName);
            }
            catch (IOException)
            {
                // Usually thrown if the archive name contains korean. Copy it to a local file and open.
                var tmpPath = Path.GetTempFileName();

                File.Copy(fileName, tmpPath, true);

                archive = new CArchive(tmpPath);
            }

            try
            {
                var files = archive.FindFiles("replay.*");

                {
                    const string curFile = "replay.initData";

                    var fileSize = (from f in files where f.FileName.Equals(curFile) select f).Single().Size;


                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    ReplayInitData.Parse(replay, buffer);
                }

                // Local scope allows the byte[] to be GC sooner, and prevents misreferences
                {
                    const string curFile = "replay.details";

                    var fileSize = (from f in files where f.FileName.Equals(curFile) select f).Single().Size;


                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    ReplayDetails.Parse(replay, buffer);
                }

                {
                    const string curFile = "replay.attributes.events";
                    var fileSize = (from f in files where f.FileName.Equals(curFile) select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    ReplayAttributeEvents.Parse(replay, buffer);
                }

                {
                    const string curFile = "replay.message.events";
                    var fileSize = (from f in files where f.FileName.Equals(curFile) select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    replay.ChatMessages = ReplayMessageEvents.Parse(buffer);
                }
                
                try
                {
                    const string curFile = "replay.game.events";
                    
                    var fileSize = (from f in files
                                    where f.FileName.Equals(curFile)
                                    select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(curFile, buffer);

                    replay.PlayerEvents = ReplayGameEvents.Parse(replay, buffer);
                }  
                catch(Exception)
                {
                    // In the current state, the parsing commonly fails.
                    // Incase of failing, we should probably just ignore the results of the parse
                    // And return.
                }
            }
            finally
            {
                archive.Dispose();
            }

            replay.Timestamp = File.GetCreationTime(fileName);
            
            return replay;
        }
    }
}
