// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Replay.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Contains information about a Starcraft 2 Replay, and functions for parsing one.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using MpqLib.Mpq;

    /// <summary>
    /// Contains information about a Starcraft 2 Replay, and functions for parsing one.
    /// </summary>
    public class Replay
    {
        #region Constructors and Destructors

        /// <summary> Initializes a new instance of the <see cref = "Replay" /> class. </summary>
        internal Replay()
        {
            GameUnits = new Dictionary<int, Unit>();
            Players = new Player[0x10];
        }

        #endregion

        #region Public Properties

        /// <summary> Gets a list of all chat messages which took place during the game. </summary>
        public IList<ChatMessage> ChatMessages { get; internal set; }

        /// <summary> Gets the speed the game was played at. </summary>
        public GameSpeed GameSpeed { get; internal set; }

        /// <summary> Gets the type of game this replay covers, whether it was a private or open match. </summary>
        public GameType GameType { get; internal set; }

        /// <summary> Gets the Gateway (KR, NA, etc.) that the game was played in. </summary>
        public string Gateway { get; internal set; }

        /// <summary> Gets the map the game was played on. </summary>
        public string Map { get; internal set; }

        /// <summary> Gets the list of game events occuring during the course of the replay. </summary>
        public List<IGameEvent> PlayerEvents { get; internal set; }

        /// <summary> Gets the list of units appearing throughout the replay. </summary>
        public Dictionary<int, Unit> GameUnits { get; internal set; }

        /// <summary> Gets the details of all players in the replay. </summary>
        public Player[] Players { get; internal set; }

        /// <summary> Gets the build number of the Starcraft 2 version used in creating the replay. </summary>
        public int ReplayBuild { get; internal set; }

        /// <summary> Gets the version number of the replay. </summary>
        public string ReplayVersion { get; internal set; }

        /// <summary> Gets the team size of the selected gametype. </summary>
        public string TeamSize { get; internal set; }

        /// <summary> Gets the Time at which the game took place. </summary>
        public DateTime Timestamp { get; internal set; }

        /// <summary> Gets the length of the game. </summary>
        public TimeSpan GameLength { get; internal set; }

        #endregion

        #region Public Methods

        /// <summary> Parses a .SC2Replay file and returns relevant replay information.  </summary>
        /// <param name="fileName"> Full path to a .SC2Replay file.  </param>
        /// <returns> Returns the fully parsed Replay object. </returns>
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
                    const string CurFile = "replay.initData";

                    var fileSize = (from f in files where f.FileName.Equals(CurFile) select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(CurFile, buffer);

                    ReplayInitData.Parse(replay, buffer);
                }

                {
                    // Local scope allows the byte[] to be GC sooner, and prevents misreferences
                    const string CurFile = "replay.details";

                    var fileSize = (from f in files where f.FileName.Equals(CurFile) select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(CurFile, buffer);

                    ReplayDetails.Parse(replay, buffer);
                }

                {
                    const string CurFile = "replay.attributes.events";
                    var fileSize = (from f in files where f.FileName.Equals(CurFile) select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(CurFile, buffer);

                    ReplayAttributeEvents.Parse(replay, buffer);
                }

                {
                    const string CurFile = "replay.message.events";
                    var fileSize = (from f in files where f.FileName.Equals(CurFile) select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(CurFile, buffer);

                    replay.ChatMessages = ReplayMessageEvents.Parse(buffer);
                }

                try
                {
                    const string CurFile = "replay.game.events";

                    var fileSize = (from f in files where f.FileName.Equals(CurFile) select f).Single().Size;

                    var buffer = new byte[fileSize];

                    archive.ExportFile(CurFile, buffer);

                    replay.PlayerEvents = ReplayGameEvents.Parse(replay, buffer);
                }
                catch (Exception)
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

        /// <summary> Retrieves a player based on their player ID in a replay file. Used to reduce redundency when looking up players. </summary>
        /// <param name="playerId"> The player's ID. </param>
        /// <returns> Returns the appropriate Player from the Players array. </returns>
        public Player GetPlayerById(int playerId)
        {
            if (16 > playerId)
            {
                return this.Players[playerId];
            }

            return null;
        }

        public Unit GetUnitById(int unitId)
        {
            Unit unit = null;
            GameUnits.TryGetValue(unitId, out unit);
            return unit;
        }

        #endregion
    }
}
